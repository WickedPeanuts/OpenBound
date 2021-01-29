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

RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Game Server/OpenBound Lobby Server.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Login Server/OpenBound Login Server.csproj"

RUN dotnet restore

WORKDIR "/OpenBound/OpenBound Network Object Library/"
RUN dotnet restore
RUN dotnet build -c Release -o "/OpenBound Lobby Server"

WORKDIR "/OpenBound/OpenBound Lobby Server/"
RUN dotnet restore
RUN dotnet build -c Release -o "/OpenBound Lobby Server"

RUN rm -r "/OpenBound"

EXPOSE __container_port__

WORKDIR "/OpenBound Lobby Server"
ENTRYPOINT ["dotnet", "OpenBound Lobby Server.dll"]