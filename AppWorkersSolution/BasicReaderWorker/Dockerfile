#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BasicReaderWorker/AksKedaDemo.BasicReaderWorker.csproj", "BasicReaderWorker/"]
RUN dotnet restore "BasicReaderWorker/AksKedaDemo.BasicReaderWorker.csproj"
COPY . .
WORKDIR "/src/BasicReaderWorker"
RUN dotnet build "AksKedaDemo.BasicReaderWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AksKedaDemo.BasicReaderWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AksKedaDemo.BasicReaderWorker.dll"]