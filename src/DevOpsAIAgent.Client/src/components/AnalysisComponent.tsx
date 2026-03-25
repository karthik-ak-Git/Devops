import React, { useState, useEffect } from 'react';
import { llmService, AnalysisRequest, AnalysisResponse, ProviderStatus } from '../services/api';

interface AnalysisComponentProps {
  className?: string;
}

export const AnalysisComponent: React.FC<AnalysisComponentProps> = ({ className = '' }) => {
  const [prompt, setPrompt] = useState('');
  const [context, setContext] = useState('');
  const [selectedProvider, setSelectedProvider] = useState('');
  const [providers, setProviders] = useState<ProviderStatus[]>([]);
  const [defaultProvider, setDefaultProvider] = useState('');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<AnalysisResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Load providers on component mount
  useEffect(() => {
    loadProviders();
  }, []);

  const loadProviders = async () => {
    try {
      const response = await llmService.getProviders();
      if (response.success) {
        setProviders(response.providers);
        setDefaultProvider(response.defaultProvider);
        setSelectedProvider(response.defaultProvider);
      }
    } catch (error) {
      console.error('Failed to load providers:', error);
    }
  };

  const handleAnalyze = async (useFallback = false) => {
    if (!prompt.trim()) {
      setError('Please enter a prompt');
      return;
    }

    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const request: AnalysisRequest = {
        prompt: prompt.trim(),
        context: context.trim(),
        provider: selectedProvider || undefined,
      };

      const response = useFallback
        ? await llmService.generateAnalysisWithFallback(request)
        : await llmService.generateAnalysis(request);

      setResult(response);
    } catch (error: any) {
      setError(error.response?.data?.message || error.message || 'Analysis failed');
    } finally {
      setLoading(false);
    }
  };

  const handleClear = () => {
    setPrompt('');
    setContext('');
    setResult(null);
    setError(null);
  };

  return (
    <div className={`max-w-4xl mx-auto p-6 ${className}`}>
      <div className="bg-white rounded-lg shadow-lg p-6">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">DevOps AI Analysis</h2>

        {/* Provider Selection */}
        <div className="mb-6">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            LLM Provider
          </label>
          <select
            value={selectedProvider}
            onChange={(e) => setSelectedProvider(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            {providers.map((provider) => (
              <option key={provider.name} value={provider.name}>
                {provider.name} {provider.name === defaultProvider ? '(Default)' : ''}{' '}
                {provider.isHealthy ? '✅' : '❌'}
              </option>
            ))}
          </select>
          <p className="text-xs text-gray-500 mt-1">
            Select an LLM provider for analysis. Green checkmark indicates the provider is healthy.
          </p>
        </div>

        {/* Prompt Input */}
        <div className="mb-6">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Analysis Prompt *
          </label>
          <textarea
            value={prompt}
            onChange={(e) => setPrompt(e.target.value)}
            placeholder="Enter your DevOps analysis request (e.g., 'Analyze this deployment for security risks', 'Review CI/CD pipeline performance')"
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            rows={3}
          />
        </div>

        {/* Context Input */}
        <div className="mb-6">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Context (Optional)
          </label>
          <textarea
            value={context}
            onChange={(e) => setContext(e.target.value)}
            placeholder="Provide additional context for analysis (logs, configuration files, deployment info, etc.)"
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            rows={4}
          />
        </div>

        {/* Action Buttons */}
        <div className="flex gap-3 mb-6">
          <button
            onClick={() => handleAnalyze(false)}
            disabled={loading || !prompt.trim()}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            {loading ? 'Analyzing...' : 'Analyze'}
          </button>

          <button
            onClick={() => handleAnalyze(true)}
            disabled={loading || !prompt.trim()}
            className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
          >
            {loading ? 'Analyzing...' : 'Analyze with Fallback'}
          </button>

          <button
            onClick={handleClear}
            disabled={loading}
            className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
          >
            Clear
          </button>

          <button
            onClick={loadProviders}
            disabled={loading}
            className="px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700 disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2"
          >
            Refresh Providers
          </button>
        </div>

        {/* Loading Indicator */}
        {loading && (
          <div className="mb-6">
            <div className="flex items-center justify-center p-4 bg-blue-50 rounded-md">
              <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mr-3"></div>
              <span className="text-blue-700">Generating analysis...</span>
            </div>
          </div>
        )}

        {/* Error Display */}
        {error && (
          <div className="mb-6">
            <div className="p-4 bg-red-50 border border-red-200 rounded-md">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-red-500 text-lg">❌</span>
                </div>
                <div className="ml-3">
                  <h3 className="text-sm font-medium text-red-800">Analysis Failed</h3>
                  <p className="text-sm text-red-700 mt-1">{error}</p>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Results Display */}
        {result && result.success && result.analysis && (
          <div className="bg-gray-50 border border-gray-200 rounded-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">Analysis Result</h3>
              <div className="flex items-center text-sm text-gray-500">
                <span className="mr-2">Provider:</span>
                <span className="font-medium text-gray-900">{result.analysis.provider}</span>
                <span className="ml-4 mr-2">Model:</span>
                <span className="font-medium text-gray-900">{result.analysis.model}</span>
                <span className="ml-4 mr-2">Confidence:</span>
                <span className="font-medium text-gray-900">
                  {(result.analysis.confidence * 100).toFixed(0)}%
                </span>
              </div>
            </div>

            <div className="prose max-w-none">
              <div className="bg-white p-4 rounded border">
                <pre className="whitespace-pre-wrap text-sm text-gray-700 font-sans leading-relaxed">
                  {result.analysis.output}
                </pre>
              </div>
            </div>

            <div className="mt-4 text-xs text-gray-500">
              Generated at: {new Date(result.analysis.createdAt).toLocaleString()}
            </div>
          </div>
        )}

        {/* Provider Status */}
        <div className="mt-8 pt-6 border-t border-gray-200">
          <h4 className="text-sm font-medium text-gray-900 mb-3">Provider Status</h4>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            {providers.map((provider) => (
              <div
                key={provider.name}
                className={`p-3 rounded-md border ${provider.isHealthy
                    ? 'bg-green-50 border-green-200'
                    : 'bg-red-50 border-red-200'
                  }`}
              >
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium text-gray-900">
                    {provider.name}
                  </span>
                  <span className={`text-sm ${provider.isHealthy ? 'text-green-700' : 'text-red-700'
                    }`}>
                    {provider.isHealthy ? '✅ Healthy' : '❌ Unavailable'}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default AnalysisComponent;