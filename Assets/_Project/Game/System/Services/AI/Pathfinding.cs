using System.Collections.Generic;
using System.Linq;

namespace Game.System.Services.AI
{
    /// <summary>
    /// Dijkstra algorithm is being used here
    /// </summary>
    public static class Pathfinding
    {
        public static List<IConnectedObject> GetAllConnectedObjects(IConnectedObject obj)
        {
            var result = new List<IConnectedObject>();
            var visited = new HashSet<IConnectedObject>();
            var queue = new Queue<IConnectedObject>();

            queue.Enqueue(obj);
            visited.Add(obj);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var connection in current.Connections)
                {
                    var next = connection.NextObject;

                    if (!visited.Add(next))
                        continue;

                    queue.Enqueue(next);
                }
            }

            return result;
        }
        
        public static IConnectedObject[] FindPath(
            List<IConnectedObject> objects,
            IConnectedObject start,
            IConnectedObject target)
        {
            var distances = new Dictionary<IConnectedObject, float>();
            var previous = new Dictionary<IConnectedObject, IConnectedObject>();
            var unvisited = new List<IConnectedObject>();

            foreach (var obj in objects)
            {
                distances[obj] = float.MaxValue;
                unvisited.Add(obj);
            }

            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                var current = unvisited.OrderBy(o => distances[o]).First();

                unvisited.Remove(current);

                if (current == target)
                    break;

                foreach (var connection in current.Connections)
                {
                    var neighbor = connection.NextObject;
                    var newDistance = distances[current] + connection.Distance;

                    if (!(newDistance < distances[neighbor])) continue;
                    
                    distances[neighbor] = newDistance;
                    previous[neighbor] = current;
                }
            }

            List<IConnectedObject> path = new();

            var currentPath = target;

            while (currentPath != start)
            {
                path.Add(currentPath);

                if (!previous.TryGetValue(currentPath, out var prev))
                    return null;

                currentPath = prev;
            }

            path.Add(start);

            path.Reverse();

            return path.ToArray();
        }
    }
}