﻿
using System.Reflection;
using System.Text;

namespace Arch.CQRS.Query;

public class QueryCached<T> : IQuery<T>
{
    /// <summary>
    /// Living time of the cache, Default is one day.
    /// </summary>
    public virtual DateTimeOffset? AbsoluteExpiration => DateTime.Now + TimeSpan.FromDays(1);

    /// <summary>
    /// SlidingExpiration defaults to 1 day.
    /// </summary>
    public virtual TimeSpan? SlidingExpiration => TimeSpan.FromDays(1);

    /// <summary>
    /// When is true, handler will be executed and cache will be updated. (even if the cache was exist, it will be ignored)
    /// </summary>
    public virtual bool ReValidate { get; set; }

    public virtual string GetKey()
    {
        var exNames=new List<string>() { nameof(AbsoluteExpiration), nameof(SlidingExpiration),nameof(ReValidate) };
        var typeInfo = GetType();
        var props = typeInfo
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(t=> !exNames.Any(e=>t.Name.Contains(e)))
            .OrderByDescending(t => t.Name);

        if (!props.Any())
        {
            return typeInfo.FullName;
        }

        var sb = new StringBuilder(typeInfo.FullName);

        foreach (var property in props)
        {
            sb.Append($"&{property.Name}={property.GetValue(this)}");
        }

        return sb.ToString();
    }
}
