using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using External.DI;
using UI;
using UI.PlaneTablet.Exercise;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private UIGameRoot uIGameRoot;
    [SerializeField] private Sprite avatarMail;
    private Exercise baseExerciseMail;
    private ExerciseManager exerciseManager;
    private EventsManager eventsManager;
    private ToastManager toastManager;
    [Header("Нужно ли показывать тост за некоторое время перед ивентом")]
    public bool needToast = true;
    [Header("За сколько времени до начала ивента показать тост")]
    public TimeBefore timeBeforeToast;
    [Header("Нужно ли отправлять письмо за некоторое время перед ивентом")]
    public bool needMail = true;
    [Header("За сколько времени до начала ивента отправить письмо")]
    public TimeBefore timeBeforeMail;

    public List<DataOfEvent> eventsData = new();

    public void Init(ToastManager toastManager, EventsManager eventsManager)
    {
        this.eventsManager = eventsManager;
        this.toastManager = toastManager;
        exerciseManager = uIGameRoot.GetExerciseManager();

        baseExerciseMail = new();
        baseExerciseMail.nameExercise = "eventMail";
        baseExerciseMail.isMail = true;
        baseExerciseMail.avatar = avatarMail;

    }

    public void NoteEvent(DataOfEvent dataOfEvent)
    {
        for (int i = 0; i < eventsData.Count; i++)
        {
            if (eventsData[i].nameEvent == dataOfEvent.nameEvent)
            {
                eventsData[i].timeEventOn = dataOfEvent.timeEventOn;
                return;
            }
        }
        eventsData.Add(dataOfEvent);
    }

    public void OnUpdate(float deltaTime)
    {
        for (int i = 0;  i < eventsData.Count; i++)
        {
            if (eventsData[i].timeEventOn.Subtract(DateTime.UtcNow).TotalSeconds < timeBeforeMail.timeBefore && needMail && eventsData[i].mailWasSend == false)
            {
                baseExerciseMail.header = "Система";

                baseExerciseMail.description = "Поторопись! " + timeBeforeMail.nameSetting + " до события " + eventsData[i].nameEvent;
                exerciseManager.AddExercise(baseExerciseMail);
                toastManager.ShowToast("Вам пришло письмо!");
                eventsData[i].mailWasSend = true;
            }
            if (eventsData[i].timeEventOn.Subtract(DateTime.UtcNow).TotalSeconds < timeBeforeToast.timeBefore && needToast && eventsData[i].toastWasShow == false)
            {
                toastManager.ShowToast(timeBeforeToast.nameSetting + " до события " + eventsData[i].nameEvent);
                eventsData[i].toastWasShow = true;
            }
        }
    }
    private DateTime ServerToReal(string serverTime)
    {
        DateTime dt = DateTime.Parse(serverTime);
        return dt;
    }
}

[Serializable]
public class DataOfEvent
{
    public string nameEvent;
    public DateTime timeEventOn;
    public bool toastWasShow = false;
    public bool mailWasSend = false;
}

[Serializable]
public class TimeBefore
{
    public string nameSetting;
    public float timeBefore;
}
