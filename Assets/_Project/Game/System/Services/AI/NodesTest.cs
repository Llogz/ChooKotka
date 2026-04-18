using System.Collections.Generic;
using UnityEngine;

namespace Game.System.Services.AI
{
    /// <summary>
    /// AI slop just to test pathfinding, to get distances reloaded you have to reopen scene
    /// </summary>
    public class NodesTest : MonoBehaviour
    {
        [SerializeField] private Node graphRoot;
        [SerializeField] private Transform target;

        private readonly List<Node> _path = new();

        public void SetPath()
        {
            _path.Clear();

            var objs = Pathfinding.GetAllConnectedObjects(graphRoot);

            var startNode = GetClosestNode(objs, transform.position);
            var targetNode = GetClosestNode(objs, target.position);

            var path = Pathfinding.FindPath(objs, startNode, targetNode);

            if (path == null)
                return;

            foreach (var obj in path)
                _path.Add((Node)obj);
        }

        private Node GetClosestNode(List<IConnectedObject> objs, Vector3 pos)
        {
            Node closest = null;
            var minDist = float.MaxValue;

            foreach (var obj in objs)
            {
                var node = (Node)obj;

                var dist = Vector3.Distance(node.transform.position, pos);

                if (!(dist < minDist)) continue;
                
                minDist = dist;
                closest = node;
            }

            return closest;
        }

        private void Update()
        {
            SetPath();
        }

        private void OnDrawGizmos()
        {
            if (_path.Count == 0)
                return;

            Gizmos.color = Color.green;

            for (int i = 0; i < _path.Count; i++)
            {
                Gizmos.DrawSphere(_path[i].transform.position, 0.35f);

                if (i < _path.Count - 1)
                {
                    Gizmos.DrawLine(
                        _path[i].transform.position,
                        _path[i + 1].transform.position
                    );
                }
            }
        }
    }
}