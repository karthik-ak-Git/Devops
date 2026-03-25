import React from 'react';
import CodeReviewComponent from '../components/CodeReviewComponent';

const CodeReviewPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">AI Code Review</h1>
          <p className="mt-2 text-gray-600">
            Get comprehensive code reviews powered by advanced AI models to improve code quality,
            security, and performance.
          </p>
        </div>

        <CodeReviewComponent />

        <div className="mt-12 bg-white rounded-lg shadow p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">About AI Code Review</h2>
          <div className="prose text-gray-700">
            <p>
              Our AI Code Review system analyzes your code changes using state-of-the-art language models
              to identify potential issues, suggest improvements, and ensure best practices. The AI examines
              code for security vulnerabilities, performance bottlenecks, maintainability issues, and
              adherence to coding standards.
            </p>

            <h3 className="text-lg font-medium mt-6 mb-3">What the AI Reviews:</h3>
            <ul className="space-y-2">
              <li>
                <strong>Security Issues:</strong> SQL injection, XSS vulnerabilities, insecure data handling,
                and authentication flaws.
              </li>
              <li>
                <strong>Performance Problems:</strong> Inefficient algorithms, memory leaks, unnecessary
                database queries, and blocking operations.
              </li>
              <li>
                <strong>Code Quality:</strong> Code smells, duplicated logic, complex functions,
                and maintainability concerns.
              </li>
              <li>
                <strong>Best Practices:</strong> Coding standards, naming conventions, error handling,
                and documentation quality.
              </li>
            </ul>

            <h3 className="text-lg font-medium mt-6 mb-3">Review Features:</h3>
            <ul className="space-y-2">
              <li>• Multi-language support (JavaScript, Python, Java, C#, Go, and more)</li>
              <li>• Security-focused analysis with vulnerability detection</li>
              <li>• Performance optimization suggestions</li>
              <li>• Best practices recommendations</li>
              <li>• Code maintainability assessment</li>
              <li>• Specific line-by-line feedback</li>
            </ul>

            <h3 className="text-lg font-medium mt-6 mb-3">How to Use:</h3>
            <ol className="space-y-2 list-decimal list-inside">
              <li>Select your preferred AI provider (Gemini, OpenRouter, or Ollama)</li>
              <li>Paste your code changes or new functions into the text area</li>
              <li>Click "Review Code" to get comprehensive analysis and suggestions</li>
              <li>Review the feedback and implement recommended improvements</li>
            </ol>

            <div className="mt-6 p-4 bg-blue-50 border-l-4 border-blue-400 rounded">
              <p className="text-sm text-blue-700">
                <strong>Pro Tip:</strong> Include context about what your code is supposed to do
                for more targeted and relevant feedback from the AI.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CodeReviewPage;