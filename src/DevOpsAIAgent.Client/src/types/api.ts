// Base API Types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// CI/CD Event Types
export interface CICDEvent {
  id: string;
  repositoryId: string;
  repositoryName: string;
  eventType: 'push' | 'pull_request' | 'release' | 'deployment' | 'workflow_run';
  branch: string;
  commit: string;
  author: string;
  message: string;
  status: 'pending' | 'success' | 'failure' | 'cancelled';
  workflowName?: string;
  jobName?: string;
  buildNumber?: number;
  duration?: number;
  startedAt: string;
  completedAt?: string;
  createdAt: string;
  updatedAt: string;
  failureReason?: string;
  logs?: string;
  artifactsUrl?: string;
  pullRequestNumber?: number;
  aiAnalysisId?: string;
}

// Incident Types
export interface Incident {
  id: string;
  title: string;
  description: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
  status: 'open' | 'investigating' | 'resolved' | 'closed';
  assignedTo?: string;
  repositoryIds: string[];
  relatedEventIds: string[];
  tags: string[];
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string;
  resolutionNotes?: string;
}

export interface CreateIncidentRequest {
  title: string;
  description: string;
  severity: Incident['severity'];
  assignedTo?: string;
  repositoryIds: string[];
  relatedEventIds: string[];
  tags: string[];
}

export interface UpdateIncidentRequest {
  title?: string;
  description?: string;
  severity?: Incident['severity'];
  status?: Incident['status'];
  assignedTo?: string;
  repositoryIds?: string[];
  relatedEventIds?: string[];
  tags?: string[];
  resolutionNotes?: string;
}

// Repository Types
export interface Repository {
  id: string;
  name: string;
  fullName: string;
  owner: string;
  description?: string;
  isPrivate: boolean;
  defaultBranch: string;
  url: string;
  cloneUrl: string;
  webhookId?: string;
  webhookUrl?: string;
  isWebhookActive: boolean;
  lastEventAt?: string;
  eventCount: number;
  failureRate: number;
  averageBuildTime: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRepositoryRequest {
  name: string;
  fullName: string;
  owner: string;
  description?: string;
  isPrivate: boolean;
  defaultBranch: string;
  url: string;
  cloneUrl: string;
}

// AI Analysis Types
export interface AIAnalysis {
  id: string;
  eventId?: string;
  type: 'failure_analysis' | 'performance_review' | 'security_scan' | 'code_quality';
  input: string;
  context?: string;
  output: string;
  summary: string;
  recommendations: string[];
  severity?: 'low' | 'medium' | 'high' | 'critical';
  confidence: number;
  provider: string;
  model: string;
  processingTime: number;
  tokens: number;
  cost?: number;
  createdAt: string;
  metadata?: Record<string, any>;
}

export interface TriggerAnalysisRequest {
  eventId?: string;
  type: AIAnalysis['type'];
  input: string;
  context?: string;
  provider?: string;
}

// Dashboard Types
export interface DashboardSummary {
  totalRepositories: number;
  totalEvents: number;
  totalIncidents: number;
  activeIncidents: number;
  eventsToday: number;
  failureRate: number;
  averageBuildTime: number;
  topFailedRepositories: Array<{
    repositoryName: string;
    failureCount: number;
    failureRate: number;
  }>;
  recentEvents: CICDEvent[];
  recentIncidents: Incident[];
  systemHealth: {
    apiStatus: 'healthy' | 'degraded' | 'down';
    databaseStatus: 'healthy' | 'degraded' | 'down';
    webhookStatus: 'healthy' | 'degraded' | 'down';
    aiServiceStatus: 'healthy' | 'degraded' | 'down';
  };
  metrics: {
    successRate: number;
    averageResolutionTime: number;
    deploymentsToday: number;
    criticalIncidents: number;
  };
}

// SignalR Types
export interface SignalRMessage {
  type: 'event_created' | 'event_updated' | 'incident_created' | 'incident_updated' | 'analysis_completed';
  data: CICDEvent | Incident | AIAnalysis;
  timestamp: string;
}

// Query Parameters
export interface EventQueryParams {
  page?: number;
  pageSize?: number;
  repositoryId?: string;
  status?: CICDEvent['status'];
  eventType?: CICDEvent['eventType'];
  branch?: string;
  author?: string;
  dateFrom?: string;
  dateTo?: string;
  search?: string;
}

export interface IncidentQueryParams {
  page?: number;
  pageSize?: number;
  severity?: Incident['severity'];
  status?: Incident['status'];
  assignedTo?: string;
  repositoryId?: string;
  dateFrom?: string;
  dateTo?: string;
  search?: string;
}

export interface AnalysisQueryParams {
  page?: number;
  pageSize?: number;
  type?: AIAnalysis['type'];
  eventId?: string;
  dateFrom?: string;
  dateTo?: string;
  provider?: string;
  minConfidence?: number;
}

// Theme Types
export interface ThemeConfig {
  mode: 'light' | 'dark';
  primaryColor: string;
  accentColor: string;
}

// LLM Service Types
export interface LLMProvider {
  name: string;
  isHealthy: boolean;
  responseTime?: number;
  lastChecked?: string;
}

export interface ProviderStatus extends LLMProvider {
  models: string[];
  maxTokens: number;
}

export interface ProvidersResponse {
  success: boolean;
  providers: ProviderStatus[];
  defaultProvider: string;
}

export interface AnalysisRequest {
  prompt: string;
  context?: string;
  provider?: string;
  model?: string;
  maxTokens?: number;
  temperature?: number;
}

export interface AnalysisResult {
  output: string;
  provider: string;
  model: string;
  confidence: number;
  processingTime: number;
  tokensUsed: number;
  cost?: number;
  createdAt: string;
}

export interface AnalysisResponse {
  success: boolean;
  analysis?: AnalysisResult;
  message?: string;
  error?: string;
}

// Error Types
export interface AppError {
  code: string;
  message: string;
  details?: string;
  timestamp: string;
}