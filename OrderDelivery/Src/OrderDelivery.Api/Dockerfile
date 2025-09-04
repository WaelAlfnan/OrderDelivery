# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file first
COPY OrderDelivery.sln ./

# Copy all project files
COPY OrderDelivery.Api/*.csproj ./OrderDelivery.Api/
COPY OrderDelivery.Application/*.csproj ./OrderDelivery.Application/
COPY OrderDelivery.Domain/*.csproj ./OrderDelivery.Domain/
COPY OrderDelivery.Infrastructure/*.csproj ./OrderDelivery.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY OrderDelivery.Api/ ./OrderDelivery.Api/
COPY OrderDelivery.Application/ ./OrderDelivery.Application/
COPY OrderDelivery.Domain/ ./OrderDelivery.Domain/
COPY OrderDelivery.Infrastructure/ ./OrderDelivery.Infrastructure/

# Build and publish the API project
RUN dotnet publish OrderDelivery.Api/OrderDelivery.Api.csproj -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80 and 443
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "OrderDelivery.Api.dll"]
