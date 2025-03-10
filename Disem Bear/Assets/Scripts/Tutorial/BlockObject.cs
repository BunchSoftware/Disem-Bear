using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject : MonoBehaviour
{
    [SerializeField] private Transform blockObject;

    private Collider blockCollider;

    public void Init()
    {
        blockCollider = GetComponent<Collider>();

        transform.position = blockObject.position;
        transform.rotation = blockObject.rotation;
        transform.localScale = new Vector3(blockObject.localScale.x * 1.01f, blockObject.localScale.y * 1.01f, blockObject.localScale.z * 1.01f);
    }

    public void OnUpdate(float deltaTime)
    {
        transform.position = blockObject.position;
        transform.rotation = blockObject.rotation;
        transform.localScale = new Vector3(blockObject.localScale.x * 1.01f, blockObject.localScale.y * 1.01f, blockObject.localScale.z * 1.01f);
    }

    public void OffOnCollider(bool state)
    {
        blockCollider.enabled = state;
    }
}
