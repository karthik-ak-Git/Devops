import { useEffect, useState, useCallback } from 'react';
import signalRService, { SignalRCallbacks } from '../services/signalr';
import { useUpdateEventCache, useAddEventToCache } from './useEvents';
import { useUpdateIncidentCache, useAddIncidentToCache } from './useIncidents';
import { useAddAnalysisToCache } from './useAnalysis';
import { useQueryClient } from '@tanstack/react-query';
import { dashboardKeys } from './useDashboard';
import toast from 'react-hot-toast';
import type { CICDEvent, Incident, AIAnalysis } from '../types/api';

export interface SignalRState {
  isConnected: boolean;
  connectionState: string;
  isReconnecting: boolean;
  error: string | null;
}

export const useSignalR = (enableNotifications = true) => {
  const [state, setState] = useState<SignalRState>({
    isConnected: false,
    connectionState: 'Disconnected',
    isReconnecting: false,
    error: null,
  });

  const updateEventCache = useUpdateEventCache();
  const addEventToCache = useAddEventToCache();
  const updateIncidentCache = useUpdateIncidentCache();
  const addIncidentToCache = useAddIncidentToCache();
  const addAnalysisToCache = useAddAnalysisToCache();
  const queryClient = useQueryClient();

  const handleConnectionStateChanged = useCallback((connectionState: string) => {
    setState(prev => ({
      ...prev,
      connectionState,
      isConnected: connectionState === 'Connected',
      isReconnecting: connectionState === 'Reconnecting',
      error: connectionState === 'Connected' ? null : prev.error,
    }));

    if (connectionState === 'Connected' && enableNotifications) {
      toast.success('Real-time connection established', { id: 'signalr-connected' });
    } else if (connectionState === 'Disconnected' && enableNotifications) {
      toast.error('Real-time connection lost', { id: 'signalr-disconnected' });
    }
  }, [enableNotifications]);

  const handleError = useCallback((error: string) => {
    setState(prev => ({
      ...prev,
      error,
    }));

    if (enableNotifications) {
      toast.error(error, { id: 'signalr-error' });
    }
  }, [enableNotifications]);

  const handleEventCreated = useCallback((event: CICDEvent) => {
    addEventToCache(event);

    // Invalidate dashboard to refresh metrics
    queryClient.invalidateQueries({ queryKey: dashboardKeys.summary() });

    if (enableNotifications) {
      toast.success(`New ${event.eventType} event from ${event.repositoryName}`, {
        duration: 4000,
      });
    }
  }, [addEventToCache, queryClient, enableNotifications]);

  const handleEventUpdated = useCallback((event: CICDEvent) => {
    updateEventCache(event);

    // Invalidate dashboard to refresh metrics
    queryClient.invalidateQueries({ queryKey: dashboardKeys.summary() });

    if (enableNotifications && event.status === 'failure') {
      toast.error(`Build failed: ${event.repositoryName} (${event.branch})`, {
        duration: 6000,
      });
    } else if (enableNotifications && event.status === 'success') {
      toast.success(`Build successful: ${event.repositoryName} (${event.branch})`, {
        duration: 3000,
      });
    }
  }, [updateEventCache, queryClient, enableNotifications]);

  const handleIncidentCreated = useCallback((incident: Incident) => {
    addIncidentToCache(incident);

    // Invalidate dashboard to refresh metrics
    queryClient.invalidateQueries({ queryKey: dashboardKeys.summary() });

    if (enableNotifications) {
      toast.error(`New ${incident.severity} incident: ${incident.title}`, {
        duration: 6000,
      });
    }
  }, [addIncidentToCache, queryClient, enableNotifications]);

  const handleIncidentUpdated = useCallback((incident: Incident) => {
    updateIncidentCache(incident);

    // Invalidate dashboard to refresh metrics
    queryClient.invalidateQueries({ queryKey: dashboardKeys.summary() });

    if (enableNotifications && incident.status === 'resolved') {
      toast.success(`Incident resolved: ${incident.title}`, {
        duration: 4000,
      });
    }
  }, [updateIncidentCache, queryClient, enableNotifications]);

  const handleAnalysisCompleted = useCallback((analysis: AIAnalysis) => {
    addAnalysisToCache(analysis);

    if (enableNotifications) {
      toast.success(`AI analysis completed: ${analysis.type}`, {
        duration: 4000,
      });
    }
  }, [addAnalysisToCache, enableNotifications]);

  useEffect(() => {
    const callbacks: SignalRCallbacks = {
      onConnectionStateChanged: handleConnectionStateChanged,
      onError: handleError,
      onEventCreated: handleEventCreated,
      onEventUpdated: handleEventUpdated,
      onIncidentCreated: handleIncidentCreated,
      onIncidentUpdated: handleIncidentUpdated,
      onAnalysisCompleted: handleAnalysisCompleted,
    };

    signalRService.setCallbacks(callbacks);

    // Start connection
    signalRService.start().catch(error => {
      console.error('Failed to start SignalR connection:', error);
    });

    // Cleanup on unmount
    return () => {
      signalRService.stop();
    };
  }, [
    handleConnectionStateChanged,
    handleError,
    handleEventCreated,
    handleEventUpdated,
    handleIncidentCreated,
    handleIncidentUpdated,
    handleAnalysisCompleted,
  ]);

  const reconnect = useCallback(async () => {
    try {
      await signalRService.stop();
      await signalRService.start();
    } catch (error) {
      console.error('Failed to reconnect SignalR:', error);
    }
  }, []);

  const joinGroup = useCallback(async (groupName: string) => {
    try {
      await signalRService.joinGroup(groupName);
    } catch (error) {
      console.error(`Failed to join group ${groupName}:`, error);
    }
  }, []);

  const leaveGroup = useCallback(async (groupName: string) => {
    try {
      await signalRService.leaveGroup(groupName);
    } catch (error) {
      console.error(`Failed to leave group ${groupName}:`, error);
    }
  }, []);

  return {
    ...state,
    reconnect,
    joinGroup,
    leaveGroup,
    service: signalRService,
  };
};