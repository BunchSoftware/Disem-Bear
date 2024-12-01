using API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClientHandler))]
public class SaveManager : MonoBehaviour
{
    private ClientHandler clientHandler;
    private SaveManagerIO saveManagerIO;

    private string pathToFileResourcePlayer;
    private string pathToFileResourceShop;

    private void Start()
    {
        clientHandler = GetComponent<ClientHandler>();
        saveManagerIO = new SaveManagerIO();

        pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
        pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";
    }

    public void RegistrtionPlayer(string name)
    {
        JSONPlayer jsonPlayer = new JSONPlayer();
        jsonPlayer.name = name;
        clientHandler.RegistrationPlayer(name, null);
        saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, jsonPlayer);
    }

    public void Print()
    {
        JSONPlayer jsonPlayer = saveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);
        if (jsonPlayer != null)
            print(jsonPlayer.name);

        PrintListPlayer();
    }

    private async void PrintListPlayer()
    {
        List<JSONPlayer> jSONPlayers = await clientHandler.GetListPlayers();
    }

    public async void DeleteListPlayer()
    {
        List<JSONPlayer> jSONPlayers = await clientHandler.GetListPlayers();
        for (int i = 0; i < jSONPlayers.Count; i++)
        {
            clientHandler.DeletePlayer(jSONPlayers[i].name);
        }
    }
}
