FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/LibrarySeatReservation.Web/*.csproj src/LibrarySeatReservation.Web/
RUN dotnet restore src/LibrarySeatReservation.Web/LibrarySeatReservation.Web.csproj

COPY src/ src/
RUN dotnet publish src/LibrarySeatReservation.Web/LibrarySeatReservation.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5000

ENTRYPOINT ["dotnet", "LibrarySeatReservation.Web.dll"]
