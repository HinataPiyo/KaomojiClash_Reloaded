using System;
using System.Collections.Generic;

public static class EventBus
{
    readonly static Dictionary<Type, object> dictionary = new Dictionary<Type, object>();

    public static void Subscribe<T>(Action<T> action)
    {
        if (dictionary.TryGetValue(typeof(T), out var obj))
        {
            var actions = obj as Action<T>;
            actions += action;
            dictionary[typeof(T)] = actions;
        }
        else
        {
            dictionary.Add(typeof(T), action);
        }
    }

    public static void Unsubscribe<T>(Action<T> action)
    {
        if (dictionary.TryGetValue(typeof(T), out var obj))
        {
            var actions = obj as Action<T>;
            actions -= action;
            if (actions == null)
            {
                dictionary.Remove(typeof(T));
            }
            else
            {
                dictionary[typeof(T)] = actions;
            }
        }
    }

    public static void Publish<T>(T eventData)
    {
        if (dictionary.TryGetValue(typeof(T), out var obj))
        {
            var actions = obj as Action<T>;
            actions?.Invoke(eventData);
        }
    }
}