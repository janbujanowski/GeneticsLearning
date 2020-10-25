FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /source
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq

# Copy csproj and restore as distinct layers
COPY . ./build
RUN dotnet restore build/*.sln
RUN dotnet build build/*.sln -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app ./
ENTRYPOINT ["dotnet", "Neurox.Web.dll"]
