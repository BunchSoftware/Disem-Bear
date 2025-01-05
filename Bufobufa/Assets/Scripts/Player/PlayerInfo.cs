using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [HideInInspector] public bool playerInSomething = false;
    [HideInInspector] public bool playerPickSometing = false;
    [HideInInspector] public GameObject currentPickObject;

    [SerializeField] private ParticleSystem playerParticleSystem;
    public ParticleSystem PlayerParticleSystem => playerParticleSystem;

    [SerializeField] private GameObject pointItemLeft;
    public GameObject PointItemLeft => pointItemLeft;

    [SerializeField] private GameObject pointItemRight;
    public GameObject PointItemRight => pointItemRight;

    [SerializeField] private GameObject pointItemBack;
    public GameObject PointItemBack => pointItemBack;

    [SerializeField] private GameObject pointItemForward;
    public GameObject PointItemForward => pointItemForward;
}
