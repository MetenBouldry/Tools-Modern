FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TFG-1.20.x-ExcelToVeinJson/TFG-1.20.x-ExcelToVeinJson.csproj", "TFG-1.20.x-ExcelToVeinJson/"]
RUN dotnet restore "TFG-1.20.x-ExcelToVeinJson/TFG-1.20.x-ExcelToVeinJson.csproj"
COPY . .
WORKDIR "/src/TFG-1.20.x-ExcelToVeinJson"
RUN dotnet build "TFG-1.20.x-ExcelToVeinJson.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TFG-1.20.x-ExcelToVeinJson.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TFG-1.20.x-ExcelToVeinJson.dll"]
