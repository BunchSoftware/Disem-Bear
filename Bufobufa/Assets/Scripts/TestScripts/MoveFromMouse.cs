using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromMouse : MonoBehaviour
{
    private GameObject Player;

    private Vector3 startPos;

    private Vector3 mousePosition;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
        Player.GetComponent<MoveAnimation>().startCoords = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).x, Player.transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition).z);
        Player.GetComponent<MoveAnimation>().needPosition = true;
        Player.GetComponent<MoveAnimation>().TimeAnimation = 3f;
        Player.GetComponent<MoveAnimation>().StartMove();
    }
}
