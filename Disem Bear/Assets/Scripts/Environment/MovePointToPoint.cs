using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovePointToPoint : MonoBehaviour
{
    private bool isMove = false;
    private string state = "point1";
    public Transform point1;
    public Transform point2;
    


    public bool StartMoveTo(float time)
    {
        if (!isMove)
        {
            isMove = true;
            ObjectMoveToPosition objectMoveToPosition = new();
            switch (state)
            {
                case "point1":
                    objectMoveToPosition.position = point2.position;
                    objectMoveToPosition.eulerAngles = point2.eulerAngles;
                    state = "point2";
                    break;
                case "point2":
                    objectMoveToPosition.position = point1.position;
                    objectMoveToPosition.eulerAngles = point1.eulerAngles;
                    state = "point1";
                    break;
            }
            objectMoveToPosition.time = time;

            transform.DOMove(objectMoveToPosition.position, objectMoveToPosition.time).SetEase(Ease.Linear);
            transform.DORotate(objectMoveToPosition.eulerAngles, objectMoveToPosition.time).SetEase(Ease.Linear);
            StartCoroutine(WaitMoveObject(objectMoveToPosition.time));
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator WaitMoveObject(float time)
    {
        yield return new WaitForSeconds(time);
        isMove = false;
    }

    public bool IsMove()
    {
        return isMove;
    }
}

public class ObjectMoveToPosition
{
    public float time;
    public Vector3 position;
    public Vector3 eulerAngles;
}

