import api from './api';
import type { EventSearchResponse, EventDetailDto } from '../types';

export const eventService = {
  async search(params: {
    keyword?: string;
    artist?: string;
    city?: string;
    page?: number;
    pageSize?: number;
  }) {
    const { data } = await api.get<EventSearchResponse>('/events/search', { params });
    return data;
  },

  async getById(id: string) {
    const { data } = await api.get<EventDetailDto>(`/events/${id}`);
    return data;
  },

  async getByTicketmasterId(ticketmasterEventId: string) {
    const { data } = await api.get<EventDetailDto>(`/events/ticketmaster/${ticketmasterEventId}`);
    return data;
  },
};
