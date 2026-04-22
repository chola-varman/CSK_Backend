FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Api/CskMasala.Api.csproj", "src/Api/"]
COPY ["src/Shared/CskMasala.Shared.csproj", "src/Shared/"]
COPY ["src/Identity/CskMasala.Identity.Contracts/CskMasala.Identity.Contracts.csproj", "src/Identity/CskMasala.Identity.Contracts/"]
COPY ["src/Identity/CskMasala.Identity.Domain/CskMasala.Identity.Domain.csproj", "src/Identity/CskMasala.Identity.Domain/"]
COPY ["src/Identity/CskMasala.Identity.Application/CskMasala.Identity.Application.csproj", "src/Identity/CskMasala.Identity.Application/"]
COPY ["src/Identity/CskMasala.Identity.Infrastructure/CskMasala.Identity.Infrastructure.csproj", "src/Identity/CskMasala.Identity.Infrastructure/"]
COPY ["src/Identity/CskMasala.Identity.Api/CskMasala.Identity.Api.csproj", "src/Identity/CskMasala.Identity.Api/"]
COPY ["src/Retail/CskMasala.Retail.Contracts/CskMasala.Retail.Contracts.csproj", "src/Retail/CskMasala.Retail.Contracts/"]
COPY ["src/Retail/CskMasala.Retail.Domain/CskMasala.Retail.Domain.csproj", "src/Retail/CskMasala.Retail.Domain/"]
COPY ["src/Retail/CskMasala.Retail.Application/CskMasala.Retail.Application.csproj", "src/Retail/CskMasala.Retail.Application/"]
COPY ["src/Retail/CskMasala.Retail.Infrastructure/CskMasala.Retail.Infrastructure.csproj", "src/Retail/CskMasala.Retail.Infrastructure/"]
COPY ["src/Retail/CskMasala.Retail.Api/CskMasala.Retail.Api.csproj", "src/Retail/CskMasala.Retail.Api/"]
COPY ["src/Payment/CskMasala.Payment.Contracts/CskMasala.Payment.Contracts.csproj", "src/Payment/CskMasala.Payment.Contracts/"]
COPY ["src/Payment/CskMasala.Payment.Domain/CskMasala.Payment.Domain.csproj", "src/Payment/CskMasala.Payment.Domain/"]
COPY ["src/Payment/CskMasala.Payment.Application/CskMasala.Payment.Application.csproj", "src/Payment/CskMasala.Payment.Application/"]
COPY ["src/Payment/CskMasala.Payment.Infrastructure/CskMasala.Payment.Infrastructure.csproj", "src/Payment/CskMasala.Payment.Infrastructure/"]
COPY ["src/Payment/CskMasala.Payment.Api/CskMasala.Payment.Api.csproj", "src/Payment/CskMasala.Payment.Api/"]
COPY ["src/Receipt/CskMasala.Receipt.Contracts/CskMasala.Receipt.Contracts.csproj", "src/Receipt/CskMasala.Receipt.Contracts/"]
COPY ["src/Receipt/CskMasala.Receipt.Application/CskMasala.Receipt.Application.csproj", "src/Receipt/CskMasala.Receipt.Application/"]
COPY ["src/Receipt/CskMasala.Receipt.Api/CskMasala.Receipt.Api.csproj", "src/Receipt/CskMasala.Receipt.Api/"]
COPY ["src/Email/CskMasala.Email.Application/CskMasala.Email.Application.csproj", "src/Email/CskMasala.Email.Application/"]
COPY ["src/Email/CskMasala.Email.Worker/CskMasala.Email.Worker.csproj", "src/Email/CskMasala.Email.Worker/"]
RUN dotnet restore "src/Api/CskMasala.Api.csproj"

COPY . .
WORKDIR /src/src/Api
RUN dotnet build "CskMasala.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CskMasala.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CskMasala.Api.dll"]
