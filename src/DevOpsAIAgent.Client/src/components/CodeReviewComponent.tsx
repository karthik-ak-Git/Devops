import React, { useState, useEffect } from 'react';
import { llmService, CodeReviewRequest, ProviderStatus } from '../services/api';

interface CodeReviewComponentProps {
  className?: string;
}

export const CodeReviewComponent: React.FC<CodeReviewComponentProps> = ({ className = '' }) => {
  const [codeChanges, setCodeChanges] = useState('');
  const [selectedProvider, setSelectedProvider] = useState('');
  const [providers, setProviders] = useState<ProviderStatus[]>([]);
  const [defaultProvider, setDefaultProvider] = useState('');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<any>(null);
  const [error, setError] = useState<string | null>(null);

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

  const handleReview = async () => {
    if (!codeChanges.trim()) {
      setError('Please enter code changes to review');
      return;
    }

    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const request: CodeReviewRequest = {
        codeChanges: codeChanges.trim(),
        provider: selectedProvider || undefined,
      };

      const response = await llmService.generateCodeReview(request);
      setResult(response);
    } catch (error: any) {
      setError(error.response?.data?.message || error.message || 'Code review failed');
    } finally {
      setLoading(false);
    }
  };

  const handleClear = () => {
    setCodeChanges('');
    setResult(null);
    setError(null);
  };

  const sampleCode = `// Sample code changes for testing
function validateUser(user) {
  if (!user) return false;
  if (!user.email) return false;
  if (!user.password) return false;

  // TODO: Add proper password validation
  if (user.password.length < 8) return false;

  return true;
}

// New function with potential issues
function processPayment(amount, cardNumber) {
  // Direct database query - potential SQL injection
  const query = "SELECT * FROM cards WHERE number = '" + cardNumber + "'";

  // No input validation
  const result = database.query(query);

  if (result) {
    // Log sensitive data - security risk
    console.log("Processing payment for card:", cardNumber);
    return chargeCard(amount, cardNumber);
  }

  return false;
}`;

  return (
    <div className={`max-w-4xl mx-auto p-6 ${className}`}>
      <div className="bg-white rounded-lg shadow-lg p-6">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">AI Code Review</h2>

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
        </div>

        {/* Code Input */}
        <div className="mb-6">
          <div className="flex items-center justify-between mb-2">
            <label className="block text-sm font-medium text-gray-700">
              Code Changes *
            </label>
            <button
              onClick={() => setCodeChanges(sampleCode)}
              className="text-sm text-blue-600 hover:text-blue-700 underline"
            >
              Load Sample Code
            </button>
          </div>
          <textarea
            value={codeChanges}
            onChange={(e) => setCodeChanges(e.target.value)}
            placeholder="Paste your code changes here for AI-powered review..."
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 font-mono text-sm"
            rows={12}
          />
          <p className="text-xs text-gray-500 mt-1">
            Enter code changes, new functions, or code snippets you'd like reviewed for security, performance, and best practices.
          </p>
        </div>

        {/* Action Buttons */}
        <div className="flex gap-3 mb-6">
          <button
            onClick={handleReview}
            disabled={loading || !codeChanges.trim()}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            {loading ? 'Reviewing...' : 'Review Code'}
          </button>

          <button
            onClick={handleClear}
            disabled={loading}
            className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
          >
            Clear
          </button>
        </div>

        {/* Loading Indicator */}
        {loading && (
          <div className="mb-6">
            <div className="flex items-center justify-center p-4 bg-blue-50 rounded-md">
              <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mr-3"></div>
              <span className="text-blue-700">Analyzing code...</span>
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
                  <h3 className="text-sm font-medium text-red-800">Code Review Failed</h3>
                  <p className="text-sm text-red-700 mt-1">{error}</p>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Results Display */}
        {result && result.success && (
          <div className="bg-gray-50 border border-gray-200 rounded-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">Code Review Results</h3>
              <div className="flex items-center text-sm text-gray-500">
                <span className="mr-2">Provider:</span>
                <span className="font-medium text-gray-900">{result.provider}</span>
              </div>
            </div>

            <div className="prose max-w-none">
              <div className="bg-white p-4 rounded border">
                <pre className="whitespace-pre-wrap text-sm text-gray-700 font-sans leading-relaxed">
                  {result.review}
                </pre>
              </div>
            </div>
          </div>
        )}

        {/* Help Text */}
        <div className="mt-8 p-4 bg-blue-50 border border-blue-200 rounded-md">
          <h4 className="text-sm font-medium text-blue-900 mb-2">💡 Code Review Tips</h4>
          <ul className="text-sm text-blue-800 space-y-1">
            <li>• Include context about what the code is supposed to do</li>
            <li>• Paste both old and new code if showing changes</li>
            <li>• The AI will analyze for security vulnerabilities, performance issues, and best practices</li>
            <li>• Try the sample code to see how the AI review works</li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default CodeReviewComponent;