using Mirror;

namespace GameResources.CustomSerialization
{
    public struct ResourceVeinStatsData
    {
        public float Health { get; }

        public ResourceVeinStatsData(float health) => 
            Health = health;
    }
    
    public static class ResourceVeinStatsSerializator
    {
        public static void WriteResourceVeinStatsData(this NetworkWriter writer,
            ResourceVeinStatsData data) =>
            writer.WriteFloat(data.Health);

        public static ResourceVeinStatsData ReadResourceVeinStatsData(this NetworkReader reader) => 
            new(reader.ReadFloat());
    }
}