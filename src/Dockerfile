FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["webApiAuthorize.csproj", "./"]
RUN dotnet restore "./webApiAuthorize.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "webApiAuthorize.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "webApiAuthorize.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webApiAuthorize.dll"]
