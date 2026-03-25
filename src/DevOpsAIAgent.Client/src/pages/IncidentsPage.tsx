import { useState } from 'react';
import { useIncidents, useCreateIncident, useUpdateIncident } from '../hooks/useIncidents';
import { useRepositories } from '../hooks/useRepositories';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Textarea } from '../components/ui/Textarea';
import { Select } from '../components/ui/Select';
import { StatusBadge } from '../components/ui/StatusBadge';
import { Badge } from '../components/ui/Badge';
import {
  Plus,
  Search,
  Filter,
  RefreshCw,
  Eye,
  Edit,
  X,
  AlertTriangle,
  Clock,
  User,
  Tag,
  ChevronLeft,
  ChevronRight,
} from 'lucide-react';
import { formatDistanceToNow } from 'date-fns';
import { IncidentQueryParams, CreateIncidentRequest, UpdateIncidentRequest, Incident } from '../types/api';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const createIncidentSchema = z.object({
  title: z.string().min(1, 'Title is required').max(200, 'Title too long'),
  description: z.string().min(1, 'Description is required'),
  severity: z.enum(['low', 'medium', 'high', 'critical']),
  assignedTo: z.string().optional(),
  repositoryIds: z.array(z.string()),
  tags: z.string().optional(),
});

type CreateIncidentFormData = z.infer<typeof createIncidentSchema>;

