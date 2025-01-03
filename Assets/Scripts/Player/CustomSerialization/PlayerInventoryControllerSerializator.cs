using System.Collections.Generic;
using Mirror;

namespace Player.CustomSerialization
{
    public struct PlayerInventoryControllerData
    {
        public List<string> GameResourceNameList { get; }
        public List<int> GameResourceAmountList { get; }

        public PlayerInventoryControllerData(List<string> gameResourceNameList, List<int> gameResourceAmountList)
        {
            GameResourceNameList = gameResourceNameList;
            GameResourceAmountList = gameResourceAmountList;
        }
    }

    public static class PlayerInventoryControllerSerializator
    {
        public static void WritePlayerInventoryControllerData(this NetworkWriter writer,
            PlayerInventoryControllerData data)
        {
            writer.WriteList(data.GameResourceNameList);
            writer.WriteList(data.GameResourceAmountList);
        }

        public static PlayerInventoryControllerData ReadPlayerInventoryControllerData(this NetworkReader reader)
        {
            return new(reader.ReadList<string>(), reader.ReadList<int>());
        }
    }
}