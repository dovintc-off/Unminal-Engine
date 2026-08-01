namespace Unminal.UI.EventBus;

public interface IEventUI {}

public struct ButtonPressedEvent : IEventUI {
    public uint ButtonId;
    // Code here
}

public struct ButtonHeldEvent : IEventUI {
    public uint ButtonId;
    public float Duration;
    // Code here
}

public struct ButtonRelesedEvent : IEventUI {
    public uint ButtonId;
    // Code here
}

public static class EventBusUi {
    private static readonly Dictionary<Type, List<Delegate>> _subs = new();
    public static void Subscribe<T>(Action<T> handler) where T : IEventUI {
        Type eventType = typeof(T);
        if (!_subs.ContainsKey(eventType)) 
            _subs[eventType] = new List<Delegate>();
        _subs[eventType].Add(handler);
    }

    public static void Publish<T>(T Event) where T : IEventUI {
        Type eventType = typeof(T);
        if (_subs.TryGetValue(eventType, out var handler)) {
            for (int i = handler.Count - 1; i >= 0; i--) {
                var act = (Action<T>)handler[i];
                act.Invoke(Event);
            }
        }
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : IEventUI {
        Type eventType = typeof(T);
        if (_subs.TryGetValue(eventType, out var handlers)) {
            handlers.Remove(handler);
            if (handlers.Count == 0) _subs.Remove(eventType);
        }
    }
}