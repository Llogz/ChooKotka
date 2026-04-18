using System.Collections.Generic;

namespace Game.System.Services.AI
{
    public struct ConnectionInfo
    {
        public float Distance { get; }
        public IConnectedObject NextObject { get; }
        
        public ConnectionInfo(IConnectedObject nextObject, float distance)
        {
            NextObject = nextObject;
            Distance = distance;
        }
    }
    
    public interface IConnectedObject
    {
        List<ConnectionInfo> Connections { get; }
    }
}