import { useQuery } from '@tanstack/react-query';
import { paymentService } from '../services/paymentService';
import { useAuth } from '../contexts/AuthContext';
import { CreditCard, User, Mail, CheckCircle, XCircle } from 'lucide-react';

export default function AccountPage() {
  const { user } = useAuth();

  const { data: payments } = useQuery({
    queryKey: ['payments'],
    queryFn: () => paymentService.getHistory(),
  });

  const formatDate = (d: string) =>
    new Date(d).toLocaleDateString('nb-NO', {
      day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit',
    });

  return (
    <div className="max-w-3xl mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-8">Min konto</h1>

      <div className="bg-white border border-gray-200 rounded-xl p-6 mb-8">
        <h2 className="font-semibold text-lg mb-4">Profil</h2>
        <div className="space-y-3">
          <div className="flex items-center gap-2 text-gray-600">
            <User className="h-5 w-5" />
            <span>{user?.name || 'Ikke satt'}</span>
          </div>
          <div className="flex items-center gap-2 text-gray-600">
            <Mail className="h-5 w-5" />
            <span>{user?.email}</span>
            {user?.emailVerified ? (
              <CheckCircle className="h-4 w-4 text-green-500" />
            ) : (
              <XCircle className="h-4 w-4 text-yellow-500" />
            )}
          </div>
        </div>
      </div>

      <div className="bg-white border border-gray-200 rounded-xl p-6">
        <h2 className="font-semibold text-lg mb-4 flex items-center gap-2">
          <CreditCard className="h-5 w-5" />
          Betalingshistorikk
        </h2>

        {payments?.length === 0 && (
          <p className="text-gray-500 text-center py-8">Ingen betalinger enda.</p>
        )}

        <div className="space-y-3">
          {payments?.map((payment) => (
            <div
              key={payment.id}
              className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
            >
              <div>
                <p className="font-medium">{payment.eventTitle || 'Konsertovervåkning'}</p>
                <p className="text-sm text-gray-500">{formatDate(payment.createdAt)}</p>
              </div>
              <div className="text-right">
                <p className="font-semibold">
                  {payment.amount.toFixed(2)} {payment.currency.toUpperCase()}
                </p>
                <span className={`text-xs font-medium ${
                  payment.status === 'Completed' ? 'text-green-600' : 'text-gray-500'
                }`}>
                  {payment.status === 'Completed' ? 'Betalt' : payment.status}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
