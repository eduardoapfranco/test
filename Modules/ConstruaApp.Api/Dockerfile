#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["Modules/ConstruaApp.Api/ConstruaApp.Api.csproj", "Modules/ConstruaApp.Api/"]
COPY ["Modules/ConstruaApp.Api/construa.bd", "Modules/ConstruaApp.Api/"]
COPY ["Modules/Application/Application.csproj", "Modules/Application/"]
COPY ["Modules/Domain/Domain.csproj", "Modules/Domain/"]
COPY ["Modules/Infra.CrossCutting/Infra.CrossCutting.csproj", "Modules/Infra.CrossCutting/"]
COPY ["Modules/Infra.Data/Infra.Data.csproj", "Modules/Infra.Data/"]
COPY ["Modules/Infra.IoC/Infra.IoC.csproj", "Modules/Infra.IoC/"]
RUN dotnet restore "Modules/ConstruaApp.Api/ConstruaApp.Api.csproj"
COPY . .
WORKDIR "/src/Modules/ConstruaApp.Api"
RUN dotnet build "ConstruaApp.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ConstruaApp.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
ENV TZ=America/Sao_Paulo
ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR.UTF-8
ENV LC_MONETARY pt_BR.UTF-8
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=publish /app .

# <wkhtmltopdf>
RUN apt-get update

RUN apt-get install wget libgdiplus -y

RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.dll

RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.dylib

RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.so
# </wkhtmltopdf>

ENTRYPOINT ["dotnet", "ConstruaApp.Api.dll"]