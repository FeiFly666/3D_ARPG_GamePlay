using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum Event_Type
    {
        Boss_Find_Player,
        Boss_Lose_Player
    }
    public delegate void MyEventHandler(object args);
    private Dictionary<Event_Type, MyEventHandler> _events = new Dictionary<Event_Type, MyEventHandler>();

    public void Subscribe(Event_Type event_Type, MyEventHandler newEvent)
    {
        if(_events.ContainsKey(event_Type))
        {
            _events[event_Type] += newEvent;
        }
        else
        {
            _events.Add(event_Type, newEvent);
        }
    }
    public void UnSubscribe(Event_Type event_type, MyEventHandler theEvent)
    {
        if(!_events.ContainsKey(event_type))
        {
            return;
        }

        _events[event_type] -= theEvent;
        if(_events[event_type] == null)
        {
            _events.Remove(event_type);
        }
    }

    public void Execute(Event_Type event_Type, object args = null)
    {
        if( _events.ContainsKey(event_Type))
        {
            _events[event_Type]?.Invoke(args);
        }
    }

}
