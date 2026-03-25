-- Initialize DevOps AI Agent database
-- This script runs when the PostgreSQL container starts for the first time

-- Create database if it doesn't exist (handled by POSTGRES_DB environment variable)

-- Create basic extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create schema for application
CREATE SCHEMA IF NOT EXISTS devops_ai_agent;

-- Set default search path
ALTER DATABASE devops_ai_agent SET search_path = devops_ai_agent, public;

-- Example table creation (you can add your actual schema here)
-- This is just a placeholder to show the structure

/*
CREATE TABLE IF NOT EXISTS repositories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    owner VARCHAR(255) NOT NULL,
    description TEXT,
    language VARCHAR(100),
    stars INTEGER DEFAULT 0,
    forks INTEGER DEFAULT 0,
    is_private BOOLEAN DEFAULT false,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(owner, name)
);

CREATE TABLE IF NOT EXISTS pull_requests (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    repository_id UUID REFERENCES repositories(id) ON DELETE CASCADE,
    number INTEGER NOT NULL,
    title VARCHAR(500) NOT NULL,
    state VARCHAR(20) DEFAULT 'open',
    author VARCHAR(255) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_repositories_owner_name ON repositories(owner, name);
CREATE INDEX IF NOT EXISTS idx_pull_requests_repository_id ON pull_requests(repository_id);
CREATE INDEX IF NOT EXISTS idx_pull_requests_state ON pull_requests(state);
*/

-- Grant permissions to the postgres user
GRANT ALL PRIVILEGES ON SCHEMA devops_ai_agent TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA devops_ai_agent TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA devops_ai_agent TO postgres;