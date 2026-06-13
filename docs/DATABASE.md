# Database-design

## ER-diagram

```
┌─────────────┐       ┌──────────────┐       ┌─────────────────┐
│    Users    │       │   Events     │       │ ApiPollingHistory│
├─────────────┤       ├──────────────┤       ├─────────────────┤
│ Id (PK)     │──┐    │ Id (PK)      │──┐    │ Id (PK)         │
│ Email (UQ)  │  │    │ Ticketmaster │  │    │ EventId (FK)    │
│ PasswordHash│  │    │  EventId(UQ) │  │    │ TicketsAvailable │
│ Name        │  │    │ Title        │  │    │ TotalCount      │
│ EmailVerifd │  │    │ Artist       │  │    │ RawResponse     │
│ RefreshToken│  │    │ Venue        │  │    │ HttpStatusCode  │
│ RefrshTknExp│  │    │ City         │  │    │ DurationMs      │
│ CreatedAt   │  │    │ EventDate    │  │    │ PolledAt(IDX)   │
│ UpdatedAt   │  │    │ Ticketmaster │  │    └─────────────────┘
└─────────────┘  │    │  Url         │  │           ▲
                 │    │ ImageUrl     │  │           │
                 │    │ Genre        │  │           │
                 │    │ CreatedAt    │  │           │
                 │    │ UpdatedAt    │  └───────────┘
                 │    └──────┬───────┘
                 │           │
                 │           │ 1
                 │           ▼
                 │    ┌──────────────┐       ┌──────────────────┐
                 │    │   Watches    │       │  Notifications   │
                 ├───►│              │       ├──────────────────┤
                 │    │ Id (PK)      │──┐    │ Id (PK)          │
                 │    │ UserId (FK)  │  │    │ WatchId (FK)     │
                 │    │ EventId (FK) │  │    │ UserId (FK)      │
                 │    │ Status       │  │    │ Type             │
                 │    │ CreatedAt    │  │    │ Recipient        │
                 │    │ ExpiresAt    │  │    │ Subject          │
                 │    │ TriggeredAt  │  │    │ Body             │
                 │    └──────┬───────┘  │    │ Sent             │
                 │           │          │    │ SentAt           │
                 │           │ 1        │    │ CreatedAt        │
                 │           ▼          │    └──────────────────┘
                 │    ┌──────────────┐  │           ▲
                 │    │   Payments   │  │           │
                 └───►│              │  │           │
                      │ Id (PK)      │  │           │
                      │ UserId (FK)  │  │           │
                      │ WatchId(FK,UQ)│─┘           │
                      │ StripeSessId │              │
                      │ StripePayInt │              │
                      │ Amount       │              │
                      │ Currency     │              │
                      │ Status       │              │
                      │ CreatedAt    │              │
                      │ CompletedAt  │              │
                      └──────────────┘              │
                                                    │
               Alle NOT NULL med mindre annet er angitt.
               (FK) = Foreign Key, (PK) = Primary Key,
               (UQ) = Unique Index, (IDX) = Index
```

## Indekser

| Tabell | Index | Kolonne(r) | Type |
|--------|-------|------------|------|
| Users | IX_Users_Email | Email | Unik |
| Users | IX_Users_RefreshToken | RefreshToken | Vanlig |
| Events | IX_Events_TicketmasterEventId | TicketmasterEventId | Unik |
| Events | IX_Events_EventDate | EventDate | Vanlig |
| Events | IX_Events_Artist_EventDate | Artist, EventDate | Sammensatt |
| Watches | IX_Watches_UserId | UserId | Vanlig |
| Watches | IX_Watches_EventId | EventId | Vanlig |
| Watches | IX_Watches_Status | Status | Vanlig |
| Watches | IX_Watches_ExpiresAt | ExpiresAt | Vanlig |
| Watches | IX_Watches_Status_ExpiresAt | Status, ExpiresAt | Sammensatt (viktig for polling) |
| Payments | IX_Payments_UserId | UserId | Vanlig |
| Payments | IX_Payments_WatchId | WatchId | Unik |
| Payments | IX_Payments_StripeSessionId | StripeSessionId | Unik |
| ApiPollingHistory | IX_ApiPolling_EventId | EventId | Vanlig |
| ApiPollingHistory | IX_ApiPolling_PolledAt | PolledAt | Vanlig |
| ApiPollingHistory | IX_ApiPolling_TicketsAvailable | TicketsAvailable | Vanlig |

