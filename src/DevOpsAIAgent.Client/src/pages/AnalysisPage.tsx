import { useState } from 'react';
import { useAnalyses, useTriggerAnalysis } from '../hooks/useAnalysis';
import { useEvents } from '../hooks/useEvents';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Textarea } from '../components/ui/Textarea';
import { Select } from '../components/ui/Select';
import { Badge } from '../components/ui/Badge';
import {
  Brain,
  Search,
  Filter,
  RefreshCw,
  Plus,
  Eye,
  Clock,
  Zap,
  Target,
  TrendingUp,
  AlertTriangle,
  CheckCircle,
  X,
  ChevronLeft,
  ChevronRight,
  Lightbulb,
  Activity,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import { formatDistanceToNow } from 'date-fns';
import { AnalysisQueryParams, TriggerAnalysisRequest } from '../types/api';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const triggerAnalysisSchema = z.object({
  type: z.enum(['failure_analysis', 'performance_review', 'security_scan', 'code_quality']),
  input: z.string().min(1, 'Input is required'),
  context: z.string().optional(),
  provider: z.string().optional(),
});

type TriggerAnalysisFormData = z.infer<typeof triggerAnalysisSchema>;

const AnalysisPage = () => {
  const [filters, setFilters] = useState<AnalysisQueryParams>({
    page: 1,
    pageSize: 20,
  });
  const [search, setSearch] = useState('');
  const [showFilters, setShowFilters] = useState(false);
  const [showTriggerForm, setShowTriggerForm] = useState(false);
  const [selectedEventId, setSelectedEventId] = useState<string>('');

  const { data, isLoading, error, refetch } = useAnalyses(filters);
  const { data: recentEvents } = useEvents({ pageSize: 10, status: 'failure' });
  const triggerAnalysis = useTriggerAnalysis();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    watch,
  } = useForm<TriggerAnalysisFormData>({
    resolver: zodResolver(triggerAnalysisSchema),
    defaultValues: {
      type: 'failure_analysis',
    },
  });

  const handleFilterChange = (key: keyof AnalysisQueryParams, value: any) => {
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

  const handleTriggerAnalysis = async (data: TriggerAnalysisFormData) => {
    const request: TriggerAnalysisRequest = {
      ...data,
      eventId: selectedEventId || undefined,
    };

    try {
      await triggerAnalysis.mutateAsync(request);
      setShowTriggerForm(false);
      reset();
      setSelectedEventId('');
    } catch (error) {
      console.error('Failed to trigger analysis:', error);
    }
  };

  const closeTriggerForm = () => {
    setShowTriggerForm(false);
    reset();
    setSelectedEventId('');
  };

  const getConfidenceColor = (confidence: number) => {
    if (confidence >= 0.8) return 'text-green-600 dark:text-green-400';
    if (confidence >= 0.6) return 'text-yellow-600 dark:text-yellow-400';
    return 'text-red-600 dark:text-red-400';
  };

  const getAnalysisTypeIcon = (type: string) => {
    switch (type) {
      case 'failure_analysis':
        return <AlertTriangle className="h-4 w-4" />;
      case 'performance_review':
        return <TrendingUp className="h-4 w-4" />;
      case 'security_scan':
        return <Target className="h-4 w-4" />;
      case 'code_quality':
        return <CheckCircle className="h-4 w-4" />;
      default:
        return <Brain className="h-4 w-4" />;
    }
  };

  const getAnalysisTypeColor = (type: string) => {
    switch (type) {
      case 'failure_analysis':
        return 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300';
      case 'performance_review':
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300';
      case 'security_scan':
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300';
      case 'code_quality':
        return 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300';
    }
  };

  const totalPages = data ? Math.ceil(data.totalCount / (filters.pageSize || 20)) : 0;

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">AI Analysis</h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            View AI-generated insights and trigger new analysis
          </p>
        </div>
        <div className="flex space-x-2">
          <Button onClick={() => refetch()} variant="outline">
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => setShowTriggerForm(true)}>
            <Plus className="mr-2 h-4 w-4" />
            New Analysis
          </Button>
        </div>
      </div>

      {/* Trigger Analysis Form */}
      {showTriggerForm && (
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Trigger New Analysis</CardTitle>
            <Button variant="ghost" size="sm" onClick={closeTriggerForm}>
              <X className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(handleTriggerAnalysis)} className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Analysis Type *
                  </label>
                  <Select {...register('type')} error={!!errors.type}>
                    <option value="failure_analysis">Failure Analysis</option>
                    <option value="performance_review">Performance Review</option>
                    <option value="security_scan">Security Scan</option>
                    <option value="code_quality">Code Quality</option>
                  </Select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Related Event (Optional)
                  </label>
                  <Select
                    value={selectedEventId}
                    onChange={(e) => setSelectedEventId(e.target.value)}
                  >
                    <option value="">Select an event...</option>
                    {recentEvents?.items.map((event) => (
                      <option key={event.id} value={event.id}>
                        {event.repositoryName} - {event.branch} ({event.eventType})
                      </option>
                    ))}
                  </Select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    AI Provider (Optional)
                  </label>
                  <Select {...register('provider')}>
                    <option value="">Auto-select</option>
                    <option value="openai">OpenAI</option>
                    <option value="anthropic">Anthropic</option>
                    <option value="gemini">Google Gemini</option>
                  </Select>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Input Data *
                </label>
                <Textarea
                  {...register('input')}
                  error={!!errors.input}
                  placeholder="Paste error logs, code snippets, or describe the issue to analyze..."
                  rows={6}
                />
                {errors.input && (
                  <p className="mt-1 text-sm text-red-600 dark:text-red-400">{errors.input.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Additional Context (Optional)
                </label>
                <Textarea
                  {...register('context')}
                  placeholder="Provide any additional context, environment details, or specific areas to focus on..."
                  rows={3}
                />
              </div>

              <div className="flex justify-end space-x-2 pt-4">
                <Button type="button" variant="outline" onClick={closeTriggerForm}>
                  Cancel
                </Button>
                <Button type="submit" loading={triggerAnalysis.isPending}>
                  <Zap className="mr-2 h-4 w-4" />
                  Trigger Analysis
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
                  placeholder="Search analysis..."
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
                  value={filters.type || ''}
                  onChange={(e) => handleFilterChange('type', e.target.value || undefined)}
                >
                  <option value="">All Types</option>
                  <option value="failure_analysis">Failure Analysis</option>
                  <option value="performance_review">Performance Review</option>
                  <option value="security_scan">Security Scan</option>
                  <option value="code_quality">Code Quality</option>
                </Select>

                <Select
                  value={filters.provider || ''}
                  onChange={(e) => handleFilterChange('provider', e.target.value || undefined)}
                >
                  <option value="">All Providers</option>
                  <option value="openai">OpenAI</option>
                  <option value="anthropic">Anthropic</option>
                  <option value="gemini">Google Gemini</option>
                </Select>

                <Input
                  type="number"
                  placeholder="Min confidence (0-100)"
                  value={filters.minConfidence ? (filters.minConfidence * 100) : ''}
                  onChange={(e) => {
                    const value = e.target.value ? parseFloat(e.target.value) / 100 : undefined;
                    handleFilterChange('minConfidence', value);
                  }}
                  min="0"
                  max="100"
                />

                <Input
                  placeholder="Event ID..."
                  value={filters.eventId || ''}
                  onChange={(e) => handleFilterChange('eventId', e.target.value || undefined)}
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
            <p className="mt-2 text-gray-500 dark:text-gray-400">Loading analysis...</p>
          </CardContent>
        </Card>
      ) : error ? (
        <Card>
          <CardContent className="p-12 text-center">
            <p className="text-red-600 dark:text-red-400">
              Failed to load analysis: {error instanceof Error ? error.message : 'Unknown error'}
            </p>
            <Button onClick={() => refetch()} className="mt-4">
              Try Again
            </Button>
          </CardContent>
        </Card>
      ) : !data || data.items.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <Brain className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900 dark:text-white">No analysis found</h3>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Try adjusting your search criteria or trigger a new analysis.
            </p>
            <div className="mt-6">
              <Button onClick={() => setShowTriggerForm(true)}>
                <Plus className="mr-2 h-4 w-4" />
                New Analysis
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : (
        <>
          {/* Analysis List */}
          <div className="space-y-6">
            {data.items.map((analysis) => (
              <Card key={analysis.id}>
                <CardContent className="p-6">
                  <div className="flex items-start justify-between mb-4">
                    <div className="flex items-center space-x-3">
                      <div className={`p-2 rounded-lg ${getAnalysisTypeColor(analysis.type)}`}>
                        {getAnalysisTypeIcon(analysis.type)}
                      </div>
                      <div>
                        <h3 className="text-lg font-medium text-gray-900 dark:text-white capitalize">
                          {analysis.type.replace('_', ' ')}
                        </h3>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                          {formatDistanceToNow(new Date(analysis.createdAt), { addSuffix: true })}
                        </p>
                      </div>
                    </div>

                    <div className="flex items-center space-x-3">
                      <div className="text-right">
                        <div className={`text-lg font-semibold ${getConfidenceColor(analysis.confidence)}`}>
                          {(analysis.confidence * 100).toFixed(0)}%
                        </div>
                        <p className="text-xs text-gray-500 dark:text-gray-400">Confidence</p>
                      </div>
                      <Badge variant="secondary" size="sm">
                        {analysis.provider}
                      </Badge>
                    </div>
                  </div>

                  {/* Summary */}
                  {analysis.summary && (
                    <div className="mb-4 p-4 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg">
                      <h4 className="font-medium text-blue-900 dark:text-blue-100 mb-2 flex items-center">
                        <Lightbulb className="h-4 w-4 mr-2" />
                        Summary
                      </h4>
                      <p className="text-sm text-blue-800 dark:text-blue-200">
                        {analysis.summary}
                      </p>
                    </div>
                  )}

                  {/* Analysis Output */}
                  <div className="mb-4">
                    <h4 className="font-medium text-gray-900 dark:text-white mb-2">Analysis</h4>
                    <div className="prose prose-sm max-w-none dark:prose-invert">
                      <div className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">
                        {analysis.output}
                      </div>
                    </div>
                  </div>

                  {/* Recommendations */}
                  {analysis.recommendations && analysis.recommendations.length > 0 && (
                    <div className="mb-4 p-4 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg">
                      <h4 className="font-medium text-green-900 dark:text-green-100 mb-3 flex items-center">
                        <CheckCircle className="h-4 w-4 mr-2" />
                        Recommendations
                      </h4>
                      <ul className="space-y-2">
                        {analysis.recommendations.map((recommendation, index) => (
                          <li key={index} className="flex items-start">
                            <div className="flex-shrink-0 w-5 h-5 bg-green-500 text-white rounded-full flex items-center justify-center text-xs font-bold mt-0.5 mr-3">
                              {index + 1}
                            </div>
                            <p className="text-sm text-green-800 dark:text-green-200">
                              {recommendation}
                            </p>
                          </li>
                        ))}
                      </ul>
                    </div>
                  )}

                  {/* Metadata */}
                  <div className="flex items-center justify-between pt-4 border-t border-gray-200 dark:border-gray-700 text-sm text-gray-500 dark:text-gray-400">
                    <div className="flex items-center space-x-4">
                      <div className="flex items-center">
                        <Clock className="h-4 w-4 mr-1" />
                        {(analysis.processingTime / 1000).toFixed(1)}s
                      </div>
                      <div className="flex items-center">
                        <Activity className="h-4 w-4 mr-1" />
                        {analysis.tokens.toLocaleString()} tokens
                      </div>
                      <div>Model: {analysis.model}</div>
                    </div>

                    <div className="flex space-x-2">
                      {analysis.eventId && (
                        <Link to={`/events/${analysis.eventId}`}>
                          <Button variant="ghost" size="sm">
                            <Eye className="mr-2 h-4 w-4" />
                            View Event
                          </Button>
                        </Link>
                      )}
                      <Link to={`/analysis/${analysis.id}`}>
                        <Button variant="outline" size="sm">
                          <Eye className="mr-2 h-4 w-4" />
                          View Details
                        </Button>
                      </Link>
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

export default AnalysisPage;