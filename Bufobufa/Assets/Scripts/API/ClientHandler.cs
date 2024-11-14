using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    [SerializeField] private string UUID;

    private void Start()
    {
        //RegistrationPlayer("Den4o");
        //SetResourcePlayer("Den4o");
        //GetResourcePlayers("Den4o");
        //GetListPlayers();
        //CreateLogPlayer("Den4o");
        //GetListLogsPlayer("Den4o");
        RegistrationShop("Den4o", "Shop");
        SetResourceShopPlayer("Den4o", "Shop");
        GetResourceShopPlayer("Den4o", "Shop");
        GetListShopPlayer("Den4o");
    }

    public async void GetListPlayers()
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void GetResourcePlayers(string name)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void SetResourcePlayer(string name)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, URL);
        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("resources", "{ \"apples\": 10, \"wheat\": 4 }")
        });
        request.Content = content;
        var response = await client.SendAsync(request);
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void RegistrationPlayer(string name)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", name)
        });
        request.Content = content;
        var response = await client.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            Debug.Log(await response.Content.ReadAsStringAsync());
        }
        catch (Exception)
        {
            Debug.Log($"Персонаж {name} уже зарегистрирован");
        }
    }

    public async void GetListLogsPlayer(string name)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/logs/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void CreateLogPlayer(string name)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("player_name", $"{name}"),
            new KeyValuePair<string, string>("comment", "Completed quest 1"),
            new KeyValuePair<string, string>("resources_changed", "{ \"added_apples\": \"+2\" }")
        });
        request.Content = content;
        var response = await client.SendAsync(request);
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void RegistrationShop(string name, string nameShop)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", nameShop),
            new KeyValuePair<string, string>("resources", "{ \"apples\": 10, \"wheat\": 4 }")
        });
        request.Content = content;
        var response = await client.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            Debug.Log(await response.Content.ReadAsStringAsync());
        }
        catch (Exception exc)
        {
            Debug.Log($"Магазин {nameShop} уже зарегистрирован " + exc.Message);
        }
    }

    public async void GetListShopPlayer(string name)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void GetResourceShopPlayer(string name, string nameShop)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/{nameShop}/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    public async void SetResourceShopPlayer(string name, string nameShop)
    {
        string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/{nameShop}/\r\n";

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, URL);
        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("resources", "{ \"apples\": 10, \"wheat\": 4 }")
        });
        request.Content = content;
        var response = await client.SendAsync(request);
        Debug.Log(await response.Content.ReadAsStringAsync());
    }
}
