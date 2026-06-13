import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import { eventService } from '../services/eventService';
import { watchService } from '../services/watchService';
import { paymentService } from '../services/paymentService';
import { useAuth } from '../contexts/AuthContext';
import { Calendar, MapPin, Music, ExternalLink, Bell, Loader2 } from 'lucide-react';

export default function EventPage() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const navigate = useNavigate();

  const { data: event, isLoading } = useQuery({
    queryKey: ['event', id],
    queryFn: () => eventService.getById(id!),
    enabled: !!id,
  });

  const checkoutMutation = useMutation({
    mutationFn: () =>
      paymentService.createCheckout(
        id!,
        `${window.location.origin}/dashboard?success=true`,
        `${window.location.origin}/events/${id}?canceled=true`
      ),
    onSuccess: (data) => {
      window.location.href = data.sessionUrl;
    },
  });

  if (isLoading) {
    return (
      <div className="flex justify-center py-24">
        <div className="animate-spin h-8 w-8 border-4 border-primary-500 border-t-transparent rounded-full" />
      </div>
    );
  }

  if (!event) {
    return <div className="text-center py-24 text-gray-500">Konsert ikke funnet.</div>;
  }

  const formatDate = (d: string) =>
    new Date(d).toLocaleDateString('nb-NO', {
      day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit',
    });

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        {event.imageUrl && (
          <img src={event.imageUrl} alt={event.title} className="w-full h-64 sm:h-96 object-cover" />
        )}
        <div className="p-6 sm:p-8">
          <h1 className="text-3xl font-bold mb-4">{event.title}</h1>

          <div className="space-y-2 mb-6">
            {event.artist && (
              <p className="flex items-center gap-2 text-gray-600">
                <Music className="h-5 w-5" />
                {event.artist}
              </p>
            )}
            <p className="flex items-center gap-2 text-gray-600">
              <Calendar className="h-5 w-5" />
              {formatDate(event.eventDate)}
            </p>
            {(event.venue || event.city) && (
              <p className="flex items-center gap-2 text-gray-600">
                <MapPin className="h-5 w-5" />
                {[event.venue, event.city].filter(Boolean).join(', ')}
              </p>
            )}
          </div>

          <a
            href={event.ticketmasterUrl}
            target="_blank"
            rel="noopener noreferrer"
            className="inline-flex items-center gap-2 text-primary-600 hover:underline mb-8"
          >
            <ExternalLink className="h-4 w-4" />
            Åpne på Ticketmaster
          </a>

          <div className="border-t pt-6">
            {!user ? (
              <div className="text-center py-4">
                <p className="text-gray-600 mb-4">Logg inn for å opprette overvåkning</p>
                <button
                  onClick={() => navigate('/login')}
                  className="bg-primary-600 text-white px-6 py-2 rounded-lg font-semibold hover:bg-primary-700"
                >
                  Logg inn
                </button>
              </div>
            ) : event.isWatched ? (
              <div className="bg-green-50 border border-green-200 rounded-lg p-4 text-center">
                <p className="text-green-700 font-semibold">
                  Du overvåker allerede denne konserten
                </p>
                <p className="text-green-600 text-sm mt-1">Status: {event.watchStatus}</p>
              </div>
            ) : (
              <div className="text-center">
                <p className="text-lg font-semibold mb-2">19 kr per overvåkning</p>
                <p className="text-sm text-gray-600 mb-4">
                  Overvåkningen varer frem til konsertdato. Du får varsel på e-post når billetter blir tilgjengelige.
                </p>
                <button
                  onClick={() => checkoutMutation.mutate()}
                  disabled={checkoutMutation.isPending}
                  className="inline-flex items-center gap-2 bg-primary-600 text-white px-8 py-3 rounded-lg font-semibold hover:bg-primary-700 disabled:opacity-50"
                >
                  {checkoutMutation.isPending ? (
                    <Loader2 className="h-5 w-5 animate-spin" />
                  ) : (
                    <Bell className="h-5 w-5" />
                  )}
                  {checkoutMutation.isPending ? 'Venter...' : 'Overvåk denne konserten'}
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
