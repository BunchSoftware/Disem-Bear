using System.Collections;
using External.DI;
using Game.Environment;
using Game.LPlayer;
using UnityEngine;
using UnityEngine.Events;

public class TV : MonoBehaviour, IUpdateListener
{
    [SerializeField] private TriggerObject triggerObject;
    public UnityEvent OnTVOpen;
    public UnityEvent OnTVClose;
    [SerializeField] private AudioClip TVOn;
    [SerializeField] private AudioClip TVOff;
    private GameBootstrap gameBootstrap;


    private OpenObject openObject;

    public void Init(PlayerMouseMove playerMouseMove, Player player, GameBootstrap gameBootstrap)
    {
        this.gameBootstrap = gameBootstrap;
        openObject = GetComponent<OpenObject>();
        openObject.OnEndObjectOpen.AddListener(() =>
        {
            StartCoroutine(TVSound());
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

    IEnumerator TVSound()
    {
        yield return new WaitForSeconds(34f / 60f);
        gameBootstrap.OnPlayOneShotSound(TVOn);
    }

    public void OnUpdate(float deltaTime)
    {
        if (openObject != null)
            openObject.OnUpdate(deltaTime);
    }
}
