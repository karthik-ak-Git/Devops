import { useQuery, useQueryClient } from '@tanstack/react-query';
import { eventsService } from '../services/api';
import type { CICDEvent, EventQueryParams, PaginatedResponse } from '../types/api';

// Query keys
export const eventKeys = {
  all: ['events'] as const,
  lists: () => [...eventKeys.all, 'list'] as const,
  list: (params: EventQueryParams) => [...eventKeys.lists(), params] as const,
  details: () => [...eventKeys.all, 'detail'] as const,
  detail: (id: string) => [...eventKeys.details(), id] as const,
};

// Get paginated events
export const useEvents = (params: EventQueryParams = {}) => {
  return useQuery({
    queryKey: eventKeys.list(params),
    queryFn: () => eventsService.getEvents(params),
    staleTime: 30 * 1000, // 30 seconds
    refetchInterval: 60 * 1000, // Refetch every minute
  });
};

// Get single event
export const useEvent = (id: string) => {
  return useQuery({
    queryKey: eventKeys.detail(id),
    queryFn: () => eventsService.getEvent(id),
    enabled: !!id,
  });
};

// Invalidate events cache
export const useInvalidateEvents = () => {
  const queryClient = useQueryClient();

  return {
    invalidateAll: () => queryClient.invalidateQueries({ queryKey: eventKeys.all }),
    invalidateList: (params?: EventQueryParams) =>
      params
        ? queryClient.invalidateQueries({ queryKey: eventKeys.list(params) })
        : queryClient.invalidateQueries({ queryKey: eventKeys.lists() }),
    invalidateDetail: (id: string) =>
      queryClient.invalidateQueries({ queryKey: eventKeys.detail(id) }),
  };
};

// Update event in cache (for real-time updates)
export const useUpdateEventCache = () => {
  const queryClient = useQueryClient();

  return (updatedEvent: CICDEvent) => {
    // Update the specific event detail
    queryClient.setQueryData(eventKeys.detail(updatedEvent.id), updatedEvent);

    // Update the event in all list queries
    queryClient.setQueriesData(
      { queryKey: eventKeys.lists() },
      (oldData: PaginatedResponse<CICDEvent> | undefined) => {
        if (!oldData) return oldData;

        return {
          ...oldData,
          items: oldData.items.map((event: CICDEvent) =>
            event.id === updatedEvent.id ? updatedEvent : event
          ),
        };
      }
    );
  };
};

// Add new event to cache (for real-time updates)
export const useAddEventToCache = () => {
  const queryClient = useQueryClient();

  return (newEvent: CICDEvent) => {
    // Add to all list queries
    queryClient.setQueriesData(
      { queryKey: eventKeys.lists() },
      (oldData: PaginatedResponse<CICDEvent> | undefined) => {
        if (!oldData) return oldData;

        return {
          ...oldData,
          items: [newEvent, ...oldData.items],
          totalCount: oldData.totalCount + 1,
        };
      }
    );

    // Set the new event detail
    queryClient.setQueryData(eventKeys.detail(newEvent.id), newEvent);
  };
};