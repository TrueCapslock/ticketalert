import api from './api';
import type { AuthResponse } from '../types';

export const authService = {
  async register(email: string, password: string, name?: string) {
    const { data } = await api.post<AuthResponse>('/auth/register', { email, password, name });
    return data;
  },

  async login(email: string, password: string) {
    const { data } = await api.post<AuthResponse>('/auth/login', { email, password });
    return data;
  },

  async refresh(refreshToken: string) {
    const { data } = await api.post<AuthResponse>('/auth/refresh', { refreshToken });
    return data;
  },

  async forgotPassword(email: string) {
    await api.post('/auth/forgot-password', { email });
  },

  async getMe() {
    const { data } = await api.get('/auth/me');
    return data;
  },
};
