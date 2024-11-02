using System;
using System.Collections.Generic;
using XIVSocket.App.EventSystem.Events;
using XIVSocket.App.EventSystem.Listeners;

namespace XIVSocket.App.EventSystem
{
    public class EventManager : IDisposable
    {
        private readonly Dictionary<Type, List<object>> listeners = new();

        public Plugin Plugin { get; }

        public EventManager(Plugin plugin) {
            Plugin = plugin;
        }

        public void Dispose() {
            this.listeners.Clear();
        }

        // Register a listener for a specific event type
        public void RegisterListener<T>(IEventListener<T> listener) where T : GameEvent
        {
            var eventType = typeof(T);
            if (!listeners.ContainsKey(eventType))
            {
                listeners[eventType] = new List<object>();
            }
            listeners[eventType].Add(listener);
        }

        // Unregister a listener for a specific event type
        public void UnregisterListener<T>(IEventListener<T> listener) where T : GameEvent
        {
            var eventType = typeof(T);
            if (listeners.ContainsKey(eventType))
            {
                listeners[eventType].Remove(listener);
                if (listeners[eventType].Count == 0)
                {
                    listeners.Remove(eventType);
                }
            }
        }

        // Dispatch an event to all registered listeners of its type
        public void Dispatch<T>(T gameEvent) where T : GameEvent
        {
            var eventType = typeof(T);
            if (listeners.TryGetValue(eventType, out var specificListeners))
            {
                foreach (IEventListener<T> listener in specificListeners)
                {
                    listener.OnEvent(gameEvent);
                }
            }
        }
    }
}
