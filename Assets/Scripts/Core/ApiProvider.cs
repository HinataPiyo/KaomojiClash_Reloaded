using System.Collections.Generic;
using System;

public static class ApiProvider
{
    private static readonly Dictionary<Type, object> _apis = new();

    public static void Register<T>(T api) => _apis[typeof(T)] = api!;
    public static void Unregister<T>() => _apis.Remove(typeof(T));
    public static T Get<T>()
    {
        if (_apis.TryGetValue(typeof(T), out var api)) return (T)api;

        throw new InvalidOperationException($"{typeof(T).Name} is not registered.");
    }
}