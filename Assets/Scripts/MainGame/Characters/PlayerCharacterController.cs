using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MainGame.Characters
{
    public class PlayerCharacterController : MonoBehaviour
    {
        [SerializeField] private UnityEvent<int> onTakeDamageEvent = new UnityEvent<int>();


        [Header("Navigation")] 
        [SerializeField] private Transform[] pathWaypoints;
    
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;


        public int Hp
        {
            get => _hp;
            set => _hp = value;
        }

        public void SubscribeToTakeDamageEvent(UnityAction<int> action)
        {
            onTakeDamageEvent.AddListener(action);
        }
        
        public void UnsubscribeToTakeDamageEvent(UnityAction<int> action)
        {
            onTakeDamageEvent.RemoveListener(action);
        }

        public int CurrentWaypointIndex
        {
            get => _currentWaypointIndex;
            set => _currentWaypointIndex = value;
        }

        private bool _isMoving = true;
        private int _currentWaypointIndex = 0;

        private bool _hasBloodyBoots = true;


        private int _hp;
        private int _startingHp;
        private Camera _camera;
    
        private void Awake()
        {
            _hp = 100;
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _camera = Camera.main;
            _startingHp = _hp;
            SetMudAreaCost();
            ToggleMoving(true);
            SetDestination(pathWaypoints[0]);
        }

        public void ToggleMoving(bool shouldMove)
        {
            _isMoving = shouldMove;
            if (_navMeshAgent) _navMeshAgent.enabled = shouldMove;
        }

        private void SetDestination(Transform targetTransformWaypoint)
        {
            if (_navMeshAgent)
                _navMeshAgent.SetDestination(targetTransformWaypoint.position);
        }

        public void SetDestination(int waypointIndex)
        {
            SetDestination(pathWaypoints[waypointIndex]);
        }

        public void TakeDamage(int damageAmount)
        {
            _hp -= damageAmount;
            float hpPercentLeft = (float) _hp / _startingHp;
            _animator.SetLayerWeight(1, (1 - hpPercentLeft));
            onTakeDamageEvent.Invoke(_hp);
        }

        private void SetMudAreaCost()
        {
            if (_hasBloodyBoots)
            {
                _navMeshAgent.SetAreaCost(3, 1);
            }
        }

        [ContextMenu("Take Damage Test")]
        private void TakeDamageTesting()
        {
            TakeDamage(10);
        }


        private void Update()
        {
            if (_isMoving && !_navMeshAgent.isStopped && _navMeshAgent.remainingDistance <= 0.1f)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= pathWaypoints.Length)
                    _currentWaypointIndex = 0;
                SetDestination(pathWaypoints[_currentWaypointIndex]);
            }

            if (_animator)
                _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
        
            if (_camera != null)
            {
                Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    //We want to know what the mouse is hovering now
                    // Debug.Log($"Hit: {hit.collider.name}");
                }
            }

        }
    
        private void OnEnable()
        {
        
        }

        private void OnDisable()
        {
        
        }
    }
}