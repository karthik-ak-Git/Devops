import axios from 'axios';
import {
  ApiResponse,
  PaginatedResponse,
  CICDEvent,
  Incident,
  CreateIncidentRequest,
  UpdateIncidentRequest,
  Repository,
  CreateRepositoryRequest,
  AIAnalysis,
  TriggerAnalysisRequest,
  DashboardSummary,
  EventQueryParams,
  IncidentQueryParams,
  AnalysisQueryParams,
  AnalysisRequest,
  AnalysisResponse,
  ProvidersResponse,
} from '../types/api';

// Create axios instance with base configuration
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
api.interceptors.request.use(
  config => {
    // Add auth token if available
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`);
    return config;
  },
  error => {
    console.error('[API Request Error]', error);
    return Promise.reject(error);
  }
);

// Response interceptor
api.interceptors.response.use(
  response => {
    console.log(`[API Response] ${response.status} ${response.config.url}`);
    return response;
  },
  error => {
    console.error('[API Response Error]', error.response?.data || error.message);
    if (error.response?.status === 401) {
      // Handle unauthorized access
      localStorage.removeItem('authToken');
      // Don't redirect in API service, let components handle it
    }
    return Promise.reject(error);
  }
);

// Helper function to build query string
const buildQueryString = (params: Record<string, any>): string => {
  const searchParams = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      searchParams.append(key, String(value));
    }
  });
  return searchParams.toString();
};

// Webhook Service
export const webhookService = {
  // Health check
  checkHealth: async (): Promise<ApiResponse<{ status: string; timestamp: string }>> => {
    const response = await api.get('/webhooks/health');
    return response.data;
  },
};

// CI/CD Events Service
export const eventsService = {
  // Get paginated events
  getEvents: async (params: EventQueryParams = {}): Promise<PaginatedResponse<CICDEvent>> => {
    const queryString = buildQueryString(params);
    const response = await api.get(`/cicd-events?${queryString}`);
    return response.data;
  },

  // Get specific event
  getEvent: async (id: string): Promise<CICDEvent> => {
    const response = await api.get(`/cicd-events/${id}`);
    return response.data;
  },
};

// Incidents Service
export const incidentsService = {
  // Get paginated incidents
  getIncidents: async (params: IncidentQueryParams = {}): Promise<PaginatedResponse<Incident>> => {
    const queryString = buildQueryString(params);
    const response = await api.get(`/incidents?${queryString}`);
    return response.data;
  },

  // Create incident
  createIncident: async (incident: CreateIncidentRequest): Promise<Incident> => {
    const response = await api.post('/incidents', incident);
    return response.data;
  },

  // Update incident
  updateIncident: async (id: string, updates: UpdateIncidentRequest): Promise<Incident> => {
    const response = await api.put(`/incidents/${id}`, updates);
    return response.data;
  },

  // Get incident by ID
  getIncident: async (id: string): Promise<Incident> => {
    const response = await api.get(`/incidents/${id}`);
    return response.data;
  },
};

// Repositories Service
export const repositoriesService = {
  // Get all repositories
  getRepositories: async (): Promise<Repository[]> => {
    const response = await api.get('/repositories');
    return response.data;
  },

  // Add repository
  addRepository: async (repository: CreateRepositoryRequest): Promise<Repository> => {
    const response = await api.post('/repositories', repository);
    return response.data;
  },

  // Remove repository
  removeRepository: async (id: string): Promise<void> => {
    await api.delete(`/repositories/${id}`);
  },

  // Get repository by ID
  getRepository: async (id: string): Promise<Repository> => {
    const response = await api.get(`/repositories/${id}`);
    return response.data;
  },
};

// Dashboard Service
export const dashboardService = {
  // Get dashboard summary
  getSummary: async (): Promise<DashboardSummary> => {
    const response = await api.get('/dashboard/summary');
    return response.data;
  },
};

// AI Analysis Service
export const aiAnalysisService = {
  // Get analysis history
  getAnalyses: async (params: AnalysisQueryParams = {}): Promise<PaginatedResponse<AIAnalysis>> => {
    const queryString = buildQueryString(params);
    const response = await api.get(`/ai-analysis?${queryString}`);
    return response.data;
  },

  // Trigger AI analysis
  triggerAnalysis: async (request: TriggerAnalysisRequest): Promise<AIAnalysis> => {
    const response = await api.post('/ai-analysis/analyze', request);
    return response.data;
  },

  // Get specific analysis
  getAnalysis: async (id: string): Promise<AIAnalysis> => {
    const response = await api.get(`/ai-analysis/${id}`);
    return response.data;
  },
};

// LLM Service (matching .NET API endpoints)
export const llmService = {
  // Get available providers
  getProviders: async (): Promise<ProvidersResponse> => {
    const response = await api.get('/analysis/providers');
    return response.data;
  },

  // Generate analysis using DevOps endpoint
  generateAnalysis: async (request: AnalysisRequest): Promise<AnalysisResponse> => {
    const response = await api.post('/analysis/devops', request);
    return response.data;
  },

  // Generate analysis with automatic fallback
  generateAnalysisWithFallback: async (request: AnalysisRequest): Promise<AnalysisResponse> => {
    const response = await api.post('/analysis/devops', {
      ...request,
      useFallback: true
    });
    return response.data;
  },

  // Health check for LLM service
  checkHealth: async (): Promise<ApiResponse<{ status: string }>> => {
    const response = await api.get('/analysis/health');
    return response.data;
  },
};

// Export the axios instance for custom requests
export default api;