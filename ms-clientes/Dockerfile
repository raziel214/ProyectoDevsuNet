# ============================================================
# Dockerfile multi-stage (Linux) — ms-clientes .NET
#   build:   SDK .NET 10 (compila y publica)
#   runtime: ASP.NET 10 (imagen liviana, solo runtime)
# Contexto de build: carpeta ms-clientes/
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (cacheable): copiamos primero la solucion y los csproj
COPY Devsu.Clientes.slnx ./
COPY src/Devsu.Clientes.Domain/Devsu.Clientes.Domain.csproj                 src/Devsu.Clientes.Domain/
COPY src/Devsu.Clientes.Application/Devsu.Clientes.Application.csproj         src/Devsu.Clientes.Application/
COPY src/Devsu.Clientes.Infrastructure/Devsu.Clientes.Infrastructure.csproj   src/Devsu.Clientes.Infrastructure/
COPY src/Devsu.Clientes.Api/Devsu.Clientes.Api.csproj                         src/Devsu.Clientes.Api/
RUN dotnet restore src/Devsu.Clientes.Api/Devsu.Clientes.Api.csproj

# Copiamos el resto del codigo y publicamos
COPY src/ src/
RUN dotnet publish src/Devsu.Clientes.Api/Devsu.Clientes.Api.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Puerto interno del contenedor
ENV Kestrel__Endpoints__Http__Url=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Devsu.Clientes.Api.dll"]
