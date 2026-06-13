import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { eventService } from '../services/eventService';
import { Search, MapPin, Calendar, Music } from 'lucide-react';

export default function SearchPage() {
  const [keyword, setKeyword] = useState('');
  const [search, setSearch] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['events', search],
    queryFn: () => eventService.search({ keyword: search, pageSize: 20 }),
    enabled: search.length > 0,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSearch(keyword);
  };

  const formatDate = (d: string) =>
    new Date(d).toLocaleDateString('nb-NO', {
      day: 'numeric', month: 'long', year: 'numeric',
    });

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Søk etter konserter</h1>

      <form onSubmit={handleSubmit} className="mb-8">
        <div className="flex gap-2">
          <input
            type="text"
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
            placeholder="Søk etter artist, konsert, by..."
            className="flex-1 border border-gray-300 rounded-lg px-4 py-3 text-lg"
          />
          <button
            type="submit"
            className="bg-primary-600 text-white px-6 py-3 rounded-lg font-semibold hover:bg-primary-700 flex items-center gap-2"
          >
            <Search className="h-5 w-5" />
            Søk
          </button>
        </div>
      </form>

      {isLoading && (
        <div className="flex justify-center py-12">
          <div className="animate-spin h-8 w-8 border-4 border-primary-500 border-t-transparent rounded-full" />
        </div>
      )}

      {data && data.events.length === 0 && (
        <p className="text-center text-gray-500 py-12">Ingen konserter funnet. Prøv et annet søk.</p>
      )}

      <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
        {data?.events.map((event) => (
          <Link
            key={event.id}
            to={`/events/${event.id}`}
            className="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-lg transition-shadow"
          >
            {event.imageUrl && (
              <img
                src={event.imageUrl}
                alt={event.title}
                className="w-full h-48 object-cover"
                loading="lazy"
              />
            )}
            <div className="p-4">
              <h3 className="font-semibold text-lg mb-1">{event.title}</h3>
              {event.artist && (
                <p className="flex items-center gap-1 text-sm text-gray-600 mb-1">
                  <Music className="h-4 w-4" />
                  {event.artist}
                </p>
              )}
              <p className="flex items-center gap-1 text-sm text-gray-600 mb-1">
                <Calendar className="h-4 w-4" />
                {formatDate(event.eventDate)}
              </p>
              {(event.venue || event.city) && (
                <p className="flex items-center gap-1 text-sm text-gray-600">
                  <MapPin className="h-4 w-4" />
                  {[event.venue, event.city].filter(Boolean).join(', ')}
                </p>
              )}
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
