# Prioritert roadmap for MVP

## Fase 1 — Fundament (uke 1-2)
- [x] Prosjektstruktur (Clean Architecture)
- [x] Domain entities og enums
- [x] EF Core DbContext + konfigurasjoner
- [ ] Sett opp PostgreSQL (lokal utvikling)
- [ ] Kjøre første migrering
- [ ] Registrering/innlogging (JWT)
- [ ] appsettings.json med alle secrets

## Fase 2 — Ticketmaster-integrasjon (uke 2-3)
- [ ] Implementer Discovery API (søk)
- [ ] Implementer Inventory Status API (polling)
- [ ] Lag migrering for å lagre events lokalt
- [ ] Test mot Ticketmaster API med nøkler
- [ ] Håndter rate limiting fra Ticketmaster

## Fase 3 — Overvåkning og betaling (uke 3-4)
- [ ] CRUD for Watches
- [ ] Stripe Checkout-integrasjon
- [ ] Stripe Webhook for payment completed
- [ ] Koble betaling → watch creation
- [ ] Valider webhook-signatur

## Fase 4 — Bakgrunnsjobber (uke 4-5)
- [ ] TicketPollingService med smart polling
- [ ] WatchExpirationService
- [ ] Send e-post via SendGrid/SMTP
- [ ] Loggføring i ApiPollingHistory
- [ ] Unngå duplikate varsler

## Fase 5 — Frontend (uke 5-7)
- [ ] Auth-sider (login/register)
- [ ] Søkeside med Ticketmaster-integrasjon
- [ ] Event-detaljside med "Overvåk"-knapp
- [ ] Dashboard med oversikt
- [ ] Betalingsflyt via Stripe Checkout
- [ ] Min konto-side

## Fase 6 — Sikkerhet og kvalitet (uke 7-8)
- [ ] Rate limiting (middleware)
- [ ] Input validering
- [ ] Refresh tokens
- [ ] Exception middleware
- [ ] Logging (Serilog)
- [ ] Unit tests for handlers
- [ ] Integration tests for API

## Fase 7 — Deployment (uke 8-9)
- [ ] Azure PostgreSQL Flexible Server (alternativ 1)
- [ ] Azure App Service deployment
- [x] Docker Compose for Coolify/VPS (alternativ 2)
- [x] Dockerfile for backend + frontend
- [x] Nginx reverse proxy for frontend
- [x] .env.example med alle miljøvariabler
- [ ] GitHub Actions CI/CD
- [ ] Domene + SSL
- [ ] Miljøvariabler i Azure/Coolify

## Fase 8 — Etter MVP
- [ ] E-postverifisering
- [ ] Glemt passord
- [ ] Flere varslingskanaler (SMS, push)
- [ ] Admin-panel
- [ ] Analytics
- [ ] Abonnementsmodell
- [ ] Mobilapp (React Native)

## Risk matrix

| Risiko | Sannsynlighet | Konsekvens | Tiltak |
|--------|:-----------:|:---------:|--------|
| Ticketmaster API-endringer | Lav | Høy | Versjonert API, abstraksjonslag |
| Ticketmaster rate limit | Medium | Medium | Smart polling, caching |
| Stripe webhook feil | Lav | Høy | Idempotency, logging |
| E-post forsinkelser | Medium | Lav | Flere forsøk, logging |
| Skaleringsproblemer | Medium | Medium | Skalerbare hosted services |
