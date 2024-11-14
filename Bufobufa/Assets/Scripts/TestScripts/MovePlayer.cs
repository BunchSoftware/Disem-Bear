using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    //Скрипт движения с помощью MovePosition для TopDown но в 3D
    public float speed = 10f;

    private Rigidbody rb;
    private Vector3 moveVector;

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
        rb.MovePosition(transform.position + moveVector * speed * Time.deltaTime);
    }
}
