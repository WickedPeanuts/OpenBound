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
        public const string DEFAULT_FETCH_SERVER_DOCKERFILE_PATH = "FetchServer/OpenBoundFetchServer.Dockerfile";
        
        public const int DEFAULT_DATABASE_SERVER_CONTAINER_PORT = 1433;
        public const int DEFAULT_DATABASE_SERVER_STARTING_PORT  = 1433;
        public const string DEFAULT_DATABASE_SERVER_CONTAINER_NAME  = "openbound-database";
        public const string DEFAULT_DATABASE_SERVER_VOLUME_NAME     = "openbound_database_data";
        public const string DEFAULT_DATABASE_SERVER_CONTEXT         = "./";
        public const string DEFAULT_DATABASE_SERVER_DOCKERFILE_PATH = "OpenBoundDatabase.Dockerfile";
        public const string DEFAULT_DATABASE_SERVER_PASSWORD        = "P@55w0rD";
        public const string DEFAULT_DATABASE_SERVER_PID             = "Express";

        public const int DEFAULT_GAME_SERVER_CONTAINER_PORT = 8024;
        public const int DEFAULT_GAME_SERVER_STARTING_PORT  = 8024;
        public const string DEFAILT_GAME_SERVER_CONTAINER_NAME  = "openbound-game-server";
        public const string DEFAULT_GAME_SERVER_VOLUME_NAME     = "openbound_game_server_data";
        public const string DEFAULT_GAME_SERVER_CONTEXT         = "./";
        public const string DEFAULT_GAME_SERVER_DOCKERFILE_PATH = "OpenBound Game Server/GameServer.Dockerfile";

        public const int DEFAULT_LOBBY_SERVER_CONTAINER_PORT = 8023;
        public const int DEFAULT_LOBBY_SERVER_STARTING_PORT  = 8023;
        public const string DEFAILT_LOBBY_SERVER_CONTAINER_NAME  = "openbound-lobby-server";
        public const string DEFAULT_LOBBY_SERVER_VOLUME_NAME     = "openbound_lobby_server_data";
        public const string DEFAULT_LOBBY_SERVER_CONTEXT         = "./";
        public const string DEFAULT_LOBBY_SERVER_DOCKERFILE_PATH = "OpenBound Lobby Server/LobbyServer.Dockerfile";

        public const int DEFAULT_LOGIN_SERVER_CONTAINER_PORT = 8022;
        public const int DEFAULT_LOGIN_SERVER_STARTING_PORT = 8022;
        public const string DEFAILT_LOGIN_SERVER_CONTAINER_NAME  = "openbound-login-server";
        public const string DEFAULT_LOGIN_SERVER_VOLUME_NAME     = "openbound_login_server_data";
        public const string DEFAULT_LOGIN_SERVER_CONTEXT         = "./";
        public const string DEFAULT_LOGIN_SERVER_DOCKERFILE_PATH = "OpenBound Login Server/LoginServer.Dockerfile";


        


    }
}