const IncidentsPage = () => {
  const [filters, setFilters] = useState<IncidentQueryParams>({
    page: 1,
    pageSize: 20,
  });
  const [search, setSearch] = useState('');
  const [showFilters, setShowFilters] = useState(false);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingIncident, setEditingIncident] = useState<Incident | null>(null);

  const { data, isLoading, error, refetch } = useIncidents(filters);
  const { data: repositories } = useRepositories();
  const createIncident = useCreateIncident();
  const updateIncident = useUpdateIncident();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    setValue,
    watch,
  } = useForm<CreateIncidentFormData>({
    resolver: zodResolver(createIncidentSchema),
    defaultValues: {
      repositoryIds: [],
      severity: 'medium',
    },
  });

  const handleFilterChange = (key: keyof IncidentQueryParams, value: any) => {
    setFilters(prev => ({
      ...prev,
      [key]: value,
      page: 1,
    }));
  };

  const handleSearch = () => {
    setFilters(prev => ({
      ...prev,
      search: search.trim() || undefined,
      page: 1,
    }));
  };

  const handleCreateIncident = async (data: CreateIncidentFormData) => {
    const tags = data.tags ? data.tags.split(',').map(tag => tag.trim()).filter(Boolean) : [];

    const incidentData: CreateIncidentRequest = {
      title: data.title,
      description: data.description,
      severity: data.severity,
      assignedTo: data.assignedTo || undefined,
      repositoryIds: data.repositoryIds,
      relatedEventIds: [],
      tags,
    };

    try {
      await createIncident.mutateAsync(incidentData);
      setShowCreateForm(false);
      reset();
    } catch (error) {
      console.error('Failed to create incident:', error);
    }
  };

  const handleUpdateIncident = async (incident: Incident, updates: Partial<UpdateIncidentRequest>) => {
    try {
      await updateIncident.mutateAsync({
        id: incident.id,
        updates,
      });
      setEditingIncident(null);
    } catch (error) {
      console.error('Failed to update incident:', error);
    }
  };

  const openEditForm = (incident: Incident) => {
    setEditingIncident(incident);
    setValue('title', incident.title);
    setValue('description', incident.description);
    setValue('severity', incident.severity);
    setValue('assignedTo', incident.assignedTo || '');
    setValue('repositoryIds', incident.repositoryIds);
    setValue('tags', incident.tags.join(', '));
    setShowCreateForm(true);
  };

  const closeForm = () => {
    setShowCreateForm(false);
    setEditingIncident(null);
    reset();
  };

  const totalPages = data ? Math.ceil(data.totalCount / (filters.pageSize || 20)) : 0;

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Incidents</h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            Track and manage DevOps incidents and issues
          </p>
        </div>
        <div className="flex space-x-2">
          <Button onClick={() => refetch()} variant="outline">
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => setShowCreateForm(true)}>
            <Plus className="mr-2 h-4 w-4" />
            New Incident
          </Button>
        </div>
      </div>

      {/* Create/Edit Form */}
      {showCreateForm && (
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>{editingIncident ? 'Edit Incident' : 'Create New Incident'}</CardTitle>
            <Button variant="ghost" size="sm" onClick={closeForm}>
              <X className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(handleCreateIncident)} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Title *
                </label>
                <Input
                  {...register('title')}
                  error={!!errors.title}
                  placeholder="Brief description of the incident"
                />
                {errors.title && (
                  <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.title.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Description *
                </label>
                <Textarea
                  {...register('description')}
                  error={!!errors.description}
                  placeholder="Detailed description of the incident, impact, and any relevant information"
                  rows={4}
                />
                {errors.description && (
                  <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.description.message}</p>
                )}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Severity *
                  </label>
                  <Select {...register('severity')} error={!!errors.severity}>
                    <option value="low">Low</option>
                    <option value="medium">Medium</option>
                    <option value="high">High</option>
                    <option value="critical">Critical</option>
                  </Select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Assigned To
                  </label>
                  <Input
                    {...register('assignedTo')}
                    placeholder="Username or email of the assignee"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Affected Repositories
                </label>
                <Select
                  multiple
                  {...register('repositoryIds')}
                  className="h-32"
                  onChange={(e) => {
                    const values = Array.from(e.target.selectedOptions, option => option.value);
                    setValue('repositoryIds', values);
                  }}
                >
                  {repositories?.map((repo) => (
                    <option key={repo.id} value={repo.id}>
                      {repo.name}
                    </option>
                  ))}
                </Select>
                <p className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                  Hold Ctrl/Cmd to select multiple repositories
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Tags
                </label>
                <Input
                  {...register('tags')}
                  placeholder="production, database, api (comma-separated)"
                />
                <p className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                  Separate multiple tags with commas
                </p>
              </div>

              <div className="flex justify-end space-x-2 pt-4">
                <Button type="button" variant="outline" onClick={closeForm}>
                  Cancel
                </Button>
                <Button
                  type="submit"
                  loading={createIncident.isPending || updateIncident.isPending}
                >
                  {editingIncident ? 'Update' : 'Create'} Incident
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      {/* Search and Filters */}
      <Card>
        <CardContent className="p-6">
          <div className="flex flex-col space-y-4 md:flex-row md:space-y-0 md:space-x-4">
            <div className="flex-1 flex space-x-2">
              <div className="relative flex-1">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  placeholder="Search incidents..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                  className="pl-10"
                />
              </div>
              <Button onClick={handleSearch}>Search</Button>
            </div>
            <Button
              variant="outline"
              onClick={() => setShowFilters(!showFilters)}
            >
              <Filter className="mr-2 h-4 w-4" />
              Filters
            </Button>
          </div>

          {showFilters && (
            <div className="mt-4 pt-4 border-t border-gray-200 dark:border-gray-700">
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                <Select
                  value={filters.status || ''}
                  onChange={(e) => handleFilterChange('status', e.target.value || undefined)}
                >
                  <option value="">All Statuses</option>
                  <option value="open">Open</option>
                  <option value="investigating">Investigating</option>
                  <option value="resolved">Resolved</option>
                  <option value="closed">Closed</option>
                </Select>

                <Select
                  value={filters.severity || ''}
                  onChange={(e) => handleFilterChange('severity', e.target.value || undefined)}
                >
                  <option value="">All Severities</option>
                  <option value="low">Low</option>
                  <option value="medium">Medium</option>
                  <option value="high">High</option>
                  <option value="critical">Critical</option>
                </Select>

                <Input
                  placeholder="Assigned to..."
                  value={filters.assignedTo || ''}
                  onChange={(e) => handleFilterChange('assignedTo', e.target.value || undefined)}
                />

                <Select
                  value={filters.repositoryId || ''}
                  onChange={(e) => handleFilterChange('repositoryId', e.target.value || undefined)}
                >
                  <option value="">All Repositories</option>
                  {repositories?.map((repo) => (
                    <option key={repo.id} value={repo.id}>
                      {repo.name}
                    </option>
                  ))}
                </Select>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Results */}
      {isLoading ? (
        <Card>
          <CardContent className="p-12 text-center">
            <RefreshCw className="mx-auto h-8 w-8 animate-spin text-gray-400" />
            <p className="mt-2 text-gray-500 dark:text-gray-400">Loading incidents...</p>
          </CardContent>
        </Card>
      ) : error ? (
        <Card>
          <CardContent className="p-12 text-center">
            <p className="text-red-600 dark:text-red-400">
              Failed to load incidents: {error instanceof Error ? error.message : 'Unknown error'}
            </p>
            <Button onClick={() => refetch()} className="mt-4">
              Try Again
            </Button>
          </CardContent>
        </Card>
      ) : !data || data.items.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <AlertTriangle className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900 dark:text-white">No incidents found</h3>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Get started by creating your first incident.
            </p>
            <div className="mt-6">
              <Button onClick={() => setShowCreateForm(true)}>
                <Plus className="mr-2 h-4 w-4" />
                New Incident
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : (
        <>
          {/* Incidents List */}
          <div className="space-y-4">
            {data.items.map((incident) => (
              <Card key={incident.id}>
                <CardContent className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center space-x-3 mb-3">
                        <StatusBadge status={incident.severity} />
                        <StatusBadge status={incident.status} />
                      </div>

                      <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
                        {incident.title}
                      </h3>

                      <p className="text-gray-600 dark:text-gray-400 mb-4 line-clamp-2">
                        {incident.description}
                      </p>

                      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-500 dark:text-gray-400 mb-4">
                        {incident.assignedTo && (
                          <div className="flex items-center">
                            <User className="h-4 w-4 mr-2" />
                            {incident.assignedTo}
                          </div>
                        )}
                        <div className="flex items-center">
                          <Clock className="h-4 w-4 mr-2" />
                          {formatDistanceToNow(new Date(incident.createdAt), { addSuffix: true })}
                        </div>
                        {incident.resolvedAt && (
                          <div className="flex items-center text-green-600 dark:text-green-400">
                            <Clock className="h-4 w-4 mr-2" />
                            Resolved {formatDistanceToNow(new Date(incident.resolvedAt), { addSuffix: true })}
                          </div>
                        )}
                      </div>

                      {incident.tags.length > 0 && (
                        <div className="flex flex-wrap gap-2">
                          {incident.tags.map((tag) => (
                            <Badge key={tag} variant="secondary" size="sm">
                              <Tag className="h-3 w-3 mr-1" />
                              {tag}
                            </Badge>
                          ))}
                        </div>
                      )}
                    </div>

                    <div className="flex flex-col space-y-2 ml-4">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => openEditForm(incident)}
                      >
                        <Edit className="mr-2 h-4 w-4" />
                        Edit
                      </Button>

                      {incident.status === 'open' && (
                        <Button
                          variant="secondary"
                          size="sm"
                          onClick={() => handleUpdateIncident(incident, { status: 'investigating' })}
                        >
                          Start Investigation
                        </Button>
                      )}

                      {incident.status === 'investigating' && (
                        <Button
                          variant="success"
                          size="sm"
                          onClick={() => handleUpdateIncident(incident, { status: 'resolved' })}
                        >
                          Mark Resolved
                        </Button>
                      )}

                      {incident.status === 'resolved' && (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleUpdateIncident(incident, { status: 'closed' })}
                        >
                          Close
                        </Button>
                      )}
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <Card>
              <CardContent className="p-4">
                <div className="flex items-center justify-between">
                  <div className="text-sm text-gray-500 dark:text-gray-400">
                    Showing {((filters.page || 1) - 1) * (filters.pageSize || 20) + 1} to{' '}
                    {Math.min((filters.page || 1) * (filters.pageSize || 20), data.totalCount)} of{' '}
                    {data.totalCount} results
                  </div>
                  <div className="flex items-center space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleFilterChange('page', (filters.page || 1) - 1)}
                      disabled={!filters.page || filters.page <= 1}
                    >
                      <ChevronLeft className="h-4 w-4" />
                    </Button>
                    <span className="text-sm text-gray-700 dark:text-gray-300">
                      Page {filters.page || 1} of {totalPages}
                    </span>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleFilterChange('page', (filters.page || 1) + 1)}
                      disabled={!filters.page || filters.page >= totalPages}
                    >
                      <ChevronRight className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}
        </>
      )}
    </div>
  );
};

export default IncidentsPage;