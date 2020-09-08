## Couchbase Tester
This project is based on the ASP.NET Core with React.js template (`dotnet new react`). To launch the app run the following commands (from the project's home directory):\
* `dotnet publish`
* `.\bin\Debug\netcoreapp3.1\publish\CouchbaseTester.exe`

More details on the React.js front end can be found [here](ClientApp/README.md).

A local instance of Couchbase is required for the basic connectivity test and N1QL query added to the WeatherForecastController. An easy approach is to run the provided [Docker container](https://hub.docker.com/r/couchbase/server/). If you are using Windows and have upgraded to [WSL 2.0](https://docs.microsoft.com/en-us/windows/wsl/wsl2-index) this can be run within your preferred Linux distribution integrated with [Docker Desktop](https://docs.docker.com/docker-for-windows/wsl/).

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

Note that for the whole app to function the server side needs to be running too. So the preferred method of launching while developing/debugging is `dotnet run` from the project folder. (and [this SO suggestion](https://stackoverflow.com/a/61354275) is no longer required).