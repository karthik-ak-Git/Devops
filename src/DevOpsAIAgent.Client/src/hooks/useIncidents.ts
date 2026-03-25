import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { incidentsService } from '../services/api';
import type { Incident, CreateIncidentRequest, UpdateIncidentRequest, IncidentQueryParams, PaginatedResponse } from '../types/api';
import toast from 'react-hot-toast';

// Query keys
export const incidentKeys = {
  all: ['incidents'] as const,
  lists: () => [...incidentKeys.all, 'list'] as const,
  list: (params: IncidentQueryParams) => [...incidentKeys.lists(), params] as const,
  details: () => [...incidentKeys.all, 'detail'] as const,
  detail: (id: string) => [...incidentKeys.details(), id] as const,
};

// Get paginated incidents
export const useIncidents = (params: IncidentQueryParams = {}) => {
  return useQuery({
    queryKey: incidentKeys.list(params),
    queryFn: () => incidentsService.getIncidents(params),
    staleTime: 30 * 1000, // 30 seconds
  });
};

// Get single incident
export const useIncident = (id: string) => {
  return useQuery({
    queryKey: incidentKeys.detail(id),
    queryFn: () => incidentsService.getIncident(id),
    enabled: !!id,
  });
};

// Create incident
export const useCreateIncident = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (incident: CreateIncidentRequest) => incidentsService.createIncident(incident),
    onSuccess: (newIncident) => {
      // Invalidate and refetch incidents
      queryClient.invalidateQueries({ queryKey: incidentKeys.all });

      // Add to cache
      queryClient.setQueriesData(
        { queryKey: incidentKeys.lists() },
        (oldData: PaginatedResponse<Incident> | undefined) => {
          if (!oldData) return oldData;
          return {
            ...oldData,
            items: [newIncident, ...oldData.items],
            totalCount: oldData.totalCount + 1,
          };
        }
      );

      toast.success('Incident created successfully');
    },
    onError: (error: Error) => {
      toast.error(error?.message || 'Failed to create incident');
    },
  });
};

// Update incident
export const useUpdateIncident = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, updates }: { id: string; updates: UpdateIncidentRequest }) =>
      incidentsService.updateIncident(id, updates),
    onSuccess: (updatedIncident) => {
      // Update specific incident detail
      queryClient.setQueryData(incidentKeys.detail(updatedIncident.id), updatedIncident);

      // Update incident in all list queries
      queryClient.setQueriesData(
        { queryKey: incidentKeys.lists() },
        (oldData: PaginatedResponse<Incident> | undefined) => {
          if (!oldData) return oldData;
          return {
            ...oldData,
            items: oldData.items.map((incident: Incident) =>
              incident.id === updatedIncident.id ? updatedIncident : incident
            ),
          };
        }
      );

      toast.success('Incident updated successfully');
    },
    onError: (error: Error) => {
      toast.error(error?.message || 'Failed to update incident');
    },
  });
};

// Invalidate incidents cache
export const useInvalidateIncidents = () => {
  const queryClient = useQueryClient();

  return {
    invalidateAll: () => queryClient.invalidateQueries({ queryKey: incidentKeys.all }),
    invalidateList: (params?: IncidentQueryParams) =>
      params
        ? queryClient.invalidateQueries({ queryKey: incidentKeys.list(params) })
        : queryClient.invalidateQueries({ queryKey: incidentKeys.lists() }),
    invalidateDetail: (id: string) =>
      queryClient.invalidateQueries({ queryKey: incidentKeys.detail(id) }),
  };
};

// Update incident in cache (for real-time updates)
export const useUpdateIncidentCache = () => {
  const queryClient = useQueryClient();

  return (updatedIncident: Incident) => {
    // Update the specific incident detail
    queryClient.setQueryData(incidentKeys.detail(updatedIncident.id), updatedIncident);

    // Update the incident in all list queries
    queryClient.setQueriesData(
      { queryKey: incidentKeys.lists() },
      (oldData: PaginatedResponse<Incident> | undefined) => {
        if (!oldData) return oldData;

        return {
          ...oldData,
          items: oldData.items.map((incident: Incident) =>
            incident.id === updatedIncident.id ? updatedIncident : incident
          ),
        };
      }
    );
  };
};

// Add new incident to cache (for real-time updates)
export const useAddIncidentToCache = () => {
  const queryClient = useQueryClient();

  return (newIncident: Incident) => {
    // Add to all list queries
    queryClient.setQueriesData(
      { queryKey: incidentKeys.lists() },
      (oldData: PaginatedResponse<Incident> | undefined) => {
        if (!oldData) return oldData;

        return {
          ...oldData,
          items: [newIncident, ...oldData.items],
          totalCount: oldData.totalCount + 1,
        };
      }
    );

    // Set the new incident detail
    queryClient.setQueryData(incidentKeys.detail(newIncident.id), newIncident);
  };
};