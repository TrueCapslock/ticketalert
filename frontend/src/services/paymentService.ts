import api from './api';
import type { CheckoutResponse, PaymentDto } from '../types';

export const paymentService = {
  async createCheckout(eventId: string, successUrl: string, cancelUrl: string) {
    const { data } = await api.post<CheckoutResponse>('/payments/checkout', {
      eventId,
      successUrl,
      cancelUrl,
    });
    return data;
  },

  async getHistory() {
    const { data } = await api.get<PaymentDto[]>('/payments/history');
    return data;
  },
};
