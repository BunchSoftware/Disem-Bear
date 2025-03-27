using External.API;

namespace External.Storage
{
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

        public void SaveUGCPoint(string pathSaveFile, UGCPoint ugcPoint)
        {
            base.Serialize(pathSaveFile, ugcPoint);
        }

        public UGCPoint LoadUGCPoint(string pathSaveFile)
        {
            return base.Deserialize<UGCPoint>(pathSaveFile);
        }

        public void SaveJSONShop(string pathSaveFile, JSONShop JSONShop)
        {
            base.Serialize(pathSaveFile, JSONShop);
        }
        public JSONShop LoadJSONShop(string pathSaveFile)
        {
            return base.Deserialize<JSONShop>(pathSaveFile);
        }
    }
}
