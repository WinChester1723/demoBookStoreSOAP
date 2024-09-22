# Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the solution and restore dependencies
COPY *.sln ./
COPY src/Application/*.csproj ./src/Application/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/
COPY src/BookStoreSOAPService/*.csproj ./src/BookStoreSOAPService/
COPY test/BookStoreSOAPTests/*.csproj ./test/BookStoreSOAPTests/
RUN dotnet restore

# Copy everything and build the project
COPY . ./
RUN dotnet publish src/BookStoreSOAPService/BookStoreSOAPService.csproj -c Release -o out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Installing Entity Framework Tools (only if needed in a container)
RUN apt-get update && apt-get install -y --no-install-recommends unzip \
    && rm -rf /var/lib/apt/lists/*

# Installation.NET SDK
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --install-dir /usr/share/dotnet --channel 8.0

# Install dotnet-ef (if necessary, you can specify the version)
RUN /usr/share/dotnet/dotnet tool install --global dotnet-ef --version 8.0.8

# Setting the environment variable to execute the EF Core CLI
ENV PATH="$PATH:/root/.dotnet/tools"

# Expose port 80 for the service
EXPOSE 80

# Define the entry point for the container
ENTRYPOINT ["dotnet", "BookStoreSOAPService.dll"]