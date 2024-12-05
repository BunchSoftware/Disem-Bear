using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBoard : MonoBehaviour
{
    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();

    private GameObject Player;
    private GameObject Workbench;

    public bool ModelOpen = false;
    private bool InTableAndBoard = false;
    private bool OneTap = true;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Workbench = GameObject.Find("Workbench");
    }
    private void Update()
    {
        if (GetComponent<OpenObject>().ObjectIsOpen && Workbench.GetComponent<OpenObject>().ObjectIsOpen && OneTap)
        {
            OneTap = false;
            Workbench.GetComponent<OpenObject>().ArgumentsNotQuit += 1;
            GetComponent<OpenObject>().ArgumentsNotQuit += 1;
            InTableAndBoard = true;
        }
        if (GetComponent<OpenObject>().ArgumentsNotQuit != 2 && InTableAndBoard && !GetComponent<OpenObject>().ObjectAnim && GetComponent<OpenObject>().ObjectIsOpen && Input.GetMouseButtonDown(1))
        {
            OneTap = true;
            GetComponent<OpenObject>().TriggerObject.SetActive(false);
            GetComponent<OpenObject>().ObjectIsOpen = false;
            GetComponent<OpenObject>().ObjectAnim = true;
            GetComponent<OpenObject>().ClickedMouse = false;

            GetComponent<OpenObject>().Vcam.GetComponent<MoveCameraAnimation>().EndMove();

            StartCoroutine(WaitAnimTable(GetComponent<OpenObject>().Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));
            StartCoroutine(WaitAnimCamera(GetComponent<OpenObject>().Vcam.GetComponent<MoveCameraAnimation>().TimeAnimation + 0.1f));

            GetComponent<BoxCollider>().enabled = true;
            GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
            InTableAndBoard = false;
        }

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
    IEnumerator WaitAnimTable(float f)
    {
        yield return new WaitForSeconds(f);
        GetComponent<OpenObject>().ObjectAnim = false;
        Workbench.GetComponent<OpenObject>().ArgumentsNotQuit -= 1;
    }
    IEnumerator WaitAnimCamera(float f)
    {
        yield return new WaitForSeconds(f);
        Player.GetComponent<PlayerInfo>().PlayerInSomething = false;
    }
}
