# Couchbase Tester
This project is based on the ASP.NET Core with React.js template (`dotnet new react`). To launch the app run the following commands (from the project's home directory):
* `dotnet publish`
* `.\bin\Debug\netcoreapp3.1\publish\CouchbaseTester.exe`

More details on the React.js front end can be found [here](ClientApp/README.md).

A local instance of Couchbase is required for the basic connectivity test and N1QL query added to the WeatherForecastController. An easy approach is to run the provided [Docker container](https://hub.docker.com/r/couchbase/server/). If you are using Windows and have upgraded to [WSL 2.0](https://docs.microsoft.com/en-us/windows/wsl/wsl2-index) this can be run within your preferred Linux distribution integrated with [Docker Desktop](https://docs.docker.com/docker-for-windows/wsl/).
* `docker run -d --name db -p 8091-8095:8091-8095 -p 11210:11210 couchbase`
* Set up a new cluster with defaults
* Install the __gamesim__ sample bucket

__Note:__ the run command seen on the docker hub page omits port 8095 which is required by the analytics service. Without this port exposed operations such as `WaitUntilReadyAsync()` will fail.

## Todo
* Deploy to Azure
* Try to remove Linq2Couchbase's dependency on Microsoft.Bcl.AsyncInterfaces (from https://www.nuget.org/packages/System.Linq.Async/)
* Ask Linq2Couchbase owners why a working package (e.g., 1.4.3 which I build off master locally) hasn't been pushed to NuGet

## Docker
To run in a Docker container perform the following steps (from the project's home directory):
* `docker build -t couchbase-tester . --rm`
* `docker run -d -p 5000:80 -p 5001:443 -e ASPNETCORE_Kestrel__Certificates__Default__Password="yourpassword" -v $env:USERPROFILE\.aspnet\log:/app/log -v $env:USERPROFILE\.aspnet\https:/https --name couchbase-tester couchbase-tester`

To inspect the filesystem of the running container use this command: `docker exec -it couchbase-tester /bin/ash`

<ins>**Linq2Couchbase compilation error**</ins>\
The latest version of this package, 1.4.2, does not compile when using a 3.x version of CouchbaseNetClient.

`CS7069 Reference to type 'IBucket' claims it is defined in 'Couchbase.NetClient', but it could not be found`

The solution was to push a 1.4.3 version to a local NuGet feed, compiled from the repo's master branch.

Then there were problems with the Dockerfile's restore step, since Docker cannot access a folder-based private feed. So instead of running a local NuGet server I decided to just bring the nupkg into the build context, then copy it to the container.

<ins>**Solution to the npm/yarn home directory problem**</ins>\
Ever since we began exploring .NET Core with React.js there had been an issue where npm and/or yarn always chose the home directory (e.g., c:\users\matt) as the `cwd`. This could be seen by simply running `npm get` and observing the cwd value. We did notice that the problem was isolated to Powershell terminals, and not the regular cmd processor which worked as expected.

This behavior broke everything about npm/yarn because it prevented any operations from working (package.json is essentially invisible).

[This SO answer](https://stackoverflow.com/a/29561882) provided the solution. The only key/value pair that was deleted is shown below
```Batchfile
Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\SOFTWARE\Microsoft\Command Processor]
"Autorun"="cd /d %userprofile%"
```

With that entry removed we can confirm the fix by running `npm start` from the `~\source\repos\CouchbaseTester\ClientApp` folder. The fix can also be tested by running `npm config list` within any folder, and confirming that the value of `cwd` is that folder (and not c:\users\matt).

Note that for the whole app to function the server side needs to be running too. So the preferred method of launching while developing/debugging is `dotnet run` from the project folder (and [this SO suggestion](https://stackoverflow.com/a/61354275) is no longer required).

## References
* https://hub.docker.com/r/microsoft/aspnetcore/
* https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1
* https://www.vivienfabing.com/docker/2019/10/03/docker-aspnetcore-container-and-https.html (copy of 2nd link but breaks down command line args)
* https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1#startup-class-conventions
* https://docs.couchbase.com/dotnet-sdk/current/howtos/collecting-information-and-logging.html
* https://natemcmaster.com/blog/2017/12/21/netcore-primitives
* http://4tecture.ch/blog/2020/08/31/Restore-Nuget-Packages-inside-a-Docker-Container