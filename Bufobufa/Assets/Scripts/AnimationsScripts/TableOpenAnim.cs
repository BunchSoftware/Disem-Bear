using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOpenAnim : MonoBehaviour
{
    public Vector3 endCoords = new();
    private Vector3 startCoords = new();
    private float timer = 0f;
    private bool MoveOn = false;

    public void StartMove()
    {
        GetComponent<MovePlayer>().StopMovePlayer();
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
            {
                MoveOn = false;
            }
        }
    }
}
