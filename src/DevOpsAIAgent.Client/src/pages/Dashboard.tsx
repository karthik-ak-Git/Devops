import { useDashboardSummary } from '../hooks/useDashboard';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Badge } from '../components/ui/Badge';
import { StatusBadge } from '../components/ui/StatusBadge';
import { Button } from '../components/ui/Button';
import {
  Activity,
  AlertTriangle,
  GitBranch,
  TrendingUp,
  Clock,
  CheckCircle,
  XCircle,
  RefreshCw,
  Eye,
  Plus,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import { formatDistanceToNow } from 'date-fns';

const Dashboard = () => {
  const { data: summary, isLoading, error, refetch } = useDashboardSummary();

  if (isLoading) {
    return (
      <div className="p-6">
        <div className="flex items-center justify-center h-64">
          <RefreshCw className="h-8 w-8 animate-spin text-gray-400" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-6">
        <div className="text-center">
          <XCircle className="mx-auto h-12 w-12 text-red-400" />
          <h3 className="mt-2 text-sm font-medium text-gray-900 dark:text-gray-100">
            Failed to load dashboard
          </h3>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {error instanceof Error ? error.message : 'Something went wrong'}
          </p>
          <div className="mt-6">
            <Button onClick={() => refetch()}>
              <RefreshCw className="mr-2 h-4 w-4" />
              Try again
            </Button>
          </div>
        </div>
      </div>
    );
  }

  const getSystemHealthColor = (status: string) => {
    switch (status) {
      case 'healthy':
        return 'text-green-600 dark:text-green-400';
      case 'degraded':
        return 'text-yellow-600 dark:text-yellow-400';
      case 'down':
        return 'text-red-600 dark:text-red-400';
      default:
        return 'text-gray-600 dark:text-gray-400';
    }
  };

  const getSystemHealthIcon = (status: string) => {
    switch (status) {
      case 'healthy':
        return <CheckCircle className="h-5 w-5" />;
      case 'degraded':
        return <AlertTriangle className="h-5 w-5" />;
      case 'down':
        return <XCircle className="h-5 w-5" />;
      default:
        return <Activity className="h-5 w-5" />;
    }
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            Real-time overview of your DevOps pipeline health and activity
          </p>
        </div>
        <Button onClick={() => refetch()} variant="outline">
          <RefreshCw className="mr-2 h-4 w-4" />
          Refresh
        </Button>
      </div>

      {/* Key Metrics */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="rounded-md bg-blue-500 p-3">
                <GitBranch className="h-6 w-6 text-white" />
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Repositories
                  </dt>
                  <dd className="text-2xl font-semibold text-gray-900 dark:text-white">
                    {summary?.totalRepositories || 0}
                  </dd>
                </dl>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="rounded-md bg-green-500 p-3">
                <Activity className="h-6 w-6 text-white" />
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Events Today
                  </dt>
                  <dd className="flex items-baseline">
                    <div className="text-2xl font-semibold text-gray-900 dark:text-white">
                      {summary?.eventsToday || 0}
                    </div>
                    {summary?.metrics?.deploymentsToday ? (
                      <div className="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                        {summary.metrics.deploymentsToday} deployments
                      </div>
                    ) : null}
                  </dd>
                </dl>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="rounded-md bg-yellow-500 p-3">
                <AlertTriangle className="h-6 w-6 text-white" />
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Active Incidents
                  </dt>
                  <dd className="flex items-baseline">
                    <div className="text-2xl font-semibold text-gray-900 dark:text-white">
                      {summary?.activeIncidents || 0}
                    </div>
                    {summary?.metrics?.criticalIncidents ? (
                      <div className="ml-2 flex items-baseline text-sm font-semibold text-red-600">
                        {summary.metrics.criticalIncidents} critical
                      </div>
                    ) : null}
                  </dd>
                </dl>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="rounded-md bg-purple-500 p-3">
                <TrendingUp className="h-6 w-6 text-white" />
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Success Rate
                  </dt>
                  <dd className="text-2xl font-semibold text-gray-900 dark:text-white">
                    {summary?.metrics?.successRate ?
                      `${(summary.metrics.successRate * 100).toFixed(1)}%` :
                      '0%'
                    }
                  </dd>
                </dl>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
        {/* System Health */}
        <Card>
          <CardHeader>
            <CardTitle>System Health</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {summary?.systemHealth && Object.entries(summary.systemHealth).map(([service, status]) => (
              <div key={service} className="flex items-center justify-between">
                <div className="flex items-center">
                  <div className={getSystemHealthColor(status)}>
                    {getSystemHealthIcon(status)}
                  </div>
                  <span className="ml-3 text-sm font-medium text-gray-900 dark:text-white capitalize">
                    {service.replace('Status', '').replace(/([A-Z])/g, ' $1').trim()}
                  </span>
                </div>
                <Badge variant={status === 'healthy' ? 'success' : status === 'degraded' ? 'warning' : 'error'}>
                  {status}
                </Badge>
              </div>
            ))}
          </CardContent>
        </Card>

        {/* Recent Events */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Recent Events</CardTitle>
            <Link to="/events">
              <Button variant="ghost" size="sm">
                <Eye className="mr-2 h-4 w-4" />
                View All
              </Button>
            </Link>
          </CardHeader>
          <CardContent>
            {summary?.recentEvents?.length ? (
              <div className="space-y-4">
                {summary.recentEvents.slice(0, 5).map((event) => (
                  <div key={event.id} className="flex items-center space-x-4">
                    <div className="flex-shrink-0">
                      <StatusBadge status={event.status} />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-900 dark:text-white truncate">
                        {event.repositoryName}
                      </p>
                      <p className="text-sm text-gray-500 dark:text-gray-400 truncate">
                        {event.branch} • {event.eventType}
                      </p>
                    </div>
                    <div className="text-xs text-gray-500 dark:text-gray-400">
                      {formatDistanceToNow(new Date(event.createdAt), { addSuffix: true })}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-500 dark:text-gray-400">No recent events</p>
            )}
          </CardContent>
        </Card>

        {/* Top Failed Repositories */}
        <Card>
          <CardHeader>
            <CardTitle>Top Failed Repositories</CardTitle>
          </CardHeader>
          <CardContent>
            {summary?.topFailedRepositories?.length ? (
              <div className="space-y-4">
                {summary.topFailedRepositories.slice(0, 5).map((repo, index) => (
                  <div key={repo.repositoryName} className="flex items-center justify-between">
                    <div className="flex items-center">
                      <div className="flex-shrink-0 w-6 h-6 bg-red-100 dark:bg-red-900/20 rounded-full flex items-center justify-center">
                        <span className="text-xs font-medium text-red-600 dark:text-red-400">
                          {index + 1}
                        </span>
                      </div>
                      <div className="ml-3">
                        <p className="text-sm font-medium text-gray-900 dark:text-white">
                          {repo.repositoryName}
                        </p>
                        <p className="text-xs text-gray-500 dark:text-gray-400">
                          {repo.failureCount} failures
                        </p>
                      </div>
                    </div>
                    <Badge variant="error" size="sm">
                      {(repo.failureRate * 100).toFixed(1)}%
                    </Badge>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-500 dark:text-gray-400">No failure data</p>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Recent Incidents */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Recent Incidents</CardTitle>
          <div className="flex space-x-2">
            <Link to="/incidents">
              <Button variant="ghost" size="sm">
                <Eye className="mr-2 h-4 w-4" />
                View All
              </Button>
            </Link>
            <Link to="/incidents/new">
              <Button size="sm">
                <Plus className="mr-2 h-4 w-4" />
                New Incident
              </Button>
            </Link>
          </div>
        </CardHeader>
        <CardContent>
          {summary?.recentIncidents?.length ? (
            <div className="space-y-4">
              {summary.recentIncidents.slice(0, 3).map((incident) => (
                <div key={incident.id} className="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center space-x-3">
                      <StatusBadge status={incident.severity} />
                      <StatusBadge status={incident.status} />
                    </div>
                    <div className="text-sm text-gray-500 dark:text-gray-400">
                      {formatDistanceToNow(new Date(incident.createdAt), { addSuffix: true })}
                    </div>
                  </div>
                  <h4 className="mt-2 text-sm font-medium text-gray-900 dark:text-white">
                    {incident.title}
                  </h4>
                  <p className="mt-1 text-sm text-gray-500 dark:text-gray-400 line-clamp-2">
                    {incident.description}
                  </p>
                  {incident.tags.length > 0 && (
                    <div className="mt-2 flex flex-wrap gap-1">
                      {incident.tags.slice(0, 3).map((tag) => (
                        <Badge key={tag} variant="secondary" size="sm">
                          {tag}
                        </Badge>
                      ))}
                    </div>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <p className="text-sm text-gray-500 dark:text-gray-400">No recent incidents</p>
          )}
        </CardContent>
      </Card>

      {/* Additional Metrics */}
      {summary?.averageBuildTime && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card>
            <CardContent className="p-6 text-center">
              <Clock className="mx-auto h-8 w-8 text-blue-500" />
              <div className="mt-2 text-2xl font-semibold text-gray-900 dark:text-white">
                {Math.round(summary.averageBuildTime / 60)}m
              </div>
              <div className="text-sm text-gray-500 dark:text-gray-400">Average Build Time</div>
            </CardContent>
          </Card>

          {summary.metrics?.averageResolutionTime && (
            <Card>
              <CardContent className="p-6 text-center">
                <AlertTriangle className="mx-auto h-8 w-8 text-yellow-500" />
                <div className="mt-2 text-2xl font-semibold text-gray-900 dark:text-white">
                  {Math.round(summary.metrics.averageResolutionTime / 3600)}h
                </div>
                <div className="text-sm text-gray-500 dark:text-gray-400">Avg Resolution Time</div>
              </CardContent>
            </Card>
          )}

          <Card>
            <CardContent className="p-6 text-center">
              <TrendingUp className="mx-auto h-8 w-8 text-green-500" />
              <div className="mt-2 text-2xl font-semibold text-gray-900 dark:text-white">
                {((1 - (summary.failureRate || 0)) * 100).toFixed(1)}%
              </div>
              <div className="text-sm text-gray-500 dark:text-gray-400">Overall Success Rate</div>
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  );
};

export default Dashboard;