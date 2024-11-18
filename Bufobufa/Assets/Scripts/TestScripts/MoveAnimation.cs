using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    [Header("Куда полетит объект")]
    public bool needPosition = true;
    private Vector3 endCoords = new();
    public Vector3 startCoords = new();
    [Header("Как повернеться объект")]
    public bool needRotate = false;
    private Quaternion endRotate = new();
    public Quaternion startRotate = new();
    private float timer = 0f;
    public float TimeAnimation = 1f;
    private bool MoveOn = false;

    public void StartMove()
    {
        endCoords = startCoords;
        startCoords = transform.position;
        endRotate = startRotate;
        startRotate = transform.rotation;
        timer = 0f;
        MoveOn = true;
    }
    public void EndMove()
    {
        endCoords = startCoords;
        startCoords = transform.position;
        endRotate = startRotate;
        startRotate = transform.rotation;
        timer = 0f;
        MoveOn = true;
    }

    private void Update()
    {
        if (MoveOn)
        {
            if (timer <= TimeAnimation)
            {
                if (needPosition)
                    transform.position = Vector3.Lerp(startCoords, endCoords, timer / TimeAnimation);
                if (needRotate)
                    transform.rotation = Quaternion.Lerp(startRotate, endRotate, timer/ TimeAnimation);
                timer += Time.deltaTime;
            }
            else
                MoveOn = false;
        }
    }
}
