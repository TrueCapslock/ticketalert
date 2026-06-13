import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { watchService } from '../services/watchService';
import { Bell, Calendar, ExternalLink, XCircle, Eye, EyeOff } from 'lucide-react';
import type { WatchDto } from '../types';

const statusColors: Record<string, string> = {
  Active: 'bg-blue-100 text-blue-700',
  Triggered: 'bg-green-100 text-green-700',
  Expired: 'bg-gray-100 text-gray-600',
  Cancelled: 'bg-red-100 text-red-600',
};

export default function DashboardPage() {
  const [filter, setFilter] = useState<string>('Active');
  const queryClient = useQueryClient();

  const { data: watches, isLoading } = useQuery({
    queryKey: ['watches', filter],
    queryFn: () => watchService.getAll(filter),
  });

  const cancelMutation = useMutation({
    mutationFn: (id: string) => watchService.cancel(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['watches'] }),
  });

  const formatDate = (d: string) =>
    new Date(d).toLocaleDateString('nb-NO', {
      day: 'numeric', month: 'short', year: 'numeric',
    });

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-2xl font-bold">Mine overvåkninger</h1>
        <Link
          to="/search"
          className="bg-primary-600 text-white px-4 py-2 rounded-lg text-sm font-semibold hover:bg-primary-700"
        >
          + Ny overvåkning
        </Link>
      </div>

      <div className="flex gap-2 mb-6">
        {['Active', 'Triggered', 'Expired', 'Cancelled'].map((s) => (
          <button
            key={s}
            onClick={() => setFilter(s)}
            className={`px-4 py-2 rounded-lg text-sm font-medium ${
              filter === s
                ? 'bg-primary-600 text-white'
                : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
            }`}
          >
            {s === 'Active' && <Eye className="h-4 w-4 inline mr-1" />}
            {s === 'Triggered' && <Bell className="h-4 w-4 inline mr-1" />}
            {s === 'Expired' && <EyeOff className="h-4 w-4 inline mr-1" />}
            {s === 'Cancelled' && <XCircle className="h-4 w-4 inline mr-1" />}
            {s === 'Active' && 'Aktive'}
            {s === 'Triggered' && 'Varslet'}
            {s === 'Expired' && 'Utløpt'}
            {s === 'Cancelled' && 'Avbrutt'}
          </button>
        ))}
      </div>

      {isLoading && (
        <div className="flex justify-center py-12">
          <div className="animate-spin h-8 w-8 border-4 border-primary-500 border-t-transparent rounded-full" />
        </div>
      )}

      {watches?.length === 0 && (
        <div className="text-center py-16 text-gray-500">
          <Bell className="h-12 w-12 mx-auto mb-4 opacity-50" />
          <p className="text-lg">Ingen overvåkninger</p>
          <Link to="/search" className="text-primary-600 hover:underline mt-2 inline-block">
            Søk etter konserter å overvåke
          </Link>
        </div>
      )}

      <div className="space-y-4">
        {watches?.map((watch: WatchDto) => (
          <div
            key={watch.id}
            className="bg-white border border-gray-200 rounded-xl p-4 sm:p-6 flex flex-col sm:flex-row items-start justify-between gap-4"
          >
            <div className="flex-1">
              <h3 className="font-semibold text-lg">{watch.eventTitle}</h3>
              <p className="text-sm text-gray-600 mt-1">
                {watch.artist && `${watch.artist} — `}
                {watch.venue && `${watch.venue}, `}
                {formatDate(watch.eventDate)}
              </p>
              <div className="flex items-center gap-3 mt-2">
                <span className={`text-xs font-medium px-2 py-1 rounded-full ${statusColors[watch.status]}`}>
                  {watch.status === 'Active' && 'Aktiv'}
                  {watch.status === 'Triggered' && 'Varslet'}
                  {watch.status === 'Expired' && 'Utløpt'}
                  {watch.status === 'Cancelled' && 'Avbrutt'}
                </span>
                <span className="text-xs text-gray-500">
                  Opprettet: {formatDate(watch.createdAt)}
                </span>
              </div>
            </div>
            <div className="flex items-center gap-2 shrink-0">
              <a
                href={watch.ticketmasterUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="text-primary-600 hover:text-primary-700 p-2"
                title="Åpne på Ticketmaster"
              >
                <ExternalLink className="h-5 w-5" />
              </a>
              {watch.status === 'Active' && (
                <button
                  onClick={() => cancelMutation.mutate(watch.id)}
                  className="text-red-600 hover:text-red-700 p-2"
                  title="Avbryt overvåkning"
                >
                  <XCircle className="h-5 w-5" />
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
