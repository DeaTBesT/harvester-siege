using Mirror;

namespace GameResources.CustomSerialization
{
    public struct GameResourceData
    {
        public int Amount { get; }

        public GameResourceData(int amount) => 
            Amount = amount;
    }
    
    public static class GameResourceSerializator
    {
        public static void WriteGameResourceData(this NetworkWriter writer,
            GameResourceData data) =>
            writer.WriteInt(data.Amount);

        public static GameResourceData ReadGameResourceData(this NetworkReader reader) => 
            new(reader.ReadInt());
    }
}