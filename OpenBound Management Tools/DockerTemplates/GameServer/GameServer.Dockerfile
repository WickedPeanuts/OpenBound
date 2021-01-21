FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine3.12 as openbound-game-server
WORKDIR "/"

ARG target_opb="/OpenBound"
ARG target_g_s="/OpenBound/OpenBound Game Server/"
ARG target_nol="/OpenBound/OpenBound Network Object Library/"

ARG target_opb_sln="./OpenBound.sln"
ARG target_ogs_csproj="./OpenBound Game Server/*.csproj"
ARG target_nol_csproj="./OpenBound Network Object Library/*.csproj"

RUN mkdir "/OpenBound"
RUN mkdir "/OpenBound/OpenBound Game Server/"
RUN mkdir "/OpenBound/OpenBound Network Object Library/"

COPY ${target_opb_sln} ${target_opb}
COPY ${target_nol_csproj} ${target_nol}
COPY ${target_ogs_csproj} ${target_g_s}

WORKDIR "/OpenBound"
COPY . .

RUN dotnet sln "OpenBound.sln" remove "/OpenBound/Avatar API/Avatar API.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound/OpenBound.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Asset Tools/OpenBound Asset Tools.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Game Launcher/OpenBound Game Launcher.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Management Tools/OpenBound Management Tools.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Patcher/OpenBound Patcher.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/WP Image Processing/WP Image Processing.csproj"

RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Lobby Server/OpenBound Lobby Server.csproj"
RUN dotnet sln "OpenBound.sln" remove "/OpenBound/OpenBound Login Server/OpenBound Login Server.csproj"

RUN dotnet restore

WORKDIR "/OpenBound/OpenBound Network Object Library/"
RUN dotnet restore
RUN dotnet build -c Release -o "/OpenBound Game Server"

WORKDIR "/OpenBound/OpenBound Game Server/"
RUN dotnet restore
RUN dotnet build -c Release -o "/OpenBound Game Server"

RUN rm -r "/OpenBound"
WORKDIR "/"
RUN ls
WORKDIR "/OpenBound Game Server"
RUN ls

EXPOSE 8023
EXPOSE 8024

ENTRYPOINT ["dotnet", "OpenBound Game Server.dll"]