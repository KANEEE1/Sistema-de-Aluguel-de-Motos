FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/RentalSystem.Api/RentalSystem.Api.csproj", "src/RentalSystem.Api/"]
RUN dotnet restore "src/RentalSystem.Api/RentalSystem.Api.csproj"
COPY . .
WORKDIR "/src/src/RentalSystem.Api"
RUN dotnet build "RentalSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RentalSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY --from=build /src/src/RentalSystem.Api /src/src/RentalSystem.Api

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "RentalSystem.Api.dll"]
