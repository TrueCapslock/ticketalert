# TicketAlert - Systemarkitektur

## Løsningsoversikt

```
┌─────────────────────────────────────────────────────────────┐
│                     Bruker (nettleser)                       │
└──────────┬──────────────────────────────────┬───────────────┘
           │                                  │
           ▼                                  ▼
┌─────────────────────┐         ┌─────────────────────────────┐
│  React SPA (Vite)   │         │    Stripe Checkout          │
│  Tailwind, TanStack  │         │    (utenfor vår domene)     │
│  React Router       │         └─────────────────────────────┘
└──────────┬──────────┘
           │ HTTPS
           ▼
┌─────────────────────────────────────────────────────────────┐
│                 Azure App Service (API)                      │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  ASP.NET Core 9 Web API                               │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌────────┐  │  │
│  │  │ Auth     │ │ Events   │ │ Watches  │ │Payments│  │  │
│  │  │ Controller│ │Controller│ │Controller│ │Controll│  │  │
│  │  └──────────┘ └──────────┘ └──────────┘ └────────┘  │  │
│  │  ┌──────────────────────────────────────────────┐    │  │
│  │  │ Middleware: Exception, RateLimiting, JWT      │    │  │
│  │  └──────────────────────────────────────────────┘    │  │
│  │  ┌──────────────────────────────────────────────┐    │  │
│  │  │ Background Services                          │    │  │
│  │  │  • TicketPollingService (smart polling)       │    │  │
│  │  │  • WatchExpirationService                    │    │  │
│  │  └──────────────────────────────────────────────┘    │  │
│  └───────────────────────────────────────────────────────┘  │
└──────────────────────┬──────────────────────────────────────┘
                       │
          ┌────────────┼────────────────┐
          ▼            ▼                ▼
┌──────────────┐ ┌──────────┐ ┌──────────────┐
│  PostgreSQL  │ │Ticketmaster│ │   SMTP /     │
│  (Azure      │ │Discovery & │ │  SendGrid    │
│   Flexible)  │ │Inventory  │ │  (E-post)    │
└──────────────┘ └──────────┘ └──────────────┘
```

## Mappestruktur (Backend - Clean Architecture)

```
backend/
├── TicketAlert.sln
└── src/
    ├── TicketAlert.Domain/           # Innerste lag - ingen avhengigheter
    │   ├── Entities/                 # User, Event, Watch, Notification, Payment, ApiPollingRecord
    │   ├── Enums/                    # WatchStatus, NotificationType, PaymentStatus
    │   ├── ValueObjects/             # (fremtidig: pris, adresse)
    │   └── Interfaces/               # (fremtidig: repository interfaces)
    │
    ├── TicketAlert.Application/      # Use cases / CQRS handlers
    │   ├── Common/
    │   │   ├── Interfaces/           # IAppDbContext, IAuthService, ITicketmasterService m.fl.
    │   │   ├── Mappings/             # Mapping extensions
    │   │   └── Validators/           # FluentValidation
    │   └── Features/
    │       ├── Auth/                 # Register, Login, Refresh, ForgotPassword
    │       ├── Events/               # Search, GetDetails
    │       ├── Watches/              # Create, Get, Cancel
    │       ├── Notifications/        # Get, MarkRead
    │       └── Payments/             # Checkout, Webhook, History
    │
    ├── TicketAlert.Infrastructure/   # Ekstern integrasjon
    │   ├── Data/
    │   │   ├── AppDbContext.cs
    │   │   ├── Configurations/       # EF Core Fluent API per entity
    │   │   └── Migrations/           # EF Core migrations
    │   ├── Services/
    │   │   ├── Auth/                 # JwtAuthService
    │   │   ├── Notifications/        # EmailNotificationService
    │   │   ├── Payment/              # StripePaymentService
    │   │   └── Polling/              # TicketmasterService
    │   └── BackgroundJobs/
    │       ├── TicketPollingService.cs    # Smart polling av Ticketmaster
    │       └── WatchExpirationService.cs  # Automatisk utløp
    │
    └── TicketAlert.Api/             # Presentasjonslag
        ├── Controllers/             # Auth, Events, Watches, Notifications, Payments
        ├── DTOs/                    # Request/Response records
        ├── Middleware/              # ExceptionMiddleware, RateLimitingMiddleware
        ├── Extensions/              # DI-registrering, Auth config
        ├── Program.cs
        └── appsettings.json
```

## Mappestruktur (Frontend)

```
frontend/
├── index.html
├── package.json
├── vite.config.ts
├── tailwind.config.js
├── tsconfig.json
└── src/
    ├── main.tsx                    # Entry point
    ├── App.tsx                     # Router + providers
    ├── index.css                   # Tailwind imports
    ├── types/
    │   └── index.ts                # TypeScript interfaces
    ├── services/
    │   ├── api.ts                  # Axios instance + interceptors
    │   ├── authService.ts
    │   ├── eventService.ts
    │   ├── watchService.ts
    │   └── paymentService.ts
    ├── contexts/
    │   └── AuthContext.tsx
    ├── hooks/                      # Custom hooks (fremtidig)
    ├── components/
    │   ├── layout/
    │   │   └── Layout.tsx          # Header + footer
    │   ├── ui/                     # Gjenbrukbare UI-komponenter
    │   └── features/               # Feature-spesifikke komponenter
    └── pages/
        ├── LandingPage.tsx
        ├── LoginPage.tsx
        ├── RegisterPage.tsx
        ├── SearchPage.tsx
        ├── EventPage.tsx
        ├── DashboardPage.tsx
        └── AccountPage.tsx
```

## Deployment-arkitektur

### Azure (alternativ 1)
```
┌──────────────────────────────────────────────────┐
│  Azure App Service (Backend API)                  │
│  └── ASP.NET Core 9 + BackgroundServices         │
├──────────────────────────────────────────────────┤
│  Azure PostgreSQL Flexible Server                 │
│  └── Database                                     │
├──────────────────────────────────────────────────┤
│  Azure Static Web Apps / App Service (Frontend)   │
│  └── React SPA                                    │
└──────────────────────────────────────────────────┘
```

### Coolify (alternativ 2 — Docker Compose)
```
┌──────────────────────────────────────────────────┐
│  Coolify Server (VPS)                             │
│  ┌────────────────┐  ┌──────────────────────┐   │
│  │  Frontend       │  │  Backend API         │   │
│  │  nginx :80      │──│  ASP.NET 9 :8080     │   │
│  │  (statiske filer)│  │  + BackgroundServices│   │
│  └────────────────┘  └──────────┬───────────┘   │
│                                 │               │
│  ┌──────────────────────────────▼────────────┐  │
│  │  PostgreSQL :5432                         │  │
│  │  (volum: postgres_data)                   │  │
│  └───────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
```

## Developer Control Center (DCC)

`@hartvig/developer-control-center` er et prosjekt-lokalt CLI-verktøy for å kjøre kommandoer, bygg, tester og pipeliner via et TUI-grensesnitt.

```
npm install               # installerer DCC lokalt
npm run dcc               # åpner TUI med alle kommandoer
npx dcc                   # kjør uten install
```

Alle prosjektkommandoer er definert i `dcc.config.mjs` i rot, inkludert:

- **Backend**: dev, build, test, migrate, migration-add
- **Frontend**: dev, build, test, preview
- **Docker**: build, up, down, logs
- **Pipelines**: dev:all, build:all, test:all, docker:refresh
- **Profiler**: dev (utvikling), ci (CI-miljø)
