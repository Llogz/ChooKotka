using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.System.Services.AI
{
    public class Node : MonoBehaviour, IConnectedObject
    {
        public List<ConnectionInfo> Connections => ConnectionInfos.Keys.ToList();
        public Dictionary<ConnectionInfo, NodeType> ConnectionInfos { get; private set; } = new();
        public float MinDistance => minDistance;
        
        [SerializeField] private List<NodeConnectionInfo> connectedNodesSet;
        [SerializeField] private float minDistance = 0.5f;
        [SerializeField] private Color gizmosColor = new(1f, 1f, 1f, 0.5f);
        [SerializeField] private Color gizmosSelectedColor = Color.green;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            ConnectionInfos.Clear();
            
            foreach (var node in connectedNodesSet)
            {
                var distance = Vector3.Distance(transform.position, node.nextNode.transform.position);
                ConnectionInfos.Add(new ConnectionInfo(node.nextNode, distance), node.nextNodeType);
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            foreach (var node in connectedNodesSet.Where(node => node.nextNode != null)) 
                Gizmos.DrawLine(transform.position, node.nextNode.transform.position);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosSelectedColor;
            foreach (var node in connectedNodesSet.Where(node => node.nextNode != null)) 
                Gizmos.DrawLine(transform.position, node.nextNode.transform.position);
        }

        public KeyValuePair<ConnectionInfo, NodeType> GetCurrentConnection(Node connectedObject)
        {
            return ConnectionInfos
                .First(c => (Node)c.Key.NextObject == connectedObject);
        }
        

        public enum NodeType
        {
            Move,
            Jump
        }
        
        [Serializable]
        private struct NodeConnectionInfo
        {
            public Node nextNode;
            public NodeType nextNodeType;
        }
        
        public static Node FindClosestNode(Node startNode, Vector3 position)
        {
            var graph = Pathfinding.GetAllConnectedObjects(startNode);

            float minDist = float.MaxValue;
            Node closest = null;

            foreach (var obj in graph)
            {
                var node = (Node)obj;

                var dist = Vector3.Distance(node.transform.position, position);

                if (dist >= minDist)
                    continue;

                minDist = dist;
                closest = node;
            }

            return closest;
        }
    }
}
