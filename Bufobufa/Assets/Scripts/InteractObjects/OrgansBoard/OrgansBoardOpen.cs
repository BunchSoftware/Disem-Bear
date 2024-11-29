using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrgansBoardOpen : MonoBehaviour
{
    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();

    private GameObject Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (!GetComponent<OpenObject>().ObjectIsOpen && GetComponent<OpenObject>().InTrigger && GetComponent<OpenObject>().ClickedMouse && Player.GetComponent<PlayerInfo>().PlayerPickSometing)
        {
            GetComponent<OpenObject>().ClickedMouse = false;
            if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>())
            {
                if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().PackageName == "Document")
                {
                    if (items.Count < points.Count)
                    {

                        GameObject item = Instantiate(Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().ItemInPackage);
                        items.Add(item);
                        items[items.Count - 1].transform.parent = transform;
                        items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
                        items[items.Count - 1].SetActive(true);
                        Player.GetComponent<PlayerInfo>().PlayerPickSometing = false;
                        Destroy(Player.GetComponent<PlayerInfo>().currentPickObject);
                        Player.GetComponent<PlayerInfo>().currentPickObject = null;
                    }
                }
            }
        }
    }
}
