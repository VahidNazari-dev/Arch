

using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Arch.CQRS.Query.Behavior;

public class QueryCachedBehave<TQuery, TResult> : IPipelineBehavior<TQuery, TResult> where TQuery : IQuery<TResult>
{
    
    private readonly IDistributedCache _cache;

    public QueryCachedBehave(IDistributedCache cache)
    {
        _cache = cache;
    }
    public Task<TResult> Handle(TQuery request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(5000);
        cts.CancelAfter(5000);
        if (request is QueryCached<TResult> cached)
        {
            if (!cached.ReValidate)
            {
                return GetOrCreateAsync(
                    cached.GetKey(),
                    (options) =>
                    {
                        options.AbsoluteExpiration = cached.AbsoluteExpiration;
                        options.SlidingExpiration = cached.SlidingExpiration;
                        return next();
                    },
                    cts.Token);
            }
            else
            {
                return CreateAsync(
                    cached.GetKey(),
                    (options) =>
                    {
                        options.AbsoluteExpiration = cached.AbsoluteExpiration;
                        options.SlidingExpiration = cached.SlidingExpiration;
                        return next();
                    },
                    cts.Token);
            }
        }
        else
        {
            return next();
        }
    }
    private  async Task<T> GetOrCreateAsync<T>(
    string key,
    Func<DistributedCacheEntryOptions, Task<T>> factory,
    CancellationToken cancellationToken)
    {
        var cachedResult = await _cache.GetStringAsync(key, cancellationToken);
        if (cachedResult != null)
        {
            return JsonConvert.DeserializeObject<T>(cachedResult);
        }
        else
        {
            var options = new DistributedCacheEntryOptions();

            // 1. invoke factory method to create new object
            var result = await factory(options);

            if (result == null)
            {
                return default;
            }

            // 2. store the newly created object into cache
            await CreateEntry(key, result, options, cancellationToken);

            return result;
        }
    }

    private async Task<T> CreateAsync<T>(
        string key,
        Func<DistributedCacheEntryOptions, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions();

        // 1. invoke factory method to create new object
        var result = await factory(options);

        if (result == null)
        {
            return default;
        }

        // 2. store the newly created object into cache
        await CreateEntry(key, result, options, cancellationToken);

        return result;
    }

    private Task CreateEntry(string key, object value, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
    {
        var jsonEntry = JsonConvert.SerializeObject(value);

        return _cache.SetStringAsync(key, jsonEntry, options, cancellationToken);
    }

   
}
