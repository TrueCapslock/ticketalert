# Coolify-deployment for TicketAlert

## Forutsetninger

- Coolify-instans (≥ v4.x) med riktig server konfigurert
- Domene eller subdomene klar for tjenesten
- Miljøvariabler klare (se `.env.example` i rot)

## Alternativ 1 — Docker Compose (anbefalt)

Dette alternativet deployer PostgreSQL, backend og frontend som én stack.

1. **Lag et nytt prosjekt i Coolify** → `ticketalert`

2. **Legg til en "Docker Compose"-ressurs** og lim inn `docker-compose.yml`  
   Eller pek Coolify til Git-repoet og velg `docker-compose.yml`

3. **Sett miljøvariabler** i Coolify UI.  
   Minimumskrav:

   | Variabel | Beskrivelse |
   |----------|-------------|
   | `POSTGRES_PASSWORD` | Passord for PostgreSQL |
   | `Jwt__Key` | Minimum 32 tegn for HS256 |
   | `Ticketmaster__ApiKey` | API-nøkkel fra Ticketmaster |
   | `Stripe__SecretKey` | Stripe secret key |
   | `Stripe__WebhookSecret` | Stripe webhook secret |
   | `Email__SmtpHost` | SMTP-server |
   | `Email__Username` | SMTP-brukernavn |
   | `Email__Password` | SMTP-passord |

4. **Koble til et domene** → pek til `frontend`-tjenesten på port 80.

5. **Start stacken**. Coolify bygger images automatisk ved push.

## Alternativ 2 — Individuelle tjenester

Hver service kan deployes separat i Coolify for mer granularitet.

### Backend

1. **Ny ressurs** → "Dockerfile" → pek til `backend/Dockerfile`
2. Sett `Ports` til `8080` (intern)
3. Legg til miljøvariabler (samme som over, uten prefix)
4. Legg til en PostgreSQL-tjeneste via Coolifys database-meny

### Frontend

1. **Ny ressurs** → "Dockerfile" → pek til `frontend/Dockerfile`
2. Sett `Ports` til `80` (intern)
3. Legg til miljøvariabel: `VITE_API_URL` (la stå tom, nginx proxy håndterer dette)
4. I `nginx.conf` må `proxy_pass http://backend:8080/api/;`  
   endres til Coolifys interne domenenavn for backend-tjenesten  
   (f.eks. `proxy_pass http://ticketalert-backend:8080/api/;`)

## Database-migreringer

Coolify kan kjøre et migreringsscript ved førstegangs oppstart.

**Alternativ A:** Legg til en `entrypoint.sh` i backend Dockerfile:

```dockerfile
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
```

Med innhold:

```bash
#!/bin/sh
dotnet ef database update --connection "$ConnectionStrings__DefaultConnection"
dotnet TicketAlert.Api.dll
```

**Alternativ B:** Kjør manuelt via Coolifys "Execute Command" på backend-containeren:

```bash
dotnet ef database update
```

## Nettverkssikkerhet

- PostgreSQL-porten (`5432`) skal kun være tilgjengelig internt i Docker-nettverket
- Backend-porten (`8080`) skal kun være tilgjengelig internt (frontend nginx proxy)
- Kun frontend-porten (`80`) eksponeres mot omverden

## Oppdateringer

Ved push til Git-repoet kan Coolify automatisk bygge og deployere på nytt:

1. Gå til ressursen i Coolify
2. Aktiver "Auto update" under "Settings → Watch path"
3. Velg gren (f.eks. `main`)

## Feilsøking

| Problem | Sjekk |
|---------|-------|
| Backend starter ikke | Logg: `ConnectionStrings__DefaultConnection` er satt korrekt? |
| Frontend får 502 på /api | Logg: `nginx.conf` sin `proxy_pass` peker til riktig backend-host? |
| DB-tilkobling feiler | Er PostgreSQL-containeren healthy? (`docker ps`) |
| E-post blir ikke sendt | Sjekk SMTP-innstillinger og at port 587/465 er åpen |
