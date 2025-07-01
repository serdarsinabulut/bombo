FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Proje dosyalarını kopyala
COPY . .

# Bağımlılıkları restore et ve projeyi derle
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Runtime imajı
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "projectbombo.dll"]