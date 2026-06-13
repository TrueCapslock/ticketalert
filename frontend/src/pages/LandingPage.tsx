import { Link } from 'react-router-dom';
import { Bell, Search, Ticket, Zap } from 'lucide-react';

export default function LandingPage() {
  return (
    <div>
      <section className="bg-gradient-to-br from-primary-600 to-primary-800 text-white">
        <div className="max-w-7xl mx-auto px-4 py-24 sm:py-32">
          <div className="max-w-3xl">
            <h1 className="text-4xl sm:text-5xl font-bold leading-tight mb-6">
              Gå aldri glipp av billetter igjen
            </h1>
            <p className="text-lg sm:text-xl text-primary-100 mb-8">
              TicketAlert overvåker utsolgte konserter på Ticketmaster Norge og
              varsler deg på e-post så snart billetter blir tilgjengelige.
              Ingen abonnement &mdash; betal per overvåkning.
            </p>
            <div className="flex flex-wrap gap-4">
              <Link
                to="/search"
                className="inline-flex items-center gap-2 bg-white text-primary-700 px-6 py-3 rounded-lg font-semibold hover:bg-primary-50"
              >
                <Search className="h-5 w-5" />
                Søk etter konserter
              </Link>
              <Link
                to="/register"
                className="inline-flex items-center gap-2 bg-primary-500 text-white px-6 py-3 rounded-lg font-semibold hover:bg-primary-400"
              >
                Kom i gang
              </Link>
            </div>
          </div>
        </div>
      </section>

      <section className="max-w-7xl mx-auto px-4 py-20">
        <h2 className="text-2xl font-bold text-center mb-12">Slik fungerer det</h2>
        <div className="grid md:grid-cols-3 gap-8">
          <div className="text-center p-6">
            <div className="bg-primary-100 w-16 h-16 rounded-2xl flex items-center justify-center mx-auto mb-4">
              <Search className="h-8 w-8 text-primary-600" />
            </div>
            <h3 className="font-semibold text-lg mb-2">1. Finn konserten</h3>
            <p className="text-gray-600">Søk etter artister og konserter på Ticketmaster Norge.</p>
          </div>
          <div className="text-center p-6">
            <div className="bg-primary-100 w-16 h-16 rounded-2xl flex items-center justify-center mx-auto mb-4">
              <Bell className="h-8 w-8 text-primary-600" />
            </div>
            <h3 className="font-semibold text-lg mb-2">2. Opprett overvåkning</h3>
            <p className="text-gray-600">Velg konserten du vil overvåke. Betal 19 kr per overvåkning.</p>
          </div>
          <div className="text-center p-6">
            <div className="bg-primary-100 w-16 h-16 rounded-2xl flex items-center justify-center mx-auto mb-4">
              <Zap className="h-8 w-8 text-primary-600" />
            </div>
            <h3 className="font-semibold text-lg mb-2">3. Få varsel</h3>
            <p className="text-gray-600">Vi overvåker døgnet rundt og varsler deg på e-post når billetter dukker opp.</p>
          </div>
        </div>
      </section>

      <section className="bg-gray-100 py-20">
        <div className="max-w-7xl mx-auto px-4">
          <div className="max-w-2xl mx-auto text-center">
            <h2 className="text-2xl font-bold mb-6">19 kr per overvåkning</h2>
            <p className="text-gray-600 mb-8">
              Ingen skjulte kostnader. Ingen abonnement. Overvåkningen varer frem til konsertdato.
              Du betaler kun når du oppretter en ny overvåkning.
            </p>
            <div className="flex items-center justify-center gap-2 text-primary-600">
              <Ticket className="h-5 w-5" />
              <span className="font-semibold">Du blir sendt til Ticketmaster for kjøp</span>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
