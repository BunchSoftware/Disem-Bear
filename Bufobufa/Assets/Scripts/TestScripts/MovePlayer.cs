using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    //Скрипт движения с помощью MovePosition для TopDown но в 3D
    public float speed = 10f;

    private Rigidbody rb;
    private Vector3 moveVector;
    public bool MoveOn = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.z = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (MoveOn)
        {
            rb.MovePosition(transform.position + moveVector * speed * Time.deltaTime);
        }
    }
    public void StopMovePlayer()
    {
        MoveOn = false;
    }
    public void ReturnMovePlayer()
    {
        MoveOn = true;
    }
}
