# ============================================================
# Dockerfile multi-stage (Linux) — ms-cuentas .NET
#   build:   SDK .NET 10 (compila y publica)
#   runtime: ASP.NET 10 (imagen liviana, solo runtime)
# Contexto de build: carpeta ms-cuentas/
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (cacheable): copiamos primero la solucion y los csproj
COPY Devsu.Cuentas.slnx ./
COPY src/Devsu.Cuentas.Domain/Devsu.Cuentas.Domain.csproj                 src/Devsu.Cuentas.Domain/
COPY src/Devsu.Cuentas.Application/Devsu.Cuentas.Application.csproj         src/Devsu.Cuentas.Application/
COPY src/Devsu.Cuentas.Infrastructure/Devsu.Cuentas.Infrastructure.csproj   src/Devsu.Cuentas.Infrastructure/
COPY src/Devsu.Cuentas.Api/Devsu.Cuentas.Api.csproj                         src/Devsu.Cuentas.Api/
RUN dotnet restore src/Devsu.Cuentas.Api/Devsu.Cuentas.Api.csproj

# Copiamos el resto del codigo y publicamos
COPY src/ src/
RUN dotnet publish src/Devsu.Cuentas.Api/Devsu.Cuentas.Api.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Puerto interno del contenedor
ENV Kestrel__Endpoints__Http__Url=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Devsu.Cuentas.Api.dll"]
