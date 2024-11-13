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
            ResourceVeinStatsData statsData) =>
            writer.WriteFloat(statsData.Health);

        public static ResourceVeinStatsData ReadResourceVeinStatsData(this NetworkReader reader) => 
            new(reader.ReadFloat());
    }
}