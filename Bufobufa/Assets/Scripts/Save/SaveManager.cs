using API;
using System.Collections;
using System.Collections.Generic;
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

        if (filePlayer.name == "")
            filePlayer.JSONPlayer = saveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

        if(filePlayer.JSONPlayer == null)
        {
            filePlayer.JSONPlayer = new JSONPlayer();
            filePlayer.JSONPlayer.resources = new ResourcePlayer();
            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
        }
    }


    public void RegistrationPlayer(string name)
    {
        filePlayer.JSONPlayer.nameUser = name;
        if(filePlayer.JSONPlayer.resources == null)
            filePlayer.JSONPlayer.resources = new ResourcePlayer();

        filePlayer.JSONPlayer.resources.isPlayerRegistration = true;

        clientHandler.RegistrationPlayer(name, filePlayer.JSONPlayer.resources);
        clientHandler.SetResourcePlayer(name, filePlayer.JSONPlayer.resources);
        ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
        resourceChangedPlayer.changePlayerRegistration = true;
        clientHandler.CreateLogPlayer(name, "Игрок зарегистрирован впервые, приветствуем нового игрока !", resourceChangedPlayer);
        PrintListPlayer();
        PrintListLogPlayer(name);

        saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);

    }

    public void Print()
    {
        if (filePlayer.JSONPlayer != null)
            print(filePlayer.JSONPlayer.nameUser);

        PrintListPlayer();
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
