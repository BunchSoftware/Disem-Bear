using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace API
{
    // Данные игрока для передачи на сервер
    [Serializable]
    public class ResourcePlayer
    {
        public int apple;
    }

    [Serializable]
    public class ResourceChangedPlayer
    {
        public int add_apple;
    }

    // Данные магазина игрока для передачи на сервер
    [Serializable]
    public class ResourceShop
    {
        public int apple;
    }

    [Serializable]
    public class ResourceChangedShop
    {
        public int add_apple;
    }


    [Serializable]
    public class JSONPlayer
    {
        public string name;
        public ResourcePlayer resources;
    }

    [Serializable]
    public class JSONErrorLog
    {
        public string Error;
    }

    [Serializable]
    public class JSONError
    {
        public string Error;
    }

    public class ClientHandler : MonoBehaviour
    {
        [SerializeField] private string UUID;

        private async void Start()
        {
            ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
            resourceChangedPlayer.add_apple = 10;
            ResourceShop resourceShop = new ResourceShop();
            resourceShop.apple = 10;
            ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
            resourceChangedShop.add_apple = 10;

            // Пример регистрации игрока и задания ресурсов, если нужно отсутсвие ресурсов, укажите null
            //ResourcePlayer resourcePlayer = new ResourcePlayer();
            //resourcePlayer.apple = 10;
            //RegistrationPlayer("Den4o", resourcePlayer);
            //RegistrationPlayer("Den4o", null);

            // Пример отправки ресурсов игрока на сервер
            //ResourcePlayer resourcePlayer = new ResourcePlayer();
            //resourcePlayer.apple = 10;
            //SetResourcePlayer("Den4o", resourcePlayer);

            // Пример получения ресурсов игрока
            //ResourcePlayer resourcePlayerGet = await GetResourcePlayer("Den4o");

            // Получение списка игроков с их именами и ресурсами
            //List<JSONPlayer> listPlayer = await GetListPlayers();

            // Удаление игрока по имени
            DeletePlayer("Den4o");

            // Создание логов о персонаже для орг. коммитета, указывается ник игрока, комментария к логу и измененные ресурсы у персонажа
            //CreateLogPlayer("Den4o", "123", resourceChangedPlayer);

            // Получение всех логов игрока
            //GetListLogsPlayer("Den4o");

            // Регистрация магазина для игрока
            //RegistrationShop("Den4o", "Shop", resourceShop);
            //SetResourceShopPlayer("Den4o", "Shop", resourceShop);
            //GetResourceShopPlayer("Den4o", "Shop");

            GetListShopPlayer("Den4o");
            //CreateLogShop("Den4o", "Shop", "123", resourceChangedShop);
            //GetListLogsShop("Den4o", "Shop");
            //GetListLogsGame();
        }


        public async void RegistrationPlayer(string userName, ResourcePlayer resourcePlayer)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
                FormUrlEncodedContent content;

                if (resourcePlayer != null)
                {
                    content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("name", userName),
                        new KeyValuePair<string, string>("resources", JsonConvert.SerializeObject(resourcePlayer))
                    });
                }
                else
                {
                    content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("name", userName),
                    });
                }

                request.Content = content;
                var response = await client.SendAsync(request);
                try
                {
                    response.EnsureSuccessStatusCode();
                    Debug.Log(await response.Content.ReadAsStringAsync());
                }
                catch (Exception)
                {
                    Debug.Log($"Персонаж {userName} уже зарегистрирован");
                }
            }
        }

        public async Task<ResourcePlayer> GetResourcePlayer(string userName)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    JSONPlayer JSONPlayer = JsonConvert.DeserializeObject<JSONPlayer>(json);
                    Debug.Log(await response.Content.ReadAsStringAsync());
                    Debug.LogWarning($"Предупреждение об отсутствии ресурсов у игрока {JSONPlayer.name}, возможны ошибки в реализации");
                    return JSONPlayer.resources;
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Игрок {userName} отсутсвует на сервере. Ошибка: {exc.Message}");
                }
            }
            return null;
        }

        public async void SetResourcePlayer(string userName, ResourcePlayer resourcePlayer)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, URL);
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("resources",  JsonConvert.SerializeObject(resourcePlayer))
                });
                request.Content = content;
                var response = await client.SendAsync(request);
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void DeletePlayer(string userName)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, URL);
                var response = await client.SendAsync(request);
                Debug.Log(await response.Content.ReadAsStringAsync());
                Debug.Log($"Игрок {userName} удален с сервера");
            }
        }

        public async Task<List<JSONPlayer>> GetListPlayers()
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                List<JSONPlayer> listPlayers = JsonConvert.DeserializeObject<List<JSONPlayer>>(json);

                for (int i = 0; i < listPlayers.Count; i++)
                {
                    if (listPlayers[i].resources == null)
                        Debug.LogWarning($"Предупреждение об отсутствии ресурсов у игрока {listPlayers[i].name}, возможны ошибки в реализации");
                }

                Debug.Log(await response.Content.ReadAsStringAsync());
                return listPlayers;
            }
            return null;
        }

        public async void CreateLogPlayer(string userName, string comment, ResourceChangedPlayer resourceChangedPlayer)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("player_name", $"{userName}"),
                    new KeyValuePair<string, string>("comment", comment),
                    new KeyValuePair<string, string>("resources_changed", JsonConvert.SerializeObject(resourceChangedPlayer))
                });
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                string json = await response.Content.ReadAsStringAsync();
                JSONErrorLog jsonError = JsonConvert.DeserializeObject<JSONErrorLog>(json);

                if (jsonError != null && jsonError.Error == "Not existing Player")
                {
                    Debug.LogError($"Игрок {userName} отсутсвует на сервере. Ошибка: Not existing Player");
                    return;
                }

                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }


        public async void GetListLogsPlayer(string userName)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/logs/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void RegistrationShop(string userName, string nameShop, ResourceShop resourceShop)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/shops/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("name", nameShop),
                    new KeyValuePair<string, string>("resources", JsonUtility.ToJson(resourceShop))
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
        }

        public async void GetListShopPlayer(string userName)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/shops/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void GetResourceShopPlayer(string userName, string nameShop)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/shops/{nameShop}/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void SetResourceShopPlayer(string name, string nameShop, ResourceShop resourceShop)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/{nameShop}/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, URL);
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("resources", JsonUtility.ToJson(resourceShop))
                });
                request.Content = content;
                var response = await client.SendAsync(request);
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void CreateLogShop(string userName, string shopName, string comment, ResourceChangedShop resourceChangedShop)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("player_name", userName),
                    new KeyValuePair<string, string>("shop_name", shopName),
                    new KeyValuePair<string, string>("comment", comment),
                    new KeyValuePair<string, string>("resources_changed", JsonUtility.ToJson(resourceChangedShop))
                });
                request.Content = content;
                var response = await client.SendAsync(request);
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void GetListLogsShop(string userName, string shopName)
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{userName}/shops/{shopName}/logs/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }

        public async void GetListLogsGame()
        {
            if (UUID.Length != 0)
            {
                string URL = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/\r\n";

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
