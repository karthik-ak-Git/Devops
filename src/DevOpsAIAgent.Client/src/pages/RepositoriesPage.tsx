import { useState } from 'react';
import { useRepositories, useAddRepository, useRemoveRepository } from '../hooks/useRepositories';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Badge } from '../components/ui/Badge';
import {
  Plus,
  Search,
  RefreshCw,
  GitBranch,
  ExternalLink,
  Trash2,
  Check,
  X,
  Globe,
  Lock,
  Activity,
  Clock,
  TrendingUp,
  AlertTriangle,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import { formatDistanceToNow } from 'date-fns';
import { CreateRepositoryRequest } from '../types/api';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const createRepositorySchema = z.object({
  name: z.string().min(1, 'Name is required'),
  fullName: z.string().min(1, 'Full name is required'),
  owner: z.string().min(1, 'Owner is required'),
  description: z.string().optional(),
  isPrivate: z.boolean(),
  defaultBranch: z.string().min(1, 'Default branch is required'),
  url: z.string().url('Invalid URL'),
  cloneUrl: z.string().url('Invalid clone URL'),
});

type CreateRepositoryFormData = z.infer<typeof createRepositorySchema>;

const RepositoriesPage = () => {
  const [search, setSearch] = useState('');
  const [showAddForm, setShowAddForm] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null);

  const { data: repositories, isLoading, error, refetch } = useRepositories();
  const addRepository = useAddRepository();
  const removeRepository = useRemoveRepository();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    watch,
  } = useForm<CreateRepositoryFormData>({
    resolver: zodResolver(createRepositorySchema),
    defaultValues: {
      isPrivate: false,
      defaultBranch: 'main',
    },
  });

  // Filter repositories based on search
  const filteredRepositories = repositories?.filter(repo =>
    repo.name.toLowerCase().includes(search.toLowerCase()) ||
    repo.fullName.toLowerCase().includes(search.toLowerCase()) ||
    repo.owner.toLowerCase().includes(search.toLowerCase())
  ) || [];

  const handleAddRepository = async (data: CreateRepositoryFormData) => {
    try {
      await addRepository.mutateAsync(data);
      setShowAddForm(false);
      reset();
    } catch (error) {
      console.error('Failed to add repository:', error);
    }
  };

  const handleRemoveRepository = async (id: string) => {
    try {
      await removeRepository.mutateAsync(id);
      setDeleteConfirm(null);
    } catch (error) {
      console.error('Failed to remove repository:', error);
    }
  };

  const closeAddForm = () => {
    setShowAddForm(false);
    reset();
  };

  const getRepositoryStats = (repo: any) => {
    const successRate = repo.eventCount > 0 ? ((repo.eventCount - (repo.eventCount * repo.failureRate)) / repo.eventCount * 100) : 0;
    return {
      successRate: successRate.toFixed(1),
      avgBuildTime: Math.round(repo.averageBuildTime / 60), // Convert to minutes
    };
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Repositories</h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            Manage tracked repositories and their webhook configurations
          </p>
        </div>
        <div className="flex space-x-2">
          <Button onClick={() => refetch()} variant="outline">
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => setShowAddForm(true)}>
            <Plus className="mr-2 h-4 w-4" />
            Add Repository
          </Button>
        </div>
      </div>

      {/* Add Repository Form */}
      {showAddForm && (
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Add New Repository</CardTitle>
            <Button variant="ghost" size="sm" onClick={closeAddForm}>
              <X className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(handleAddRepository)} className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Repository Name *
                  </label>
                  <Input
                    {...register('name')}
                    error={!!errors.name}
                    placeholder="my-app"
                  />
                  {errors.name && (
                    <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.name.message}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Full Name *
                  </label>
                  <Input
                    {...register('fullName')}
                    error={!!errors.fullName}
                    placeholder="owner/my-app"
                  />
                  {errors.fullName && (
                    <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.fullName.message}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Owner *
                  </label>
                  <Input
                    {...register('owner')}
                    error={!!errors.owner}
                    placeholder="owner"
                  />
                  {errors.owner && (
                    <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.owner.message}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Default Branch *
                  </label>
                  <Input
                    {...register('defaultBranch')}
                    error={!!errors.defaultBranch}
                    placeholder="main"
                  />
                  {errors.defaultBranch && (
                    <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.defaultBranch.message}</p>
                  )}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Description
                </label>
                <Input
                  {...register('description')}
                  placeholder="Brief description of the repository"
                />
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Repository URL *
                  </label>
                  <Input
                    {...register('url')}
                    error={!!errors.url}
                    placeholder="https://github.com/owner/my-app"
                  />
                  {errors.url && (
                    <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.url.message}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Clone URL *
                  </label>
                  <Input
                    {...register('cloneUrl')}
                    error={!!errors.cloneUrl}
                    placeholder="https://github.com/owner/my-app.git"
                  />
                  {errors.cloneUrl && (
                    <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.cloneUrl.message}</p>
                  )}
                </div>
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="isPrivate"
                  {...register('isPrivate')}
                  className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <label htmlFor="isPrivate" className="ml-2 block text-sm text-gray-700 dark:text-gray-300">
                  Private repository
                </label>
              </div>

              <div className="flex justify-end space-x-2 pt-4">
                <Button type="button" variant="outline" onClick={closeAddForm}>
                  Cancel
                </Button>
                <Button type="submit" loading={addRepository.isPending}>
                  Add Repository
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      {/* Search */}
      <Card>
        <CardContent className="p-6">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
            <Input
              placeholder="Search repositories..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-10"
            />
          </div>
        </CardContent>
      </Card>

      {/* Results */}
      {isLoading ? (
        <Card>
          <CardContent className="p-12 text-center">
            <RefreshCw className="mx-auto h-8 w-8 animate-spin text-gray-400" />
            <p className="mt-2 text-gray-500 dark:text-gray-400">Loading repositories...</p>
          </CardContent>
        </Card>
      ) : error ? (
        <Card>
          <CardContent className="p-12 text-center">
            <p className="text-red-600 dark:text-red-400">
              Failed to load repositories: {error instanceof Error ? error.message : 'Unknown error'}
            </p>
            <Button onClick={() => refetch()} className="mt-4">
              Try Again
            </Button>
          </CardContent>
        </Card>
      ) : filteredRepositories.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <GitBranch className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900 dark:text-white">
              {repositories?.length === 0 ? 'No repositories' : 'No repositories match your search'}
            </h3>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
              {repositories?.length === 0
                ? 'Get started by adding your first repository.'
                : 'Try adjusting your search criteria.'
              }
            </p>
            {repositories?.length === 0 && (
              <div className="mt-6">
                <Button onClick={() => setShowAddForm(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Add Repository
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          {filteredRepositories.map((repo) => {
            const stats = getRepositoryStats(repo);
            return (
              <Card key={repo.id}>
                <CardContent className="p-6">
                  <div className="flex items-start justify-between mb-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center space-x-2 mb-2">
                        {repo.isPrivate ? (
                          <Lock className="h-4 w-4 text-gray-400" />
                        ) : (
                          <Globe className="h-4 w-4 text-gray-400" />
                        )}
                        <h3 className="text-lg font-medium text-gray-900 dark:text-white truncate">
                          {repo.name}
                        </h3>
                      </div>
                      <p className="text-sm text-gray-500 dark:text-gray-400 mb-1">
                        {repo.fullName}
                      </p>
                      {repo.description && (
                        <p className="text-sm text-gray-600 dark:text-gray-400 line-clamp-2">
                          {repo.description}
                        </p>
                      )}
                    </div>

                    <div className="flex space-x-1 ml-2">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => window.open(repo.url, '_blank')}
                      >
                        <ExternalLink className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => setDeleteConfirm(repo.id)}
                        className="text-red-600 hover:text-red-700"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>

                  {/* Webhook Status */}
                  <div className="flex items-center justify-between py-3 border-t border-gray-200 dark:border-gray-700">
                    <div className="flex items-center space-x-2">
                      <span className="text-sm text-gray-500 dark:text-gray-400">Webhook:</span>
                      {repo.isWebhookActive ? (
                        <Badge variant="success" size="sm">
                          <Check className="h-3 w-3 mr-1" />
                          Active
                        </Badge>
                      ) : (
                        <Badge variant="error" size="sm">
                          <X className="h-3 w-3 mr-1" />
                          Inactive
                        </Badge>
                      )}
                    </div>
                    <Badge variant="secondary" size="sm">
                      {repo.defaultBranch}
                    </Badge>
                  </div>

                  {/* Statistics */}
                  <div className="grid grid-cols-2 gap-4 mt-4">
                    <div className="text-center">
                      <div className="flex items-center justify-center">
                        <Activity className="h-4 w-4 text-blue-500 mr-1" />
                        <span className="text-lg font-semibold text-gray-900 dark:text-white">
                          {repo.eventCount}
                        </span>
                      </div>
                      <p className="text-xs text-gray-500 dark:text-gray-400">Events</p>
                    </div>

                    <div className="text-center">
                      <div className="flex items-center justify-center">
                        <TrendingUp className={`h-4 w-4 mr-1 ${
                          parseFloat(stats.successRate) >= 90
                            ? 'text-green-500'
                            : parseFloat(stats.successRate) >= 70
                            ? 'text-yellow-500'
                            : 'text-red-500'
                        }`} />
                        <span className="text-lg font-semibold text-gray-900 dark:text-white">
                          {stats.successRate}%
                        </span>
                      </div>
                      <p className="text-xs text-gray-500 dark:text-gray-400">Success</p>
                    </div>

                    {stats.avgBuildTime > 0 && (
                      <div className="text-center col-span-2">
                        <div className="flex items-center justify-center">
                          <Clock className="h-4 w-4 text-purple-500 mr-1" />
                          <span className="text-lg font-semibold text-gray-900 dark:text-white">
                            {stats.avgBuildTime}m
                          </span>
                        </div>
                        <p className="text-xs text-gray-500 dark:text-gray-400">Avg Build Time</p>
                      </div>
                    )}
                  </div>

                  {/* Last Activity */}
                  {repo.lastEventAt && (
                    <div className="mt-4 pt-4 border-t border-gray-200 dark:border-gray-700">
                      <p className="text-xs text-gray-500 dark:text-gray-400">
                        Last activity: {formatDistanceToNow(new Date(repo.lastEventAt), { addSuffix: true })}
                      </p>
                    </div>
                  )}

                  {/* High Failure Rate Warning */}
                  {repo.failureRate > 0.3 && (
                    <div className="mt-3 p-2 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg">
                      <div className="flex items-center">
                        <AlertTriangle className="h-4 w-4 text-yellow-600 dark:text-yellow-400 mr-2" />
                        <p className="text-xs text-yellow-800 dark:text-yellow-200">
                          High failure rate ({(repo.failureRate * 100).toFixed(1)}%)
                        </p>
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
            );
          })}
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deleteConfirm && (
        <div className="fixed inset-0 z-50 bg-gray-600 bg-opacity-75 flex items-center justify-center">
          <Card className="w-full max-w-md mx-4">
            <CardHeader>
              <CardTitle className="flex items-center text-red-600">
                <AlertTriangle className="h-5 w-5 mr-2" />
                Confirm Deletion
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-gray-600 dark:text-gray-400 mb-4">
                Are you sure you want to remove this repository? This action cannot be undone and will:
              </p>
              <ul className="list-disc list-inside text-sm text-gray-500 dark:text-gray-400 mb-6 space-y-1">
                <li>Stop webhook notifications</li>
                <li>Remove all related events</li>
                <li>Delete configuration data</li>
              </ul>
              <div className="flex justify-end space-x-2">
                <Button
                  variant="outline"
                  onClick={() => setDeleteConfirm(null)}
                >
                  Cancel
                </Button>
                <Button
                  variant="danger"
                  loading={removeRepository.isPending}
                  onClick={() => handleRemoveRepository(deleteConfirm)}
                >
                  Delete Repository
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  );
};

export default RepositoriesPage;