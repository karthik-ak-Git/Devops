import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { ThemeProvider } from './hooks/useTheme';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import Repository from './pages/Repository';
import PullRequests from './pages/PullRequests';
import AnalysisPage from './pages/AnalysisPage';
import CodeReviewPage from './pages/CodeReviewPage';
import EventsPage from './pages/EventsPage';
import IncidentsPage from './pages/IncidentsPage';
import RepositoriesPage from './pages/RepositoriesPage';
import './App.css';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider>
        <Router>
          <Layout>
            <Routes>
              <Route path="/" element={<Dashboard />} />
              <Route path="/events" element={<EventsPage />} />
              <Route path="/incidents" element={<IncidentsPage />} />
              <Route path="/repositories" element={<RepositoriesPage />} />
              <Route path="/analysis" element={<AnalysisPage />} />
              <Route path="/code-review" element={<CodeReviewPage />} />
              <Route path="/repository/:owner/:repo" element={<Repository />} />
              <Route path="/repository/:owner/:repo/pulls" element={<PullRequests />} />
            </Routes>
          </Layout>
        </Router>
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 4000,
            style: {
              background: '#363636',
              color: '#fff',
            },
            success: {
              duration: 3000,
              iconTheme: {
                primary: '#10b981',
                secondary: '#fff',
              },
            },
            error: {
              duration: 6000,
              iconTheme: {
                primary: '#ef4444',
                secondary: '#fff',
              },
            },
          }}
        />
      </ThemeProvider>
    </QueryClientProvider>
  );
}

export default App;
