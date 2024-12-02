using API;
using System;

public class SaveManagerIO : DataStream
{
    public void SaveJSONPlayer(string pathSaveFile, JSONPlayer jsonPlayer)
    {
        base.Serialize(pathSaveFile, jsonPlayer);
    }
    public JSONPlayer LoadJSONPlayer(string pathSaveFile)
    {
        return base.Deserialize<JSONPlayer>(pathSaveFile);
    }
}
