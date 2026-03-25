import { useState } from 'react';
import { useEvents } from '../hooks/useEvents';
import { useTriggerAnalysis } from '../hooks/useAnalysis';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { StatusBadge } from '../components/ui/StatusBadge';
import { Badge } from '../components/ui/Badge';
import {
  Search,
  Filter,
  RefreshCw,
  Eye,
  Brain,
  Calendar,
  GitBranch,
  User,
  Clock,
  ExternalLink,
  ChevronLeft,
  ChevronRight,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import { formatDistanceToNow, format } from 'date-fns';
import { EventQueryParams, CICDEvent } from '../types/api';

const EventsPage = () => {
  const [filters, setFilters] = useState<EventQueryParams>({
    page: 1,
    pageSize: 20,
  });
  const [search, setSearch] = useState('');
  const [showFilters, setShowFilters] = useState(false);

  const { data, isLoading, error, refetch } = useEvents(filters);
  const triggerAnalysis = useTriggerAnalysis();

  const handleFilterChange = (key: keyof EventQueryParams, value: any) => {
    setFilters(prev => ({
      ...prev,
      [key]: value,
      page: 1, // Reset to first page when filtering
    }));
  };

  const handleSearch = () => {
    setFilters(prev => ({
      ...prev,
      search: search.trim() || undefined,
      page: 1,
    }));
  };

  const handleAnalyze = async (event: CICDEvent) => {
    if (event.status !== 'failure') return;

    try {
      await triggerAnalysis.mutateAsync({
        eventId: event.id,
        type: 'failure_analysis',
        input: event.failureReason || event.logs || 'Build failed',
        context: `Repository: ${event.repositoryName}, Branch: ${event.branch}, Commit: ${event.commit}`,
      });
    } catch (error) {
      console.error('Failed to trigger analysis:', error);
    }
  };

  const getEventTypeIcon = (type: string) => {
    switch (type) {
      case 'push':
        return <GitBranch className="h-4 w-4" />;
      case 'pull_request':
        return <ExternalLink className="h-4 w-4" />;
      case 'deployment':
        return <RefreshCw className="h-4 w-4" />;
      default:
        return <Calendar className="h-4 w-4" />;
    }
  };

  const totalPages = data ? Math.ceil(data.totalCount / (filters.pageSize || 20)) : 0;

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">CI/CD Events</h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            Monitor and analyze your CI/CD pipeline events
          </p>
        </div>
        <Button onClick={() => refetch()} variant="outline">
          <RefreshCw className="mr-2 h-4 w-4" />
          Refresh
        </Button>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardContent className="p-6">
          <div className="flex flex-col space-y-4 md:flex-row md:space-y-0 md:space-x-4">
            <div className="flex-1 flex space-x-2">
              <div className="relative flex-1">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  placeholder="Search events..."
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
                  <option value="success">Success</option>
                  <option value="failure">Failure</option>
                  <option value="pending">Pending</option>
                  <option value="cancelled">Cancelled</option>
                </Select>

                <Select
                  value={filters.eventType || ''}
                  onChange={(e) => handleFilterChange('eventType', e.target.value || undefined)}
                >
                  <option value="">All Types</option>
                  <option value="push">Push</option>
                  <option value="pull_request">Pull Request</option>
                  <option value="release">Release</option>
                  <option value="deployment">Deployment</option>
                  <option value="workflow_run">Workflow Run</option>
                </Select>

                <Input
                  placeholder="Branch..."
                  value={filters.branch || ''}
                  onChange={(e) => handleFilterChange('branch', e.target.value || undefined)}
                />

                <Input
                  placeholder="Author..."
                  value={filters.author || ''}
                  onChange={(e) => handleFilterChange('author', e.target.value || undefined)}
                />
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
            <p className="mt-2 text-gray-500 dark:text-gray-400">Loading events...</p>
          </CardContent>
        </Card>
      ) : error ? (
        <Card>
          <CardContent className="p-12 text-center">
            <p className="text-red-600 dark:text-red-400">
              Failed to load events: {error instanceof Error ? error.message : 'Unknown error'}
            </p>
            <Button onClick={() => refetch()} className="mt-4">
              Try Again
            </Button>
          </CardContent>
        </Card>
      ) : !data || data.items.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <Calendar className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900 dark:text-white">No events found</h3>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Try adjusting your search criteria or check back later.
            </p>
          </CardContent>
        </Card>
      ) : (
        <>
          {/* Events List */}
          <div className="space-y-4">
            {data.items.map((event) => (
              <Card key={event.id}>
                <CardContent className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center space-x-3 mb-3">
                        <StatusBadge status={event.status} />
                        <div className="flex items-center text-gray-500 dark:text-gray-400">
                          {getEventTypeIcon(event.eventType)}
                          <span className="ml-1 text-sm capitalize">{event.eventType}</span>
                        </div>
                        <Badge variant="secondary" size="sm">
                          {event.repositoryName}
                        </Badge>
                      </div>

                      <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
                        {event.message || `${event.eventType} event`}
                      </h3>

                      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-500 dark:text-gray-400">
                        <div className="flex items-center">
                          <GitBranch className="h-4 w-4 mr-2" />
                          {event.branch}
                        </div>
                        <div className="flex items-center">
                          <User className="h-4 w-4 mr-2" />
                          {event.author}
                        </div>
                        <div className="flex items-center">
                          <Clock className="h-4 w-4 mr-2" />
                          {formatDistanceToNow(new Date(event.createdAt), { addSuffix: true })}
                        </div>
                      </div>

                      {event.commit && (
                        <div className="mt-2">
                          <code className="text-xs bg-gray-100 dark:bg-gray-800 px-2 py-1 rounded">
                            {event.commit.substring(0, 8)}
                          </code>
                        </div>
                      )}

                      {event.failureReason && (
                        <div className="mt-3 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
                          <p className="text-sm text-red-800 dark:text-red-200">
                            <strong>Failure Reason:</strong> {event.failureReason}
                          </p>
                        </div>
                      )}

                      {event.duration && (
                        <div className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                          Duration: {Math.round(event.duration / 60)} minutes
                        </div>
                      )}
                    </div>

                    <div className="flex flex-col space-y-2 ml-4">
                      <Link to={`/events/${event.id}`}>
                        <Button variant="outline" size="sm">
                          <Eye className="mr-2 h-4 w-4" />
                          Details
                        </Button>
                      </Link>

                      {event.status === 'failure' && !event.aiAnalysisId && (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleAnalyze(event)}
                          loading={triggerAnalysis.isPending}
                        >
                          <Brain className="mr-2 h-4 w-4" />
                          Analyze
                        </Button>
                      )}

                      {event.aiAnalysisId && (
                        <Link to={`/analysis/${event.aiAnalysisId}`}>
                          <Button variant="secondary" size="sm">
                            <Brain className="mr-2 h-4 w-4" />
                            View Analysis
                          </Button>
                        </Link>
                      )}

                      {event.artifactsUrl && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => window.open(event.artifactsUrl, '_blank')}
                        >
                          <ExternalLink className="mr-2 h-4 w-4" />
                          Artifacts
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

export default EventsPage;