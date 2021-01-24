using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Management_Tools.Common
{
    class Parameter
    {
        public const int DEFAULT_FETCH_SERVER_CONTAINER_PORT = 8000;
        public const int DEFAULT_FETCH_SERVER_STARTING_PORT  = 8100;
        public const string DEFAULT_FETCH_SERVER_CONTAINER_NAME  = "openbound-fetch-server";
        public const string DEFAULT_FETCH_SERVER_VOLUME_NAME     = "openbound_fetch_server_data";
        public const string DEFAULT_FETCH_SERVER_CONTEXT         = "../";
        public const string DEFAULT_FETCH_SERVER_DOCKERFILE_PATH = "Docker/Dockerfile.OpenBoundFetchServer";

        public const int DEFAULT_GAME_SERVER_CONTAINER_PORT = 8024;
        public const int DEFAULT_GAME_SERVER_STARTING_PORT  = 8024;
        public const string DEFAILT_GAME_SERVER_CONTAINER_NAME  = "openbound-game-server";
        public const string DEFAULT_GAME_SERVER_VOLUME_NAME     = "openbound_game_server_data";
        public const string DEFAULT_GAME_SERVER_CONTEXT         = "./";
        public const string DEFAULT_GAME_SERVER_DOCKERFILE_PATH = "OpenBound Game Server/GameServer.Dockerfile";
    }
}
