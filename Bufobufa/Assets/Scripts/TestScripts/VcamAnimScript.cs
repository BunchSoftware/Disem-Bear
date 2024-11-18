using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VcamAnimScript : MonoBehaviour
{
    public Vector3 endCoords = new();
    public Vector3 startCoords = new();
    private float timer = 0f;
    private bool MoveOn = false;

    public void StartMove()
    {
        endCoords = startCoords;
        startCoords = transform.position;
        timer = 0f;
        MoveOn = true;
    }
    public void EndMove()
    {
        endCoords = startCoords;
        startCoords = transform.position;
        timer = 0f;
        MoveOn = true;
    }

    private void Update()
    {
        if (MoveOn)
        {
            if (timer <= 1f)
            {
                transform.position = Vector3.Lerp(startCoords, endCoords, timer / 1);
                timer += Time.deltaTime;
            }
            else
                MoveOn = false;
        }
    }
}
