using System.Reflection;

namespace SmartMonitoring.Shared.Extensions;

public static class ObjectExtension
{
    /// <summary>
    ///     A generic extension method that aids in reflecting 
    ///     and retrieving any attribute that is applied to an `T`.
    /// </summary>
    public static TAttribute GetAttribute<TAttribute>(this object model) 
        where TAttribute : Attribute
    {
        return model.GetType()
            .GetMember(model.ToString())
            .First()
            .GetCustomAttribute<TAttribute>();
    }
}