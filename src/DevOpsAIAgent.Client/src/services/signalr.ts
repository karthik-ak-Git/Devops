import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { SignalRMessage, CICDEvent, Incident, AIAnalysis } from '../types/api';

export interface SignalRCallbacks {
  onEventCreated?: (event: CICDEvent) => void;
  onEventUpdated?: (event: CICDEvent) => void;
  onIncidentCreated?: (incident: Incident) => void;
  onIncidentUpdated?: (incident: Incident) => void;
  onAnalysisCompleted?: (analysis: AIAnalysis) => void;
  onConnectionStateChanged?: (state: 'Connected' | 'Disconnected' | 'Reconnecting') => void;
  onError?: (error: string) => void;
}

class SignalRService {
  private connection: HubConnection | null = null;
  private callbacks: SignalRCallbacks = {};
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectInterval = 5000; // 5 seconds

  constructor() {
    this.initializeConnection();
  }

  private initializeConnection() {
    const hubUrl = `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'}/hubs/dashboard`;

    this.connection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        withCredentials: false,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: () => {
          this.reconnectAttempts++;
          if (this.reconnectAttempts > this.maxReconnectAttempts) {
            return null; // Stop reconnecting
          }
          return Math.min(1000 * Math.pow(2, this.reconnectAttempts), 30000); // Exponential backoff
        }
      })
      .configureLogging(LogLevel.Information)
      .build();

    this.setupEventHandlers();
  }

  private setupEventHandlers() {
    if (!this.connection) return;

    // Connection state handlers
    this.connection.onclose((error) => {
      console.warn('SignalR connection closed:', error);
      this.callbacks.onConnectionStateChanged?.('Disconnected');
      this.callbacks.onError?.('Connection lost. Attempting to reconnect...');
    });

    this.connection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
      this.callbacks.onConnectionStateChanged?.('Reconnecting');
    });

    this.connection.onreconnected(() => {
      console.log('SignalR reconnected');
      this.reconnectAttempts = 0;
      this.callbacks.onConnectionStateChanged?.('Connected');
    });

    // Event handlers
    this.connection.on('EventCreated', (event: CICDEvent) => {
      console.log('SignalR: Event created', event);
      this.callbacks.onEventCreated?.(event);
    });

    this.connection.on('EventUpdated', (event: CICDEvent) => {
      console.log('SignalR: Event updated', event);
      this.callbacks.onEventUpdated?.(event);
    });

    this.connection.on('IncidentCreated', (incident: Incident) => {
      console.log('SignalR: Incident created', incident);
      this.callbacks.onIncidentCreated?.(incident);
    });

    this.connection.on('IncidentUpdated', (incident: Incident) => {
      console.log('SignalR: Incident updated', incident);
      this.callbacks.onIncidentUpdated?.(incident);
    });

    this.connection.on('AnalysisCompleted', (analysis: AIAnalysis) => {
      console.log('SignalR: Analysis completed', analysis);
      this.callbacks.onAnalysisCompleted?.(analysis);
    });
  }

  async start(): Promise<void> {
    if (!this.connection) {
      this.initializeConnection();
    }

    try {
      if (this.connection?.state === 'Disconnected') {
        await this.connection.start();
        console.log('SignalR connected');
        this.callbacks.onConnectionStateChanged?.('Connected');
        this.reconnectAttempts = 0;
      }
    } catch (error) {
      console.error('Failed to start SignalR connection:', error);
      this.callbacks.onError?.('Failed to establish real-time connection');

      // Retry connection after delay
      setTimeout(() => {
        this.start();
      }, this.reconnectInterval);
    }
  }

  async stop(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop();
        console.log('SignalR disconnected');
        this.callbacks.onConnectionStateChanged?.('Disconnected');
      } catch (error) {
        console.error('Error stopping SignalR connection:', error);
      }
    }
  }

  setCallbacks(callbacks: SignalRCallbacks) {
    this.callbacks = { ...this.callbacks, ...callbacks };
  }

  getConnectionState(): string {
    return this.connection?.state || 'Disconnected';
  }

  isConnected(): boolean {
    return this.connection?.state === 'Connected';
  }

  // Method to send messages to server (if needed)
  async sendMessage(methodName: string, ...args: any[]): Promise<void> {
    if (this.connection?.state === 'Connected') {
      try {
        await this.connection.invoke(methodName, ...args);
      } catch (error) {
        console.error(`Error sending SignalR message ${methodName}:`, error);
        throw error;
      }
    } else {
      throw new Error('SignalR connection is not established');
    }
  }

  // Join specific groups (if server supports it)
  async joinGroup(groupName: string): Promise<void> {
    await this.sendMessage('JoinGroup', groupName);
  }

  async leaveGroup(groupName: string): Promise<void> {
    await this.sendMessage('LeaveGroup', groupName);
  }
}

// Create a singleton instance
const signalRService = new SignalRService();

export default signalRService;