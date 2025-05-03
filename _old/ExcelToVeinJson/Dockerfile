FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExcelToVeinJson/ExcelToVeinJson.csproj", "ExcelToVeinJson/"]
RUN dotnet restore "ExcelToVeinJson/ExcelToVeinJson.csproj"
COPY . .
WORKDIR "/src/ExcelToVeinJson"
RUN dotnet build "ExcelToVeinJson.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExcelToVeinJson.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExcelToVeinJson.dll"]
