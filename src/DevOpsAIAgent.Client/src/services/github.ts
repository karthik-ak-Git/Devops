import api from './api';

export interface Repository {
  owner: string;
  name: string;
  description: string;
  language: string;
  stars: number;
  forks: number;
  isPrivate: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface PullRequest {
  id: number;
  number: number;
  title: string;
  state: string;
  author: string;
  createdAt: string;
  updatedAt: string;
}

export const githubService = {
  async getRepository(owner: string, repo: string): Promise<Repository> {
    const response = await api.get(`/github/repository/${owner}/${repo}`);
    return response.data.data;
  },

  async getPullRequests(owner: string, repo: string, state = 'open'): Promise<PullRequest[]> {
    const response = await api.get(`/github/repository/${owner}/${repo}/pull-requests`, {
      params: { state },
    });
    return response.data.data;
  },

  async handleWebhook(payload: object): Promise<{ processed: boolean }> {
    const response = await api.post('/github/webhook', payload);
    return response.data.data;
  },
};

export default githubService;