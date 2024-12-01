using API;
using System;

public class SaveManagerIO : DataStream
{
    public void SaveJSONPlayer(string pathSaveFile, JSONPlayer jsonPlayer)
    {
        if (jsonPlayer != null)
            base.Serialize(pathSaveFile, jsonPlayer);
        else
            throw new Exception("Сохранения пусты");
    }
    public JSONPlayer LoadJSONPlayer(string pathSaveFile)
    {
        return base.Deserialize<JSONPlayer>(pathSaveFile);
    }
}
