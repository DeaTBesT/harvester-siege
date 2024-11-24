using System.Collections.Generic;
using Mirror;

namespace CraftingSystem.CustomSerialization
{
    public struct CraftingPlaceData
    {
        public int CraftingState { get; }
        public string SelectedRecipe { get; }
        public List<string> RequiredResourceNameList { get; }
        public List<int> RequiredResourceAmountList { get; }

        public CraftingPlaceData(int craftingState, string selectedRecipe, List<string> requiredResourceNameList, List<int> requiredResourceAmountList)
        {
            CraftingState = craftingState;
            SelectedRecipe = selectedRecipe;
            RequiredResourceNameList = requiredResourceNameList;
            RequiredResourceAmountList = requiredResourceAmountList;
        }
    }

    public static class CraftingPlaceSerializator
    {
        public static void WriteCraftingPlaceData(this NetworkWriter writer, CraftingPlaceData data)
        {
            writer.WriteInt(data.CraftingState);
            writer.WriteString(data.SelectedRecipe);
            writer.WriteList(data.RequiredResourceNameList);
            writer.WriteList(data.RequiredResourceAmountList);
        }

        public static CraftingPlaceData ReadCraftingPlaceData(this NetworkReader reader) => 
            new(reader.ReadInt(), reader.ReadString(), reader.ReadList<string>(), reader.ReadList<int>());
    }
}