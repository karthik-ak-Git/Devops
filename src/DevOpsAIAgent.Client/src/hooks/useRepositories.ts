import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { repositoriesService } from '../services/api';
import type { Repository, CreateRepositoryRequest } from '../types/api';
import toast from 'react-hot-toast';

// Query keys
export const repositoryKeys = {
  all: ['repositories'] as const,
  lists: () => [...repositoryKeys.all, 'list'] as const,
  details: () => [...repositoryKeys.all, 'detail'] as const,
  detail: (id: string) => [...repositoryKeys.details(), id] as const,
};

// Get all repositories
export const useRepositories = () => {
  return useQuery({
    queryKey: repositoryKeys.lists(),
    queryFn: () => repositoriesService.getRepositories(),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

// Get single repository
export const useRepository = (id: string) => {
  return useQuery({
    queryKey: repositoryKeys.detail(id),
    queryFn: () => repositoriesService.getRepository(id),
    enabled: !!id,
  });
};

// Add repository
export const useAddRepository = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (repository: CreateRepositoryRequest) => repositoriesService.addRepository(repository),
    onSuccess: (newRepository) => {
      // Invalidate and refetch repositories
      queryClient.invalidateQueries({ queryKey: repositoryKeys.all });

      // Add to cache
      queryClient.setQueryData(repositoryKeys.lists(), (oldData: Repository[] | undefined) => {
        if (!oldData) return [newRepository];
        return [...oldData, newRepository];
      });

      toast.success(`Repository "${newRepository.name}" added successfully`);
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || 'Failed to add repository');
    },
  });
};

// Remove repository
export const useRemoveRepository = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => repositoriesService.removeRepository(id),
    onSuccess: (_, id) => {
      // Remove from cache
      queryClient.setQueryData(repositoryKeys.lists(), (oldData: Repository[] | undefined) => {
        if (!oldData) return [];
        return oldData.filter(repo => repo.id !== id);
      });

      // Remove detail cache
      queryClient.removeQueries({ queryKey: repositoryKeys.detail(id) });

      toast.success('Repository removed successfully');
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || 'Failed to remove repository');
    },
  });
};

// Invalidate repositories cache
export const useInvalidateRepositories = () => {
  const queryClient = useQueryClient();

  return {
    invalidateAll: () => queryClient.invalidateQueries({ queryKey: repositoryKeys.all }),
    invalidateList: () => queryClient.invalidateQueries({ queryKey: repositoryKeys.lists() }),
    invalidateDetail: (id: string) =>
      queryClient.invalidateQueries({ queryKey: repositoryKeys.detail(id) }),
  };
};

// Update repository in cache
export const useUpdateRepositoryCache = () => {
  const queryClient = useQueryClient();

  return (updatedRepository: Repository) => {
    // Update the specific repository detail
    queryClient.setQueryData(repositoryKeys.detail(updatedRepository.id), updatedRepository);

    // Update the repository in list
    queryClient.setQueryData(repositoryKeys.lists(), (oldData: Repository[] | undefined) => {
      if (!oldData) return [updatedRepository];
      return oldData.map(repo =>
        repo.id === updatedRepository.id ? updatedRepository : repo
      );
    });
  };
};