## SQL migrering (kjerne-entities)

```sql
CREATE TABLE "Users" (
    "Id" uuid PRIMARY KEY,
    "Email" varchar(256) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Name" varchar(128),
    "EmailVerified" boolean NOT NULL DEFAULT false,
    "RefreshToken" varchar(512),
    "RefreshTokenExpiresAt" timestamptz,
    "CreatedAt" timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt" timestamptz
);

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE TABLE "Events" (
    "Id" uuid PRIMARY KEY,
    "TicketmasterEventId" varchar(64) NOT NULL,
    "Title" varchar(512) NOT NULL,
    "Artist" varchar(256),
    "Venue" varchar(256),
    "City" varchar(128),
    "EventDate" timestamptz NOT NULL,
    "TicketmasterUrl" varchar(2048) NOT NULL,
    "ImageUrl" varchar(2048),
    "Genre" varchar(128),
    "CreatedAt" timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt" timestamptz
);

CREATE UNIQUE INDEX "IX_Events_TicketmasterEventId" ON "Events" ("TicketmasterEventId");
CREATE INDEX "IX_Events_EventDate" ON "Events" ("EventDate");

CREATE TABLE "Watches" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL REFERENCES "Users"("Id"),
    "EventId" uuid NOT NULL REFERENCES "Events"("Id"),
    "Status" int NOT NULL DEFAULT 0,
    "CreatedAt" timestamptz NOT NULL DEFAULT now(),
    "ExpiresAt" timestamptz NOT NULL,
    "TriggeredAt" timestamptz
);

CREATE INDEX "IX_Watches_UserId" ON "Watches" ("UserId");
CREATE INDEX "IX_Watches_Status_ExpiresAt" ON "Watches" ("Status", "ExpiresAt");

CREATE TABLE "Payments" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL REFERENCES "Users"("Id"),
    "WatchId" uuid NOT NULL REFERENCES "Watches"("Id"),
    "StripeSessionId" varchar(256) NOT NULL,
    "StripePaymentIntentId" varchar(256),
    "Amount" numeric(10,2) NOT NULL,
    "Currency" varchar(4) NOT NULL DEFAULT 'nok',
    "Status" int NOT NULL DEFAULT 0,
    "CreatedAt" timestamptz NOT NULL DEFAULT now(),
    "CompletedAt" timestamptz
);

CREATE UNIQUE INDEX "IX_Payments_WatchId" ON "Payments" ("WatchId");
CREATE UNIQUE INDEX "IX_Payments_StripeSessionId" ON "Payments" ("StripeSessionId");

CREATE TABLE "Notifications" (
    "Id" uuid PRIMARY KEY,
    "WatchId" uuid NOT NULL REFERENCES "Watches"("Id"),
    "UserId" uuid NOT NULL REFERENCES "Users"("Id"),
    "Type" int NOT NULL,
    "Recipient" varchar(256) NOT NULL,
    "Subject" varchar(512) NOT NULL,
    "Body" text NOT NULL,
    "Sent" boolean NOT NULL DEFAULT false,
    "SentAt" timestamptz,
    "CreatedAt" timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX "IX_Notifications_UserId" ON "Notifications" ("UserId");

CREATE TABLE "ApiPollingHistory" (
    "Id" uuid PRIMARY KEY,
    "EventId" uuid NOT NULL REFERENCES "Events"("Id"),
    "TicketsAvailable" boolean NOT NULL,
    "TotalCount" int,
    "RawResponse" jsonb,
    "HttpStatusCode" int,
    "DurationMs" bigint NOT NULL,
    "PolledAt" timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX "IX_ApiPollingHistory_EventId" ON "ApiPollingHistory" ("EventId");
CREATE INDEX "IX_ApiPollingHistory_PolledAt" ON "ApiPollingHistory" ("PolledAt");
```
