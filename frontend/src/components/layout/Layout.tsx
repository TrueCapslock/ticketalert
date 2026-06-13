import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Bell, Search, LayoutDashboard, User, LogOut } from 'lucide-react';
import { version } from '../../../package.json';

export default function Layout({ children }: { children: React.ReactNode }) {
  const { user, logout } = useAuth();

  return (
    <div className="min-h-screen flex flex-col">
      <header className="bg-white border-b border-gray-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <Link to="/" className="text-xl font-bold text-primary-600">
              TicketAlert
            </Link>

            <nav className="flex items-center gap-4">
              <Link to="/search" className="flex items-center gap-1 text-sm text-gray-600 hover:text-primary-600">
                <Search className="h-4 w-4" />
                <span className="hidden sm:inline">Søk</span>
              </Link>

              {user ? (
                <>
                  <Link to="/dashboard" className="flex items-center gap-1 text-sm text-gray-600 hover:text-primary-600">
                    <LayoutDashboard className="h-4 w-4" />
                    <span className="hidden sm:inline">Dashboard</span>
                  </Link>
                  <Link to="/dashboard" className="relative text-gray-600 hover:text-primary-600">
                    <Bell className="h-5 w-5" />
                  </Link>
                  <Link to="/account" className="flex items-center gap-1 text-sm text-gray-600 hover:text-primary-600">
                    <User className="h-4 w-4" />
                    <span className="hidden sm:inline">{user.name || user.email}</span>
                  </Link>
                  <button onClick={logout} className="text-gray-600 hover:text-red-600">
                    <LogOut className="h-4 w-4" />
                  </button>
                </>
              ) : (
                <>
                  <Link to="/login" className="text-sm text-gray-600 hover:text-primary-600">Logg inn</Link>
                  <Link to="/register" className="text-sm bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700">
                    Registrer
                  </Link>
                </>
              )}
            </nav>
          </div>
        </div>
      </header>

      <main className="flex-1">{children}</main>

      <footer className="bg-white border-t border-gray-200 py-8">
        <div className="max-w-7xl mx-auto px-4 text-center text-sm text-gray-500">
          <p>TicketAlert &mdash; Overvåkning av utsolgte konserter. Vi selger ikke billetter.</p>
          <p className="mt-1 text-xs text-gray-400">v{version}</p>
        </div>
      </footer>
    </div>
  );
}
