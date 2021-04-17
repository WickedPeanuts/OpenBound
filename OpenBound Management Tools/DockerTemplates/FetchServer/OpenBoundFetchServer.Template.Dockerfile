﻿FROM nginx:1.19.6-alpine as __container_name__
COPY ./FetchServer/OpenBoundFetchServer.nginx.conf /etc/nginx/nginx.conf

## Create versioning & game_patches folder
## In case you decide to change this path, remember to update the description and a few fields in the following files
## OpenBound_Network_Object_Library.Common.NetworkObjectParameters.cs
## OpenBound_Management_Tools/Docker/ConfigFiles/OpenBoundFetchServer.nginx.conf
RUN mkdir -p __versioning_folder__
RUN mkdir -p __game_patches_folder__

## Remove default nginx index page
RUN rm -fR /usr/share/nginx/html/*