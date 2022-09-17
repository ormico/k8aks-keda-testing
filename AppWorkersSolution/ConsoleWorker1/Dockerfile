#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ConsoleWorker1/AksKedaDemo.ConsoleWorker1.csproj", "ConsoleWorker1/"]
RUN dotnet restore "ConsoleWorker1/AksKedaDemo.ConsoleWorker1.csproj"
COPY . .
WORKDIR "/src/ConsoleWorker1"
RUN dotnet build "AksKedaDemo.ConsoleWorker1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AksKedaDemo.ConsoleWorker1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AksKedaDemo.ConsoleWorker1.dll"]