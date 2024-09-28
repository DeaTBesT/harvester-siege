using System.Collections.Generic;
using Mirror;

namespace Vehicle.CustomSerialization
{
    public struct InteractableVehicleData
    {
        public List<NetworkIdentity> Interactors { get; }

        public InteractableVehicleData(List<NetworkIdentity> interactors) => 
            Interactors = interactors;
    }
    
    public static class InteractableVehicleSerializator
    {
        public static void WriteInteractableVehicleData(this NetworkWriter writer,
            InteractableVehicleData data)
        {
            writer.WriteList(data.Interactors);
        }

        public static InteractableVehicleData ReadInteractableVehicleData(this NetworkReader reader)
        {
            return new InteractableVehicleData(reader.ReadList<NetworkIdentity>());
        }
    }
}