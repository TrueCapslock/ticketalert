FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/TicketAlert.Domain/TicketAlert.Domain.csproj src/TicketAlert.Domain/
COPY src/TicketAlert.Application/TicketAlert.Application.csproj src/TicketAlert.Application/
COPY src/TicketAlert.Infrastructure/TicketAlert.Infrastructure.csproj src/TicketAlert.Infrastructure/
COPY src/TicketAlert.Api/TicketAlert.Api.csproj src/TicketAlert.Api/
RUN dotnet restore src/TicketAlert.Api/TicketAlert.Api.csproj

COPY . .
RUN dotnet publish src/TicketAlert.Api/TicketAlert.Api.csproj \
    --configuration Release \
    --output /app \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app .

ENTRYPOINT ["dotnet", "TicketAlert.Api.dll"]
