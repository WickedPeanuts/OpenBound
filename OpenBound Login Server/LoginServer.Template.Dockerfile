FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine3.12 as __container_name__

RUN mkdir "/OpenBound"

COPY . "/OpenBound/"

WORKDIR "/OpenBound"

RUN dotnet sln "OpenBound.sln" remove "/OpenBound/Avatar API/Avatar API.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound/OpenBound.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Asset Tools/OpenBound Asset Tools.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Game Launcher/OpenBound Game Launcher.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Management Tools/OpenBound Management Tools.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Patcher/OpenBound Patcher.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/WP Image Processing/WP Image Processing.csproj"

RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Game Server/OpenBound Game Server.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Login Server/OpenBound Lobby Server.csproj"

RUN dotnet restore

WORKDIR "/OpenBound/OpenBound Network Object Library/"
RUN dotnet restore
RUN dotnet build -c Release -o "/OpenBound Login Server"

WORKDIR "/OpenBound/OpenBound Login Server/"
RUN dotnet restore
RUN dotnet build -c Release -o "/OpenBound Login Server"

RUN rm -r "/OpenBound"

EXPOSE __container_port__

WORKDIR "/OpenBound Login Server"
ENTRYPOINT ["dotnet", "OpenBound Login Server.dll"]