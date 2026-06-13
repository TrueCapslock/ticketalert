# API-kontrakter

## Authentication

### POST /api/auth/register
```
Request:
{
  "email": "user@example.com",
  "password": "minst8tegn",
  "name": "Ola Nordmann"          // valgfritt
}

Response 201:
{
  "token": "eyJhbGciOi...",
  "refreshToken": "base64...",
  "expiresAt": "2026-06-13T13:00:00Z",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "name": "Ola Nordmann",
    "emailVerified": false
  }
}
```

### POST /api/auth/login
```
Request:
{
  "email": "user@example.com",
  "password": "secret"
}

Response 200: (samme som register)
```

### POST /api/auth/refresh
```
Request:
{
  "refreshToken": "base64..."
}

Response 200:
{
  "token": "ny token",
  "refreshToken": "ny refresh token",
  "expiresAt": "2026-06-13T14:00:00Z",
  "user": { ... }
}
```

### POST /api/auth/forgot-password
```
Request:
{
  "email": "user@example.com"
}

Response 200:
{
  "message": "If the email exists, a reset link has been sent."
}
```

### GET /api/auth/me (Authorized)
```
Response 200:
{
  "id": "guid",
  "email": "user@example.com",
  "name": "Ola Nordmann",
  "emailVerified": true
}
```

## Events

### GET /api/events/search?keyword=xxx&artist=xxx&city=xxx&page=1&pageSize=20
```
Response 200:
{
  "events": [
    {
      "id": "guid",
      "ticketmasterEventId": "Z7r9jZ1AdOkJ2",
      "title": "Coldplay - Music Of The Spheres",
      "artist": "Coldplay",
      "venue": "Telenor Arena",
      "city": "Oslo",
      "eventDate": "2026-07-15T19:00:00Z",
      "ticketmasterUrl": "https://www.ticketmaster.no/...",
      "imageUrl": "https://...",
      "genre": "Rock"
    }
  ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20
}
```

### GET /api/events/{id}
```
Response 200:
{
  "id": "guid",
  "ticketmasterEventId": "Z7r9jZ1AdOkJ2",
  "title": "Coldplay - Music Of The Spheres",
  "artist": "Coldplay",
  "venue": "Telenor Arena",
  "city": "Oslo",
  "eventDate": "2026-07-15T19:00:00Z",
  "ticketmasterUrl": "https://www.ticketmaster.no/...",
  "imageUrl": "https://...",
  "genre": "Rock",
  "isWatched": true,
  "watchId": "guid",
  "watchStatus": "Active"
}
```

## Watches

### POST /api/watches (Authorized)
```
Request:
{
  "ticketmasterEventId": "Z7r9jZ1AdOkJ2",
  "title": "Coldplay - Music Of The Spheres",
  "artist": "Coldplay",
  "venue": "Telenor Arena",
  "city": "Oslo",
  "eventDate": "2026-07-15T19:00:00Z",
  "ticketmasterUrl": "https://www.ticketmaster.no/...",
  "imageUrl": "https://..."
}

Response 201:
{
  "id": "guid",
  "eventId": "guid",
  "eventTitle": "Coldplay - Music Of The Spheres",
  "artist": "Coldplay",
  "venue": "Telenor Arena",
  "eventDate": "2026-07-15T19:00:00Z",
  "ticketmasterUrl": "https://www.ticketmaster.no/...",
  "status": "Active",
  "createdAt": "2026-06-13T12:00:00Z",
  "expiresAt": "2026-07-15T19:00:00Z",
  "triggeredAt": null
}
```

### GET /api/watches?status=Active (Authorized)
```
Response 200:
[
  {
    "id": "guid",
    "eventId": "guid",
    "eventTitle": "Coldplay - Music Of The Spheres",
    ...
    "status": "Active",
    ...
  }
]
```

### DELETE /api/watches/{id} (Authorized)
```
Response: 204 No Content
```

## Payments

### POST /api/payments/checkout (Authorized)
```
Request:
{
  "eventId": "guid",
  "successUrl": "https://ticketalert.no/dashboard?success=true",
  "cancelUrl": "https://ticketalert.no/events/{id}"
}

Response 200:
{
  "sessionUrl": "https://checkout.stripe.com/...",
  "sessionId": ""
}
```

### POST /api/payments/webhook (Stripe)
```
Headers:
  Stripe-Signature: whsec_...

Body: [raw Stripe event JSON]

Response: 200 OK
```

### GET /api/payments/history (Authorized)
```
Response 200:
[
  {
    "id": "guid",
    "watchId": "guid",
    "eventTitle": "Coldplay - Music Of The Spheres",
    "amount": 19.00,
    "currency": "nok",
    "status": "Completed",
    "createdAt": "2026-06-13T12:00:00Z",
    "completedAt": "2026-06-13T12:01:00Z"
  }
]
```

## Notifications

### GET /api/notifications?page=1&pageSize=20 (Authorized)
```
Response 200:
{
  "notifications": [
    {
      "id": "guid",
      "watchId": "guid",
      "type": "Email",
      "subject": "Billetter tilgjengelig: Coldplay!",
      "body": null,
      "sent": true,
      "sentAt": "2026-06-13T13:00:00Z",
      "createdAt": "2026-06-13T13:00:00Z"
    }
  ],
  "totalCount": 5,
  "unreadCount": 2
}
```
