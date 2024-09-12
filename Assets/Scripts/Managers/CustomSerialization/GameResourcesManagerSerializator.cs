using System.Collections.Generic;
using Mirror;

namespace Managers.CustomSerialization
{
    public struct GameResourcesManagerData
    {
        public List<string> GameResourceNameList { get; }
        public List<int> GameResourceAmountList { get; }

        public GameResourcesManagerData(List<string> gameResourceNameList, List<int> gameResourceAmountList)
        {
            GameResourceNameList = gameResourceNameList;
            GameResourceAmountList = gameResourceAmountList;
        }
    }

    public static class GameResourcesManagerSerializator
    {
        public static void WriteGameResourcesManagerData(this NetworkWriter writer,
            GameResourcesManagerData data)
        {
            writer.WriteList(data.GameResourceNameList);
            writer.WriteList(data.GameResourceAmountList);
        }

        public static GameResourcesManagerData ReadGameResourcesManagerData(this NetworkReader reader)
        {
            return new(reader.ReadList<string>(), reader.ReadList<int>());
        }
    }
}