FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ODataSamples/ODataSamples.csproj", "ODataSamples/"]
RUN dotnet restore "ODataSamples/ODataSamples.csproj"
COPY . .
WORKDIR "/src/ODataSamples"
RUN dotnet build "ODataSamples.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ODataSamples.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ODataSamples.dll"]