########################################
#  First stage of multistage build
########################################
#  Use Build image with label 'builder'
########################################
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS builder

# ASP.NET Core with React.js requires npm
RUN apk add --update npm

# Setup working directory for project
WORKDIR /app

# Copy project files
COPY *.csproj ./

# Copy nuget.config so private feed is available
COPY nuget.config ./

# Copy private feed into the image since Docker cannot access a folder-based NuGet source
COPY *.nupkg /localnugetfeed/

# Restore nuget packages
RUN dotnet restore --configfile "./nuget.config"

# Copy project files
COPY . ./

# Build project with Release configuration
# and no restore, as we did it already
RUN dotnet build -c Release --no-restore

## Test project with Release configuration
## and no build, as we did it already
#RUN dotnet test -c Release --no-build


# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

########################################
#  Second stage of multistage build
########################################
#  Use other build image as the final one
#    that won't have source code
########################################
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

# Setup working directory for project
WORKDIR /app

# Copy published in previous stage binaries
# from the `builder` image
COPY --from=builder /app/out .

# Configure ports and HTTPS redirection
ENV ASPNETCORE_URLS="http://+:80;https://+:443"
ENV ASPNETCORE_HTTPS_PORT=5001
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

# sets entry point command to automatically 
# run application on `docker run`
ENTRYPOINT ["dotnet", "CouchbaseTester.dll"]