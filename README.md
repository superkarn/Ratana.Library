# Ratana.Library

[![Build status](https://ci.appveyor.com/api/projects/status/osjl0yc29i7i5tv7/branch/master?svg=true)](https://ci.appveyor.com/project/superkarn/ratanalibrary/branch/master)  

|Library Name                    | NuGet Version |
|--------------------------------|---------------|
|Ratana.Library.Cache            | [![NuGet Status](https://img.shields.io/nuget/v/Ratana.Library.Cache.svg)](https://www.nuget.org/packages/Ratana.Library.Cache)                        |
|Ratana.Library.DistributedCache | [![NuGet Status](https://img.shields.io/nuget/v/Ratana.Library.DistributedCache.svg)](https://www.nuget.org/packages/Ratana.Library.DistributedCache)  |
|Ratana.Library.Email            | [![NuGet Status](https://img.shields.io/nuget/v/Ratana.Library.Email.svg)](https://www.nuget.org/packages/Ratana.Library.Email)                        |
|Ratana.Library.Log              | [![NuGet Status](https://img.shields.io/nuget/v/Ratana.Library.Log.svg)](https://www.nuget.org/packages/Ratana.Library.Log)                            |
|Ratana.Library.Profiler         | [![NuGet Status](https://img.shields.io/nuget/v/Ratana.Library.Profiler.svg)](https://www.nuget.org/packages/Ratana.Library.Profiler)                  |


A general library for personal projects.  Current features include
* Cache
* Multilevel Distributed Cache
* Email
* Log
* Profiler
    * DisposableStopwatch

# Cache
This library supports these implementations of `ICache`: `NoCache`, `InMemoryCache`, `RedisCache`, `MultilevelCache`, and `MultilevelDistributedCache`.  

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


# MultilevelDistributedCache
This library implements [Microsoft.Extensions.Caching.Distributed.IDistributedCache](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) while providing arbitrary multilevel caching mechanism.  Each level is also an IDistributedCache, with the lowest level (L1) being accessed first (e.g. should be the quickest).  Note that this is library does not depend on `Ratana.Library.Cache`.

```C#
var redisCacheOptions = new RedisCacheOptions();

IDistributedCache cache = new MultilevelCache(
    new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())), // L1 cache
    new RedisCache(Options.Create(redisCacheOptions))                                // L2 Cache
);
```

## Drawbacks
There are some known drawbacks/gotchas in this library.  The first is `GetAsync()`.  This method is not a true asynchronous method as it needs to get the data back from the first level cache before trying the second level.

The second drawback is related to using [Microsoft.Extensions.Caching.Distributed.MemoryDistributedCache](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.memorydistributedcache).  Since this class is not a true distributed cache (see [here](https://github.com/aspnet/Caching/issues/322)), when calling `Remove()` in a multi-server set up, only the current server will have its in-memory cache removed.  All real distributed cache implementations (e.g. Redis and SqlServer) are not affected by this drawback.



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
