using API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClientHandler))]
public class SaveManager : MonoBehaviour
{
    private ClientHandler clientHandler;
    private SaveManagerIO saveManagerIO;

    private JSONPlayer JSONPlayer;

    private string pathToFileResourcePlayer;
    private string pathToFileResourceShop;

    private void Awake()
    {
        clientHandler = GetComponent<ClientHandler>();
        saveManagerIO = new SaveManagerIO();

        pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
        pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";

        JSONPlayer = saveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

        if(JSONPlayer == null)
        {
            JSONPlayer = new JSONPlayer();
            JSONPlayer.resources = new ResourcePlayer();
            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, JSONPlayer);
        }
    }


    public void RegistrationPlayer(string name)
    {
        JSONPlayer.name = name;
        if(JSONPlayer.resources == null)
            JSONPlayer.resources = new ResourcePlayer();

        JSONPlayer.resources.isPlayerRegistration = true;

        clientHandler.RegistrationPlayer(name, JSONPlayer.resources);
        clientHandler.SetResourcePlayer(name, JSONPlayer.resources);
        ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
        resourceChangedPlayer.changePlayerRegistration = true;
        clientHandler.CreateLogPlayer(name, "Игрок зарегистрирован впервые, приветствуем нового игрока !", resourceChangedPlayer);
        PrintListPlayer();
        PrintListLogPlayer(name);

        saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, JSONPlayer);

    }

    public void Print()
    {
        if (JSONPlayer != null)
            print(JSONPlayer.name);

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
            clientHandler.DeletePlayer(jSONPlayers[i].name);
        }
    }

    public JSONPlayer GetJSONPlayer()
    {
        return JSONPlayer;
    }
}
