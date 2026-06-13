export interface User {
  id: string;
  email: string;
  name: string | null;
  emailVerified: boolean;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface EventDto {
  id: string;
  ticketmasterEventId: string;
  title: string;
  artist: string | null;
  venue: string | null;
  city: string | null;
  eventDate: string;
  ticketmasterUrl: string;
  imageUrl: string | null;
  genre: string | null;
}

export interface EventDetailDto extends EventDto {
  isWatched: boolean;
  watchId: string | null;
  watchStatus: string | null;
}

export interface EventSearchResponse {
  events: EventDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface WatchDto {
  id: string;
  eventId: string;
  eventTitle: string;
  artist: string | null;
  venue: string | null;
  eventDate: string;
  ticketmasterUrl: string;
  status: WatchStatus;
  createdAt: string;
  expiresAt: string;
  triggeredAt: string | null;
  apiPollingHistory: ApiPollingHistoryDto[];
}

export interface ApiPollingHistoryDto {
  id: string;
  ticketsAvailable: boolean;
  totalCount: number | null;
  httpStatusCode: number | null;
  durationMs: number;
  polledAt: string;
}

export type WatchStatus = 'Active' | 'Triggered' | 'Expired' | 'Cancelled';

export interface NotificationDto {
  id: string;
  watchId: string;
  type: string;
  subject: string;
  body: string | null;
  sent: boolean;
  sentAt: string | null;
  createdAt: string;
}

export interface NotificationListResponse {
  notifications: NotificationDto[];
  totalCount: number;
  unreadCount: number;
}

export interface PaymentDto {
  id: string;
  watchId: string;
  eventTitle: string;
  amount: number;
  currency: string;
  status: string;
  createdAt: string;
  completedAt: string | null;
}

export interface CheckoutResponse {
  sessionUrl: string;
  sessionId: string;
}
