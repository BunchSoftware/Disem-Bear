using API;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ClientHandler))]
public class SaveManager : MonoBehaviour
{
    private ClientHandler clientHandler;
    private SaveManagerIO saveManagerIO;

    public FilePlayer filePlayer;
    public FileShop fileShop;

    private string pathToFileResourcePlayer;
    private string pathToFileResourceShop;

    private void Awake()
    {
        clientHandler = GetComponent<ClientHandler>();
        saveManagerIO = new SaveManagerIO();

        pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
        pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";

        if (filePlayer.JSONPlayer.nameUser == "")
            filePlayer.JSONPlayer = saveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

        if(filePlayer.JSONPlayer == null)
        {
            filePlayer.JSONPlayer = new JSONPlayer();
            filePlayer.JSONPlayer.resources = new ResourcePlayer();
            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
        }

        if (fileShop.JSONShop.nameShop == "")
            fileShop.JSONShop = saveManagerIO.LoadJSONShop(pathToFileResourceShop);

        if (fileShop.JSONShop == null)
        {
            fileShop.JSONShop = new JSONShop();
            fileShop.JSONShop.resources = new ResourceShop();
            saveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
        }
    }

    public void RegistrationShop(string nameShop)
    {
        if (filePlayer.JSONPlayer.nameUser != "")
        {
            fileShop.JSONShop.nameShop = nameShop;
            if (fileShop.JSONShop.resources == null)
                fileShop.JSONShop.resources = new ResourceShop();

            fileShop.JSONShop.resources.isShopRegistration = true;

            clientHandler.RegistrationShop(filePlayer.JSONPlayer.nameUser, nameShop, fileShop.JSONShop.resources);
            clientHandler.SetResourceShopPlayer(filePlayer.JSONPlayer.nameUser, nameShop, fileShop.JSONShop.resources);
            
            ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
            Dictionary<string, string> changedResources = new Dictionary<string, string>();
            changedResources.Add("changedRegistration", filePlayer.JSONPlayer.resources.isPlayerRegistration.ToString());

            if (fileShop.JSONShop.resources.productSaves != null)
            {
                for (int i = 0; i < fileShop.JSONShop.resources.productSaves.Count; i++)
                {
                    changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].countChangeProduct.ToString());
                }
            }

            resourceChangedShop.changedResources = changedResources;

            clientHandler.CreateLogShop(filePlayer.JSONPlayer.nameUser, nameShop, "Магазин был иницилизирован", resourceChangedShop);
            PrintShopInfo();

            saveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
        }
    }

    public void RegistrationPlayer(string nameUser)
    {
        filePlayer.JSONPlayer.nameUser = nameUser;
        if(filePlayer.JSONPlayer.resources == null)
            filePlayer.JSONPlayer.resources = new ResourcePlayer();

        filePlayer.JSONPlayer.resources.isPlayerRegistration = true;


        clientHandler.RegistrationPlayer(nameUser, filePlayer.JSONPlayer.resources);
        clientHandler.SetResourcePlayer(nameUser, filePlayer.JSONPlayer.resources);
        ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
        Dictionary<string, string> changedResources = new Dictionary<string, string>
        {
            { "changedRegistration", fileShop.JSONShop.resources.isShopRegistration.ToString() }
        };
        resourceChangedPlayer.changedResources = changedResources;

        clientHandler.CreateLogPlayer(nameUser, "Игрок был иницилизирован", resourceChangedPlayer);
        PrintListPlayer();
        PrintListLogPlayer(nameUser);

        saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);

    }

    public void UpdateShopFile()
    {
        if (filePlayer.JSONPlayer.nameUser != "" && fileShop.JSONShop.nameShop != "")
        {
            clientHandler.SetResourceShopPlayer(filePlayer.JSONPlayer.nameUser, fileShop.JSONShop.nameShop, fileShop.JSONShop.resources);

            ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
            Dictionary<string, string> changedResources = new Dictionary<string, string>();

            if (fileShop.JSONShop.resources.productSaves != null)
            {
                for (int i = 0; i < fileShop.JSONShop.resources.productSaves.Count; i++)
                {
                    changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].countChangeProduct.ToString());
                }
            }

            resourceChangedShop.changedResources = changedResources;

            clientHandler.CreateLogShop(filePlayer.JSONPlayer.nameUser, fileShop.JSONShop.nameShop, "Данные магазина были изменены", resourceChangedShop);
            PrintShopInfo();

            saveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
        }
    }

    public void UpdatePlayerFile()
    {
        if (filePlayer.JSONPlayer.nameUser != "")
        {
            clientHandler.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);
            ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
            Dictionary<string, string> changedResources = new Dictionary<string, string>();

            //if (fileShop.JSONShop.resources.productSaves != null)
            //{
            //    for (int i = 0; i < fileShop.JSONShop.resources.productSaves.Count; i++)
            //    {
            //        changedResources.Add($"changedMoney_Product{i}", fileShop.JSONShop.resources.productSaves[i].money.ToString());
            //        changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].countProduct.ToString());
            //    }
            //}

            clientHandler.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока были изменены", resourceChangedPlayer);
            PrintListPlayer();
            PrintListLogPlayer(filePlayer.JSONPlayer.nameUser);

            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
        }
    }

    public void AddSaveTypeProduct(SaveTypeProduct saveTypeProduct)
    {
        if (filePlayer.JSONPlayer.nameUser != "")
        {
            for (int j = 0; j < filePlayer.JSONPlayer.resources.products.Count; j++)
            {
                if (filePlayer.JSONPlayer.resources.products[j].typeProduct == saveTypeProduct.typeProduct)
                {
                    filePlayer.JSONPlayer.resources.products[j].countProduct += 1;//saveManager.filePlayer.JSONPlayer.resources.products[j].countProduct += 1;
                    UpdatePlayerFile();//saveManager.UpdatePlayerFile();
                    //OnBuyProduct?.Invoke(product);

                    break;
                }
            }

            filePlayer.JSONPlayer.resources.products.Add(new SaveTypeProduct()//saveManager.filePlayer.JSONPlayer.resources.products.Add(new SaveTypeProduct()
            {
                            countProduct = 1,
                            //typeProduct = product.typeChangeProduct
                        });
            clientHandler.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);
            ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
            Dictionary<string, string> changedResources = new Dictionary<string, string>();

            //if (fileShop.JSONShop.resources.productSaves != null)
            //{
            //    for (int i = 0; i < fileShop.JSONShop.resources.productSaves.Count; i++)
            //    {
            //        changedResources.Add($"changedMoney_Product{i}", fileShop.JSONShop.resources.productSaves[i].money.ToString());
            //        changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].countProduct.ToString());
            //    }
            //}

            clientHandler.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока были изменены", resourceChangedPlayer);
            PrintListPlayer();
            PrintListLogPlayer(filePlayer.JSONPlayer.nameUser);

            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
        }
    }

    public void PrintPlayerInfo()
    {
        PrintListPlayer();
    }
    public void PrintShopInfo()
    {
        PrintListShop(filePlayer.JSONPlayer.nameUser);
    }


    private async void PrintListShop(string userName)
    {
        await clientHandler.GetListShopPlayer(userName);
    }

    private async void PrintListLogsShop(string userName, string shopName)
    {
        await clientHandler.GetListLogsShop(userName, shopName);
    }

    private async void PrintListPlayer()
    {
        await clientHandler.GetListPlayers();
    }
    private async void PrintListLogPlayer(string userName)
    {
        await clientHandler.GetListLogsPlayer(userName);
    }

    public async void DeleteListPlayer()
    {
        List<JSONPlayer> jSONPlayers = await clientHandler.GetListPlayers();
        for (int i = 0; i < jSONPlayers.Count; i++)
        {
            clientHandler.DeletePlayer(jSONPlayers[i].nameUser);
        }
    }

    public JSONPlayer GetJSONPlayer()
    {
        return filePlayer.JSONPlayer;
    }
}
