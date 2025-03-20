using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using External.DI;

public class MoveCamera : MonoBehaviour
{
    private bool isMove = false;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private GameBootstrap gameBootstrap;

    public bool StartMoveTo(CameraMoveToPosition cameraMoveToPosition)
    {
        if (!isMove)
        {
            gameBootstrap.OnPlayOneShotSound(audioClip);
            isMove = true;
            transform.DOMove(cameraMoveToPosition.position, cameraMoveToPosition.time).SetEase(Ease.Linear);
            transform.DORotate(cameraMoveToPosition.eulerAngles, cameraMoveToPosition.time).SetEase(Ease.Linear);
            StartCoroutine(WaitMoveCamera(cameraMoveToPosition.time));
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator WaitMoveCamera(float time)
    {
        yield return new WaitForSeconds(time);
        isMove = false;
    }

    public bool IsMove()
    {
        return isMove;
    }
}
