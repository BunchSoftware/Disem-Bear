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
    [Header("����� �� ���������� ���� �� ��������� ����� ����� �������")]
    public bool needToast = true;
    [Header("�� ������� ������� �� ������ ������ �������� ����")]
    public TimeBefore timeBeforeToast;
    [Header("����� �� ���������� ������ �� ��������� ����� ����� �������")]
    public bool needMail = true;
    [Header("�� ������� ������� �� ������ ������ ��������� ������")]
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
                baseExerciseMail.header = "�������";

                baseExerciseMail.description = "����������! " + timeBeforeMail.nameSetting + " �� ������� " + eventsData[i].nameEvent;
                exerciseManager.AddExercise(baseExerciseMail);
                toastManager.ShowToast("��� ������ ������!");
                eventsData[i].mailWasSend = true;
            }
            if (eventsData[i].timeEventOn.Subtract(DateTime.UtcNow).TotalSeconds < timeBeforeToast.timeBefore && needToast && eventsData[i].toastWasShow == false)
            {
                toastManager.ShowToast(timeBeforeToast.nameSetting + " �� ������� " + eventsData[i].nameEvent);
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
