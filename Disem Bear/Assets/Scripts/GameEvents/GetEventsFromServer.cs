using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using External.Storage;
using Newtonsoft.Json;
using UI.PlaneTablet.Shop;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;

public class GetEventsFromServer : MonoBehaviour
{
    private string baseUrl = "https://final.2025.nti-gamedev.ru/api/games/";
    private string uuid = "2c67ad27-3075-409c-bb4d-6e0e079daf80";
    private string events = "/events/";
    private string eventUrl;

    private EventsManager eventsManager;

    private float timer = 0f;
    [SerializeField] private Responce updateEvents;
    [SerializeField] private RainbowUI rainbowUI;
    private bool existResponce = false;
    private Responce currentResponce;


    public void Init(EventsManager eventsManager)
    {
        this.eventsManager = eventsManager;
        rainbowUI.Init(eventsManager);
        eventUrl = baseUrl + uuid + events;
        //StartCoroutine(PostRequest());
        StartCoroutine(GetRequest());
        //StartCoroutine(RemoveAll());
    }

    public void OnUpdate(float deltaTime)
    {
        timer += Time.deltaTime;
        if (timer > 180f && CheckInternetConnection("final.2025.nti-gamedev.ru"))
        {
            timer = 0f;
            StartCoroutine(GetRequest());
            existResponce = true;
        }
        else if (existResponce)
        {
            eventsManager.GetEventsData(currentResponce);
        }
    }

    private IEnumerator RemoveAll()
    {
        UnityWebRequest request = UnityWebRequest.Get(eventUrl);

        yield return request.SendWebRequest();

        string json = "{\"posts\":" + request.downloadHandler.text + "}";

        Responce responce = JsonUtility.FromJson<Responce>(json);

        for (int i = 0; i < responce.posts.Count; i++)
        {
            UnityWebRequest deleteRequest = UnityWebRequest.Delete(eventUrl + responce.posts[i].id.ToString() + "/");
            yield return deleteRequest.SendWebRequest();
        }
    }

    private IEnumerator GetRequest()
    {
        UnityWebRequest request = UnityWebRequest.Get(eventUrl);

        yield return request.SendWebRequest();

        string json = "{\"posts\":" + request.downloadHandler.text + "}";

        Responce responce = JsonConvert.DeserializeObject<Responce>(json);
        for(int i = 0; i < responce.posts.Count; i++)
        {
            responce.posts[i].textJson = JsonConvert.DeserializeObject<TextJson>(responce.posts[i].text);
        }

        currentResponce = responce;
        existResponce = true;
        eventsManager.GetEventsData(responce);
    }

    private IEnumerator PostRequest()
    {
        for (int i = 0; i < updateEvents.posts.Count; i++)
        {
            WWWForm formData = new WWWForm();
            updateEvents.posts[i].text = JsonConvert.SerializeObject(updateEvents.posts[i].textJson);
            string json = JsonConvert.SerializeObject(updateEvents.posts[i]);
            //Debug.Log(json);
            UnityWebRequest request = UnityWebRequest.Post(eventUrl, formData);

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            UploadHandler uploadHandler = new UploadHandlerRaw(postBytes);

            request.uploadHandler = uploadHandler;
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
        }
    }

    public bool CheckInternetConnection(string nameOrAddress)
    {
        try
        {
            using (System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping())
            {
                PingReply reply = pinger.Send(nameOrAddress);
                //return false;
                return reply.Status == IPStatus.Success;
            }
        }
        catch (Exception)
        {
            Debug.LogWarning("Нет подключения к серверу или нет интернета !");
            return false;
        }
    }
}

[System.Serializable]
public class TextJson
{
    public int indexProduct; //индекс продукта который добавиться в магазин если ивент магазины
    public int indexMod; //индекс мода который будет использоваться если ивент сезонный
}

[System.Serializable]
public class EventStruct
{
    public int id;
    public string name;
    [JsonIgnore]
    public TextJson textJson;
    public string text;
    public float hoursUntilNext;
    public int once_in_hours;
    public int duration_in_minutes;
    public string start_date_time;
}

[System.Serializable]
public class Responce
{
    public List<EventStruct> posts;
}

