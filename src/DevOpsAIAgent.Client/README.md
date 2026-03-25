# DevOps AI Agent - React Frontend

A modern, real-time React TypeScript dashboard for monitoring and managing DevOps CI/CD pipelines with AI-powered analysis and insights.

## Features

🔥 **Real-time Dashboard** - Live updates via SignalR for events, incidents, and analysis
📊 **CI/CD Event Monitoring** - Track pipeline events, failures, and build metrics
🚨 **Incident Management** - Create, track, and resolve DevOps incidents
🤖 **AI Analysis** - AI-powered failure analysis and recommendations
📁 **Repository Management** - Track multiple repositories with webhook integration
🌙 **Dark/Light Theme** - Automatic theme switching with system preference
📱 **Responsive Design** - Mobile-friendly interface for on-the-go monitoring
🔔 **Toast Notifications** - Real-time notifications for important events

## Tech Stack

- **React 19** - Latest React with hooks and concurrent features
- **TypeScript** - Full type safety throughout the application
- **Vite** - Lightning-fast build tool and dev server
- **TanStack Query** - Powerful data fetching and caching
- **React Router** - Client-side routing
- **Tailwind CSS** - Utility-first CSS framework
- **SignalR** - Real-time web functionality
- **React Hook Form + Zod** - Form handling with validation
- **Lucide React** - Beautiful icon system
- **Date-fns** - Modern JavaScript date utility library

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Running DevOps AI Agent API backend

### Installation

1. **Clone the repository**
   ```bash
   cd src/DevOpsAIAgent.Client
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Environment Configuration**
   ```bash
   cp .env.example .env
   ```
   Update the `.env` file with your API configuration:
   ```env
   VITE_API_BASE_URL=http://localhost:5000/api
   VITE_ENV=development
   VITE_ENABLE_DEBUG_LOGS=true
   ```

4. **Start the development server**
   ```bash
   npm run dev
   ```

5. **Open your browser**
   Navigate to `http://localhost:5173`

## API Integration

The frontend consumes the following API endpoints:

- `POST /api/webhooks/github` - GitHub webhook receiver
- `GET /api/webhooks/health` - Health check
- `GET /api/cicd-events` - List CI/CD events (paginated)
- `GET /api/cicd-events/{id}` - Get specific event
- `GET /api/incidents` - List incidents
- `POST /api/incidents` - Create incident
- `PUT /api/incidents/{id}` - Update incident
- `GET /api/repositories` - List tracked repositories
- `POST /api/repositories` - Add repository
- `DELETE /api/repositories/{id}` - Remove repository
- `GET /api/dashboard/summary` - Dashboard summary data
- `GET /api/ai-analysis` - AI analysis history
- `POST /api/ai-analysis/analyze` - Trigger AI analysis

**SignalR Hub:** `/hubs/dashboard` for real-time updates

## Project Structure

```
src/
├── components/           # Reusable UI components
│   ├── ui/              # Base UI components (Button, Card, etc.)
│   └── Layout.tsx       # Main layout component
├── hooks/               # Custom React hooks
│   ├── useEvents.ts     # CI/CD events management
│   ├── useIncidents.ts  # Incidents management
│   ├── useRepositories.ts # Repository management
│   ├── useAnalysis.ts   # AI analysis management
│   ├── useSignalR.ts    # Real-time connections
│   ├── useDashboard.ts  # Dashboard data
│   └── useTheme.tsx     # Theme management
├── pages/               # Page components
│   ├── Dashboard.tsx    # Main dashboard
│   ├── EventsPage.tsx   # CI/CD events listing
│   ├── IncidentsPage.tsx # Incident management
│   ├── RepositoriesPage.tsx # Repository management
│   └── AnalysisPage.tsx # AI analysis viewer
├── services/            # API services
│   ├── api.ts          # Main API client
│   └── signalr.ts      # SignalR service
├── types/              # TypeScript type definitions
│   └── api.ts          # API response types
├── utils/              # Utility functions
│   └── cn.ts           # ClassName utility
├── App.tsx             # Main app component
└── main.tsx            # Application entry point
```

## Key Components

### Dashboard
Real-time overview with:
- System health indicators
- Key metrics (repositories, events, incidents)
- Recent activity feed
- Top failed repositories
- Success rate analytics

### CI/CD Events
- Filterable event listing
- Event details and logs
- AI analysis integration
- Status tracking (success/failure/pending)
- Real-time updates

### Incident Management
- Create and track incidents
- Severity levels (low/medium/high/critical)
- Status workflow (open → investigating → resolved → closed)
- Repository association
- Tag-based organization

### Repository Management
- Add/remove tracked repositories
- Webhook status monitoring
- Build statistics and success rates
- Repository health indicators

### AI Analysis
- Trigger analysis for failures
- View AI-generated insights and recommendations
- Multiple AI provider support
- Analysis history and search

## Real-time Features

The application uses SignalR for real-time updates:

- **Live Dashboard**: Metrics update automatically
- **Event Notifications**: New CI/CD events appear instantly
- **Incident Alerts**: Critical incidents trigger notifications
- **Analysis Completion**: AI analysis results are pushed when ready
- **Connection Status**: Visual indicators show real-time connection health

## Development Scripts

```bash
# Development
npm run dev              # Start dev server
npm run build           # Build for production
npm run preview         # Preview production build

# Code Quality
npm run lint            # Run ESLint
npm run lint:fix        # Fix linting issues
npm run format          # Format with Prettier
npm run format:check    # Check formatting
npm run type-check      # Run TypeScript compiler

# Utilities
npm run clean           # Clean build artifacts
npm run deps:check      # Check for vulnerabilities
npm run deps:update     # Update dependencies
```

## Theme Support

The application supports automatic dark/light theme switching:

- **System Preference**: Automatically detects system theme
- **Manual Toggle**: Users can override with theme switcher
- **Persistent**: Theme preference saved in localStorage
- **Real-time**: Theme changes apply immediately without refresh

## Performance Optimizations

- **Code Splitting**: Automatic route-based splitting
- **Query Caching**: Intelligent data caching with TanStack Query
- **Optimistic Updates**: Immediate UI updates with cache invalidation
- **Lazy Loading**: Components loaded on demand
- **Memoization**: React.memo and useMemo for expensive operations

## Error Handling

- **Error Boundaries**: Graceful error handling with fallback UI
- **Retry Logic**: Automatic retry for failed requests
- **Toast Notifications**: User-friendly error messages
- **Loading States**: Proper loading indicators throughout
- **Offline Support**: Graceful degradation when offline

## Accessibility

- **Keyboard Navigation**: Full keyboard accessibility
- **Screen Reader**: ARIA labels and semantic HTML
- **Focus Management**: Proper focus handling
- **Color Contrast**: WCAG compliant color schemes
- **Responsive Design**: Mobile-friendly interface

## Contributing

1. Follow the existing code style and patterns
2. Use TypeScript strict mode
3. Add proper error handling
4. Include loading states for async operations
5. Follow the component architecture patterns
6. Test thoroughly with different data states

## Browser Support

- **Chrome**: Latest 2 versions
- **Firefox**: Latest 2 versions
- **Safari**: Latest 2 versions
- **Edge**: Latest 2 versions

## License

This project is part of the DevOps AI Agent system. See the root LICENSE file for details.