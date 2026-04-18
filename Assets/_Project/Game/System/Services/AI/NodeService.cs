using System.Collections.Generic;
using Core.Services;
using UnityEngine;
using VContainer;

namespace Game.System.Services.AI
{
    public interface INodeService
    {
        List<Node> GetAllNodes();
    }
    
    public class NodeService : SceneService, INodeService
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<INodeService>();
        }

        [SerializeField] private List<Node> nodes;
        public List<Node> GetAllNodes() => nodes;
    }
}