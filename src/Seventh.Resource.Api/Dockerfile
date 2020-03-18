#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["src/Seventh.Resource.Api/Seventh.Resource.Api.csproj", "src/Seventh.Resource.Api/"]
COPY ["src/Seventh.Resource/Seventh.Resource.csproj", "src/Seventh.Resource/"]
COPY ["src/Seventh.Resource.Services/Seventh.Resource.Services.csproj", "src/Seventh.Resource.Services/"]
COPY ["src/Seventh.Resource.Common/Seventh.Resource.Common.csproj", "src/Seventh.Resource.Common/"]
COPY ["src/Seventh.Core/Seventh.Core.csproj", "src/Seventh.Core/"]
RUN dotnet restore "src/Seventh.Resource.Api/Seventh.Resource.Api.csproj"
COPY . .
WORKDIR "/src/src/Seventh.Resource.Api"
RUN dotnet build "Seventh.Resource.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seventh.Resource.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seventh.Resource.Api.dll"]