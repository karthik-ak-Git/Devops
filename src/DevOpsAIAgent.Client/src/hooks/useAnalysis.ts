import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { aiAnalysisService } from '../services/api';
import type { AIAnalysis, TriggerAnalysisRequest, AnalysisQueryParams, PaginatedResponse } from '../types/api';
import toast from 'react-hot-toast';

// Query keys
export const analysisKeys = {
  all: ['analysis'] as const,
  lists: () => [...analysisKeys.all, 'list'] as const,
  list: (params: AnalysisQueryParams) => [...analysisKeys.lists(), params] as const,
  details: () => [...analysisKeys.all, 'detail'] as const,
  detail: (id: string) => [...analysisKeys.details(), id] as const,
};

// Get paginated analyses
export const useAnalyses = (params: AnalysisQueryParams = {}) => {
  return useQuery({
    queryKey: analysisKeys.list(params),
    queryFn: () => aiAnalysisService.getAnalyses(params),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

// Get single analysis
export const useAnalysis = (id: string) => {
  return useQuery({
    queryKey: analysisKeys.detail(id),
    queryFn: () => aiAnalysisService.getAnalysis(id),
    enabled: !!id,
  });
};

// Trigger AI analysis
export const useTriggerAnalysis = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: TriggerAnalysisRequest) => aiAnalysisService.triggerAnalysis(request),
    onSuccess: (newAnalysis) => {
      // Invalidate and refetch analyses
      queryClient.invalidateQueries({ queryKey: analysisKeys.all });

      // Add to cache
      queryClient.setQueriesData(
        { queryKey: analysisKeys.lists() },
        (oldData: PaginatedResponse<AIAnalysis> | undefined) => {
          if (!oldData) return oldData;
          return {
            ...oldData,
            items: [newAnalysis, ...oldData.items],
            totalCount: oldData.totalCount + 1,
          };
        }
      );

      toast.success('AI analysis started successfully');
    },
    onError: (error: Error) => {
      toast.error(error?.message || 'Failed to trigger analysis');
    },
  });
};

// Invalidate analyses cache
export const useInvalidateAnalyses = () => {
  const queryClient = useQueryClient();

  return {
    invalidateAll: () => queryClient.invalidateQueries({ queryKey: analysisKeys.all }),
    invalidateList: (params?: AnalysisQueryParams) =>
      params
        ? queryClient.invalidateQueries({ queryKey: analysisKeys.list(params) })
        : queryClient.invalidateQueries({ queryKey: analysisKeys.lists() }),
    invalidateDetail: (id: string) =>
      queryClient.invalidateQueries({ queryKey: analysisKeys.detail(id) }),
  };
};

// Update analysis in cache (for real-time updates)
export const useUpdateAnalysisCache = () => {
  const queryClient = useQueryClient();

  return (updatedAnalysis: AIAnalysis) => {
    // Update the specific analysis detail
    queryClient.setQueryData(analysisKeys.detail(updatedAnalysis.id), updatedAnalysis);

    // Update the analysis in all list queries
    queryClient.setQueriesData(
      { queryKey: analysisKeys.lists() },
      (oldData: any) => {
        if (!oldData) return oldData;

        return {
          ...oldData,
          items: oldData.items.map((analysis: AIAnalysis) =>
            analysis.id === updatedAnalysis.id ? updatedAnalysis : analysis
          ),
        };
      }
    );
  };
};

// Add new analysis to cache (for real-time updates)
export const useAddAnalysisToCache = () => {
  const queryClient = useQueryClient();

  return (newAnalysis: AIAnalysis) => {
    // Add to all list queries
    queryClient.setQueriesData(
      { queryKey: analysisKeys.lists() },
      (oldData: any) => {
        if (!oldData) return oldData;

        return {
          ...oldData,
          items: [newAnalysis, ...oldData.items],
          totalCount: oldData.totalCount + 1,
        };
      }
    );

    // Set the new analysis detail
    queryClient.setQueryData(analysisKeys.detail(newAnalysis.id), newAnalysis);
  };
};