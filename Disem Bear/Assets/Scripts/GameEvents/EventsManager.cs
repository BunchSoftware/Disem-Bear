using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using External.DI;
using UI;
using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour, IUpdateListener
{
    [SerializeField] private GetEventsFromServer getEventsFromServer;
    [SerializeField] private NotificationManager notificationManager;
    [SerializeField] private NewProductManager newProductManager;

    private Dictionary<string, EventStruct> currentEvents = new();
    private List<EventStruct> onGoingEvents = new();

    public UnityEvent<EventStruct> onEventBegin = new();
    public UnityEvent<EventStruct> onEventEnd = new();

    private ToastManager toastManager;

    public void Init(ToastManager toastManager)
    {
        this.toastManager = toastManager;
        getEventsFromServer.Init(this);
        notificationManager.Init(toastManager, this);
        newProductManager.Init(this);
    }

    public void OnUpdate(float deltaTime)
    {
        getEventsFromServer.OnUpdate(deltaTime);
        notificationManager.OnUpdate(deltaTime);
    }

    public bool EventOngoing(string eventId)
    {
        return currentEvents.ContainsKey(eventId) && EventOngoing(currentEvents[eventId]);
    }

    public DateTime GetLastBegin(EventStruct ev)
    {
        TimeSpan length = TimeSpan.FromMinutes(ev.duration_in_minutes);
        TimeSpan interval = TimeSpan.FromHours(ev.once_in_hours);
        DateTime eventStart = ServerToReal(ev.start_date_time);
        DateTime timeCurrent = DateTime.UtcNow;
        TimeSpan timeAfterFirstStart = timeCurrent - eventStart;
        if (timeCurrent < eventStart)
            return eventStart;
        int completedTimes = Mathf.FloorToInt((float)timeAfterFirstStart.TotalHours / ev.once_in_hours);
        return eventStart.Add(interval * (completedTimes));
    }

    public bool EventOngoing(EventStruct ev)
    {
        DateTime timeCurrent = DateTime.UtcNow;
        DateTime lastBegin = GetLastBegin(ev);
        TimeSpan length = TimeSpan.FromMinutes(ev.duration_in_minutes);
        return timeCurrent <= lastBegin + length && timeCurrent >= lastBegin;
    }

    public void GetEventsData(Responce responce)
    {

        Dictionary<string, EventStruct> newEvents = responce.posts.ToDictionary(keyy => keyy.name, valuee => valuee);
        foreach (var oldEvent in currentEvents)
        {
            if (!newEvents.ContainsKey(oldEvent.Key))
            {
                onEventEnd.Invoke(oldEvent.Value);
                onGoingEvents.Remove(oldEvent.Value);
            }
        }

        currentEvents = newEvents;

        foreach (var newEvent in currentEvents.Values)
        {
            DataOfEvent dataOfEvent = new()
            {
                nameEvent = newEvent.name,
                timeEventOn = ServerToReal(newEvent.start_date_time)
            };
            notificationManager.NoteEvent(dataOfEvent);
            bool isOnGoing = EventOngoing(newEvent);
            if (isOnGoing && !InOnGo(newEvent))
            {
                onGoingEvents.Add(newEvent);
                onEventBegin.Invoke(newEvent);
                toastManager.ShowToast("Ивент " +  newEvent.name + " начался!");
            }
            if (!isOnGoing && InOnGo(newEvent))
            {
                onGoingEvents.Remove(newEvent);
                onEventEnd.Invoke(newEvent);
                toastManager.ShowToast("Ивент " + newEvent.name + " закончился!");
            }
        }

        //for (int i = 0; i < responce.posts.Count; i++)
        //{
        //    for (int j = 0; j < currentEvents.posts.Count; j++)
        //    {
        //        if (responce.posts[i].name == currentEvents.posts[j].name)
        //        {
        //            if (responce.posts[i].start_date_time != currentEvents.posts[j].start_date_time)
        //            {
        //                for (int k = 0; k < notificationManager.eventsData.Count; k++)
        //                {
        //                    if (responce.posts[i].name == notificationManager.eventsData[k].nameEvent)
        //                    {
        //                        notificationManager.eventsData[k].timeEventOn = ServerToReal(responce.posts[i].start_date_time);
        //                        break;
        //                    }
        //                }
        //                for (int k = 0; k < newProductManager.eventsNewProduct.Count; k++)
        //                {
        //                    if (responce.posts[i].name == newProductManager.eventsNewProduct[k].nameEvent)
        //                    {
        //                        newProductManager.eventsNewProduct[k].startTime = ServerToReal(responce.posts[i].start_date_time);
        //                    }
        //                }
        //            }
        //            if (responce.posts[i].duration_in_minutes != currentEvents.posts[j].duration_in_minutes)
        //            {
        //                for (int k = 0; k < newProductManager.eventsNewProduct.Count; k++)
        //                {
        //                    if (responce.posts[i].name == newProductManager.eventsNewProduct[k].nameEvent)
        //                    {
        //                        newProductManager.eventsNewProduct[k].duration = responce.posts[i].duration_in_minutes;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

    private DateTime ServerToReal(string serverTime)
    {
        DateTime dt = DateTime.Parse(serverTime);
        return dt;
    }

    private bool InOnGo(EventStruct eventStruct)
    {
        for (int i = 0; i < onGoingEvents.Count; i++)
        {
            if (onGoingEvents[i].name == eventStruct.name)
            {
                return true;
            }
        }
        return false;
    }
}
