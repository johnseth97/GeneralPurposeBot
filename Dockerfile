FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY . /source
WORKDIR /source
RUN curl -sL https://deb.nodesource.com/setup_14.x | sudo -E bash -
RUN apt install -y nodejs
RUN dotnet restore
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GeneralPurposeBot.dll"]