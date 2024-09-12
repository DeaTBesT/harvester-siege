using Mirror;

namespace Player.CustomSerialization
{
    public struct PlayerControllerData
    {
        public bool IsEnableEntityStats { get; }
        public bool IsEnableEntityMovementController { get; }
        public bool IsEnableEntityWeaponController { get; }
        public bool IsEnableCollider { get; }
        public bool IsEnableGraphics { get; }

        public PlayerControllerData(bool isEnableEntityStats,
            bool isEnableEntityMovementController,
            bool isEnableEntityWeaponController,
            bool isEnableCollider,
            bool isEnableGraphic)

        {
            IsEnableEntityStats = isEnableEntityStats;
            IsEnableEntityMovementController = isEnableEntityMovementController;
            IsEnableEntityWeaponController = isEnableEntityWeaponController;
            IsEnableCollider = isEnableCollider;
            IsEnableGraphics = isEnableGraphic;
        }
    }

    public static class PlayerControllerSerializator
    {
        public static void WritePlayerControllerData(this NetworkWriter writer,
            PlayerControllerData data)
        {
            writer.WriteBool(data.IsEnableEntityStats);
            writer.WriteBool(data.IsEnableEntityMovementController);
            writer.WriteBool(data.IsEnableEntityWeaponController);
            writer.WriteBool(data.IsEnableCollider);
            writer.WriteBool(data.IsEnableGraphics);
        }

        public static PlayerControllerData ReadPlayerControllerData(this NetworkReader reader)
        {
            return new PlayerControllerData(reader.ReadBool(), 
                reader.ReadBool(),
                reader.ReadBool(),
                reader.ReadBool(),
                reader.ReadBool());
        }
    }
}