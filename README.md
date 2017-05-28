# Hangfire Task Dispatcher
Is a Hangfire extension to manage and deploy tasks from the web UI, this is a command pattern adaption of [tracstarr's](https://github.com/tracstarr) [Hangfire.Core.Dashboard.Management](https://github.com/tracstarr/Hangfire.Core.Dashboard.Management)

Installation
-------------

This library is available as a NuGet Package: [![NuGet](https://img.shields.io/nuget/v/Hangfire.Extension.TaskDispatcher.svg)](https://www.nuget.org/packages/Hangfire.Extension.TaskDispatcher/)

```
Install-Package Hangfire.Extension.TaskDispatcher
```

Setup
-------------

```c#
GlobalConfiguration.Configuration
    .UseSqlServerStorage("connectionSting")
    .UseTaskDispatcherPages(args);
```
## Option 1: all taskparameters are concrete objects

in this situation you can pass the assembly where your tasks are located

## Option 2: some taskparamters have a generic parameter

in this situation pass the configured list of taskhandlers from DI,  this will provide one task in the ui with a drop down of all the configured T's.
