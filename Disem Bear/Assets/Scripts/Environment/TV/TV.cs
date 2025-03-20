using External.DI;
using Game.Environment;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class TV : MonoBehaviour, IUpdateListener
{
    [SerializeField] private TriggerObject triggerObject;
    public UnityEvent OnTVOpen;
    public UnityEvent OnTVClose;
    [SerializeField] private AudioClip TVOn;
    [SerializeField] private AudioClip TVOff;


    private OpenObject openObject;

    public void Init(PlayerMouseMove playerMouseMove, Player player, GameBootstrap gameBootstrap)
    {
        openObject = GetComponent<OpenObject>();
        openObject.OnEndObjectOpen.AddListener(() =>
        {
            gameBootstrap.OnPlayOneShotSound(TVOn);
            OnTVOpen.Invoke();
        });
        openObject.OnStartObjectClose.AddListener(() =>
        {
            gameBootstrap.OnPlayOneShotSound(TVOff);
            OnTVClose.Invoke();
        });

        openObject.Init(triggerObject, playerMouseMove, player);

        Debug.Log("TV: Успешно иницилизирован");
    }


    public void OnUpdate(float deltaTime)
    {
        if (openObject != null)
            openObject.OnUpdate(deltaTime);
    }
}
