# Kurumsal Web — .NET 10 (Linux). docker-compose.yml ile birlikte kullanılır.
# Derleme: docker build -t kurumsal-web .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS derleme
WORKDIR /kaynak

# Önce yalnızca proje dosyası: paket geri yükleme katmanı kod değişmedikçe önbellekten gelir.
COPY CorporateWebsite/CorporateWebsite.csproj CorporateWebsite/
RUN dotnet restore CorporateWebsite/CorporateWebsite.csproj

COPY CorporateWebsite/ CorporateWebsite/
RUN dotnet publish CorporateWebsite/CorporateWebsite.csproj -c Release -o /yayin --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS calisma
WORKDIR /uygulama

# Türkçe tarih/sayı biçimleri için ICU (resmî imajda var); saat dilimi İstanbul.
ENV TZ=Europe/Istanbul \
    ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=derleme /yayin .

# Kalıcı veriler (docker-compose'da volume): yüklenen resimler, günlükler ve oturum şifreleme anahtarları.
RUN mkdir -p /uygulama/wwwroot/Uploads /uygulama/App_Data \
    && chown -R app:app /uygulama/wwwroot/Uploads /uygulama/App_Data
USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "CorporateWebsite.dll"]
