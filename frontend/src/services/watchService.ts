import api from './api';
import type { WatchDto } from '../types';

export const watchService = {
  async create(data: {
    ticketmasterEventId: string;
    title: string;
    artist?: string;
    venue?: string;
    city?: string;
    eventDate: string;
    ticketmasterUrl: string;
    imageUrl?: string;
  }) {
    const { data: watch } = await api.post<WatchDto>('/watches', data);
    return watch;
  },

  async getAll(status?: string) {
    const { data } = await api.get<WatchDto[]>('/watches', {
      params: status ? { status } : undefined,
    });
    return data;
  },

  async cancel(id: string) {
    await api.delete(`/watches/${id}`);
  },
};
