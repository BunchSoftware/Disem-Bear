using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBoard : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private List<GetItemFromTable> getItemFromTables;
    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();

    private GameObject Player;
    private GameObject Workbench;
    private StoreManager storeManager;

    public bool ModelOpen = false;
    private bool InTableAndBoard = false;
    private bool OneTap = true;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Workbench = GameObject.Find("Workbench");
        storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>();

        //if (saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves != null)
        //{
        //    for (int i = 0; i < saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves.Count; i++)
        //    {
        //        for (int j = 0; j < getItemFromTables.Count; j++)
        //        {
        //            if (getItemFromTables[j].typeItemFromTable == saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves[i].typeModelBoard)
        //            {
        //                GameObject item = Instantiate(getItemFromTables[j].gameObject.GetComponent<PackageInfo>().ItemInPackage);
        //                items.Add(item);
        //                items[items.Count - 1].transform.parent = transform;
        //                items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
        //                items[items.Count - 1].SetActive(true);
        //            }
        //        }
        //    }
        //}
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
            //GetComponent<OpenObject>().TriggerObject.SetActive(false);
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

        if (!GetComponent<OpenObject>().ObjectIsOpen && GetComponent<OpenObject>().InTrigger && GetComponent<OpenObject>().ClickedMouse && Player.GetComponent<PlayerInfo>().playerPickSometing)
        {
            GetComponent<OpenObject>().ClickedMouse = false;
            if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>())
            {
                if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().PackageName == "Document")
                {
                    if (items.Count < points.Count)
                    {
                        if (Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().HaveIngredients)
                        {
                            for (int i = 0; i < Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().amount; i++)
                            {
                                storeManager.AddIngridient(Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().NameIngredient);
                            }
                        }


                        GameObject item = Instantiate(Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<PackageInfo>().ItemInPackage);
                        items.Add(item);
                        items[items.Count - 1].transform.parent = transform;
                        items[items.Count - 1].transform.localPosition = points[items.Count - 1].transform.localPosition;
                        items[items.Count - 1].SetActive(true);
                        Player.GetComponent<PlayerInfo>().playerPickSometing = false;
                        Destroy(Player.GetComponent<PlayerInfo>().currentPickObject);

                        if (saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves == null)
                            saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves = new List<ModelBoardSave>();

                        saveManager.filePlayer.JSONPlayer.resources.modelBoardSaves.Add(new ModelBoardSave()
                        {
                            typeModelBoard = Player.GetComponent<PlayerInfo>().currentPickObject.GetComponent<GetItemFromTable>().typeItemFromTable,
                        });

                        Player.GetComponent<PlayerInfo>().currentPickObject = null;

                        saveManager.filePlayer.JSONPlayer.resources.currentItemFromTableSave = null;
                        saveManager.UpdatePlayerFile();
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
        Player.GetComponent<PlayerInfo>().playerInSomething = false;
    }
}
