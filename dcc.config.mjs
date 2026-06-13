export default {
  name: 'TicketAlert',

  commands: [
    // ── Backend ──────────────────────────────────────────────
    {
      id: 'backend:dev',
      label: 'Backend — kjør (dotnet watch)',
      command: 'dotnet watch run --project backend/src/TicketAlert.Api',

    },
    {
      id: 'backend:exec',
      label: 'Backend — kjør via DLL (omgår Windows exe-blokkering)',
      command: 'dotnet exec backend/src/TicketAlert.Api/bin/Debug/net9.0/TicketAlert.Api.dll',
    },
    {
      id: 'backend:build',
      label: 'Backend — bygg',
      command: 'dotnet build backend/TicketAlert.sln --configuration Release',
    },
    {
      id: 'backend:test',
      label: 'Backend — test',
      command: 'dotnet test backend/TicketAlert.sln --configuration Release --no-build --verbosity normal',
    },
    {
      id: 'backend:migrate',
      label: 'Backend — kjør EF Core migrering',
      command: 'dotnet ef database update --project backend/src/TicketAlert.Infrastructure --startup-project backend/src/TicketAlert.Api',
    },
    {
      id: 'backend:migration-add',
      label: 'Backend — legg til ny migrering',
      command: 'read -p "Navn på migrering: " name && dotnet ef migrations add "$name" --project backend/src/TicketAlert.Infrastructure --startup-project backend/src/TicketAlert.Api',
      confirm: true,
    },

    // ── Frontend ─────────────────────────────────────────────
    {
      id: 'frontend:dev',
      label: 'Frontend — kjør (Vite dev)',
      command: 'npm run dev --prefix frontend',
    },
    {
      id: 'frontend:build',
      label: 'Frontend — bygg',
      command: 'npm run build --prefix frontend',
    },
    {
      id: 'frontend:test',
      label: 'Frontend — typecheck (tsc)',
      command: 'npx tsc --noEmit --prefix frontend',
    },
    {
      id: 'frontend:preview',
      label: 'Frontend — preview produksjonsbygg',
      command: 'npm run preview --prefix frontend',
    },

    // ── Docker ───────────────────────────────────────────────
    {
      id: 'docker:build',
      label: 'Docker — bygg alle images',
      command: 'docker compose build',
    },
    {
      id: 'docker:up',
      label: 'Docker — start alle tjenester',
      command: 'docker compose up -d',
    },
    {
      id: 'docker:down',
      label: 'Docker — stopp alle tjenester',
      command: 'docker compose down',
    },
    {
      id: 'docker:logs',
      label: 'Docker — følg logger',
      command: 'docker compose logs -f',
    },

    // ── Kvalitet ─────────────────────────────────────────────
    {
      id: 'lint',
      label: 'Lint — sjekk alle prosjekter',
      command: 'npm run lint --prefix frontend || true',
    },
    {
      id: 'clean',
      label: 'Clean — fjern byggemapper',
      command: 'rm -rf backend/src/**/bin backend/src/**/obj frontend/dist',
      confirm: true,
    },
  ],

  // ── Pipelines ──────────────────────────────────────────────
  pipelines: [
    {
      id: 'dev:all',
      label: '🚀 Start alt for utvikling',
      steps: ['docker:up', 'backend:dev', 'frontend:dev'],
    },
    {
      id: 'build:all',
      label: '📦 Bygg backend + frontend',
      steps: ['backend:build', 'frontend:build'],
    },
    {
      id: 'test:all',
      label: '🧪 Test backend + frontend',
      steps: ['backend:test', 'frontend:test'],
    },
    {
      id: 'docker:refresh',
      label: '🐳 Rebuild og start Docker',
      steps: ['docker:down', 'docker:build', 'docker:up'],
    },
  ],

  // ── Profiler ───────────────────────────────────────────────
  profiles: {
    dev: {
      commands: [
        { id: 'dev:full', label: 'Start alt (Docker + backend + frontend)', command: 'dcc run dev:all' },
        { id: 'backend:dev', label: 'Backend — kjør', command: 'dotnet watch run --project backend/src/TicketAlert.Api' },
        { id: 'frontend:dev', label: 'Frontend — kjør', command: 'npm run dev --prefix frontend' },
        { id: 'docker:up', label: 'Docker — start PostgreSQL', command: 'docker compose up -d postgres' },
      ],
    },
    ci: {
      commands: [
        { id: 'ci:validate', label: 'CI — bygg og test alt', command: 'dcc run build:all && dcc run test:all' },
      ],
    },
  },
};
