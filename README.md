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
This library supports these implementations of `ICache`: `NoCache`, `InMemoryCache`, `MultilevelCache` and `RedisCache`.  

```C#
// Redis
var cache1 = new InMemoryCache();
            
var cachedValue1 = cache1.GetOrAdd("MyKey", () =>
{
    return "do some work";
});

// Multilevel cache using InMemory as L1 and Redis as L2
var cache2 = new MultilevelCache(
    new InMemoryCache(), // L1 cache
    new RedisCache(new RedisCache.RedisSettings() { Server = "localhost" }) // L2 cache
);

var cachedValue2 = cache2.GetOrAdd("MyKey", () =>
    {
        return "do some work";
    },
    TimeSpan.FromSeconds(1), // L1 cache expiration
    TimeSpan.FromHours(1)); // L2 cache expiration
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
ILogContext context;
context.Add("key1", "value1");
context.Add("key2", "value2");

logger.Verbose(context, "some message");
logger.Debug(context, "some message with exception", exception);
logger.Information(context, "some message with arguments: {myArg1}, {myArg2}", myArg1, myArg2);
logger.Warning(context, "some message with exception and arguments: {myArg1}, {myArg2}", exception, myArg1, myArg2);
logger.Error("some message with no context");
logger.Fatal(context, "some message");
```


# Profiler
Profiler class currently provides one usage: `DisposableStopwatch`.  You can use this to track how long a section of your code takes.

```C#
ILogger logger;
IProfiler profiler = new Profiler(logger);

       
using (profiler.GetStopwatchVerbose("MyKey"))
{
    // track work time at Verbose level
}
       
using (profiler.GetStopwatchDebug("MyKey"))
{
    // track work time at Debug level
}

using (profiler.GetStopwatchInformation("MyKey"))
{
    // track work time at Information level
}
```
