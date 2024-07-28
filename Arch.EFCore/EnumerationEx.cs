

using Arch.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Arch.EFCore;

public static class EnumerationEx
{
    public static ValueConverter<T, int> IntConverter<T>() where T : Enumeration
   => new ValueConverter<T, int>(v => v.ToInt(), v => Enumeration.FromValue<T>(v));
    public static ValueConverter<T, string> StringConverter<T>() where T : Enumeration
       => new ValueConverter<T, string>(v => v.ToString(), v => Enumeration.FromDisplayName<T>(v));
}
