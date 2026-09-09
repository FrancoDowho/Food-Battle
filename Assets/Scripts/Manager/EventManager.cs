using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventManager : MonoBehaviour
{

   public enum EventsType
    {
        Event_SetDoubleDMGGamemode,
        Event_ResetDefaultGamemode,
        Event_SetSensibleGamemode,
        Event_PlayersJoined,
        Event_PlayersLeft,
        Event_SetChaosGamemode

    }
    public delegate void EventReceiver(params object[] parameters);

    static Dictionary<EventsType, EventReceiver> _events;

    public static void SubscribeToEvent(EventsType EventType, EventReceiver Method)
    {
        if (_events == null)
            _events = new Dictionary<EventsType, EventReceiver>();

        if (!_events.ContainsKey(EventType))
        {
            _events.Add(EventType, null);
        }
        _events[EventType] += Method;
    }

    public static void UnsubscribeToEvent(EventsType EventType, EventReceiver Method)
    {
        if (_events == null) return;

        if (!_events.ContainsKey(EventType)) return;

        _events[EventType] -= Method;
    }

    public static void TriggerEvent(EventsType EventType, params object[] parameters)
    {
        if (_events == null) return;

        if (!_events.ContainsKey(EventType)) return;
        if (_events[EventType] == null) return;
        _events[EventType](parameters);
    }


}
