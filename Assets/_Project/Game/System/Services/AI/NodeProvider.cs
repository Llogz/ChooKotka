using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Services;
using UnityEngine;
using VContainer;

namespace Game.System.Services.AI
{
    public interface INodeProvider
    {
        void MoveTo(Node node);
        void Stop();
        bool Block { get; set; }
    }
    
    public class NodeProvider : UpdatableBehaviour, INodeProvider
    {
        [SerializeField] private Node currentNode;
        [SerializeField] private bool setPositionToCurrentNode = true;
        
        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private Collider2D coll;
        
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 10f;
        
        [Header("Jumping")]
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float minJumpDuration = 0.3f;

        [Inject] private INodeService _nodeService;
        
        public bool Block { get; set; }
        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        public Node.NodeType CurrentMoveType => _currentPath.Count > 0 ? 
            currentNode.GetCurrentConnection(_currentPath[0]).Value : Node.NodeType.Move;

        public Node CurrentNode => currentNode;
        
        private bool _isMoving = false;
        private List<Node> _currentPath = new();
        private float _moveTime = 0f;
        private Vector2 _startMovePos = Vector2.zero;

        private Node _prevTarget = null; 

        public void MoveTo(Node node)
        {
            if (_isMoving)
            {
                _prevTarget = node;
                return;
            }

            StartMoveTo(node);
        }

        private void StartMoveTo(Node node)
        {
            if (node == null) return;

            var graph = Pathfinding.GetAllConnectedObjects(currentNode);
            var path = Pathfinding.FindPath(graph, currentNode, node);

            if (path == null || path.Length == 0) return;

            _currentPath = path.Cast<Node>().Skip(1).ToList();
            _isMoving = true;
            _moveTime = 0f;
            _startMovePos = transform.position;
            
            _prevTarget = null;
        }

        public void Stop()
        {
            _currentPath.Clear();
            _isMoving = false;
            _prevTarget = null;
        }

        public void Start()
        {
            if (setPositionToCurrentNode && currentNode != null)
                transform.position = currentNode.transform.position;
        }

        public override void GameFixedUpdate(float dt)
        {
            coll.enabled = true;

            if (!_isMoving || _currentPath.Count == 0)
            {
                _moveTime = 0f;

                if (_prevTarget != null) StartMoveTo(_prevTarget);

                return;
            }

            var target = _currentPath[0];
            var isEnd = false;

            switch (CurrentMoveType)
            {
                case Node.NodeType.Move:
                    var dist = Vector3.Distance(transform.position, target.transform.position);
                    var vel = speed * Mathf.Sign(target.transform.position.x - transform.position.x);
                    
                    if (dist < 0.5f) vel *= dist / 0.5f;

                    var res = Mathf.MoveTowards(
                        rg.linearVelocity.x,
                        vel,
                        (vel > rg.linearVelocity.x ? acceleration : deceleration) * dt);

                    rg.linearVelocity = new Vector2(res, rg.linearVelocity.y);

                    isEnd = dist < target.MinDistance;

                    break;

                case Node.NodeType.Jump: // ai bitch ass nigga slop
                    coll.enabled = false;

                    Vector2 jumpStart = _moveTime == 0f ? transform.position : _startMovePos;
                    Vector2 jumpEnd = target.transform.position;

                    var dx = jumpEnd.x - jumpStart.x;
                    var x1 = Mathf.Abs(dx);

                    var duration = Mathf.Max(x1 / jumpSpeed, minJumpDuration);
                    var t = Mathf.Clamp01(_moveTime / duration);

                    var x = t * x1;
                    var y = ProjMath.EasingFunctions.JumpGraph(x, x1, jumpEnd.y - jumpStart.y, jumpHeight);

                    transform.position = new Vector2(
                        jumpStart.x + Mathf.Sign(dx) * x,
                        jumpStart.y + y
                    );

                    rg.linearVelocity = Vector2.zero;

                    if (t >= 1f)
                        isEnd = true;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _moveTime += dt;

            if (!isEnd) return;
            
            currentNode = target;
            _currentPath.RemoveAt(0);

            _moveTime = 0f;
            _startMovePos = transform.position;

            if (_currentPath.Count == 0 && _prevTarget != null)
            {
                StartMoveTo(_prevTarget);
            }
            else if (_currentPath.Count == 0)
            {
                _isMoving = false;
            }
        }

    }
}