# RatanaLibrary

[![NuGet Status](https://img.shields.io/nuget/v/RatanaLibrary.Common.svg)](https://www.nuget.org/packages/RatanaLibrary.Common)  
[![Build status](https://ci.appveyor.com/api/projects/status/osjl0yc29i7i5tv7/branch/master?svg=true)](https://ci.appveyor.com/project/superkarn/ratanalibrary/branch/master)  

A general library for personal projects.  Current features include
* Cache
* Email
* Log
* Profiler
    * DisposableStopwatch

# Cache
This library supports these implementations of `ICache`: `NoCache`, `InMemoryCache`, and `RedisCache`.  

```C#
ICache cache = new RedisCache(new RedisCache.RedisSettings() { Server = "localhost" });
            
var cachedValue = cache.GetOrAdd("MyKey", () =>
{
    return "do some work";
});
```


# Email
This library supports these implementations of `IEmailer`: `SystemNetEmailer`

```C#
SmtpClient smptClient;
IEmailer emailer = new SystemNetEmailer(smptClient);

// Build your own MailMessage
using (var mailMessage = new MailMessage() { })
{
    emailer.Send(mailMessage);
}

// Use shortcut
emailer.Send("from@example.com", "to@example.com", "email subject", "email body");
```


# Log
This library supports these implementations of `ILogger`: `SerilogLogger`

```C#
ILogger logger;
logger.Verbose("some message");
logger.Debug("some message");
logger.Information("some message");
logger.Warning("some message");
logger.Error("some message");
logger.Fatal("some message");
```


# Profiler
Profiler class currently provides one usage: `DisposableStopwatch`.  You can use this to track how long a section of your code takes.

```C#
ILogger logger;
IProfiler profiler = new Profiler(logger);
            
using (profiler.GetStopwatchInformation("MyKey"))
{
    // do some work
}
```
