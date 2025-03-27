using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class UGCSound
{
    public string nameMasterSound;
    [JsonIgnore]
    public AudioClip sound;
}

[Serializable]
public class UGCTexture
{
    public string nameMasterTexture;
    [JsonIgnore]
    public Sprite sprite;
}

[Serializable]
public class UGCReciep
{
    public string nameMasterReciep;
    public CraftRecipe craftReciep;
}

[Serializable]
public class UGCPoint
{
    public string nameUGCPoint;
    public bool isHide = false;
    [JsonIgnore]
    public Sprite avatar;
    [HideInInspector] public byte[] avatarBinary;
    public List<UGCSound> ugcSounds = new List<UGCSound>();
    public List<UGCTexture> ugcTextures = new List<UGCTexture>();
    public List<UGCReciep> ugcRecieps = new List<UGCReciep>();
}

[Serializable]
[CreateAssetMenu(fileName = "UGCDatabase", menuName = "UGC/UGCDatabase")]
public class UGCDatabase : ScriptableObject
{
    public List<UGCPoint> ugcPoints = new List<UGCPoint>();
}
