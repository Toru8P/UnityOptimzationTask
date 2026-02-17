using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerCharacterController : MonoBehaviour
{
    public event UnityAction<int> onTakeDamageEventAction;
    public event UnityAction onDeathEventAction;
    [SerializeField] private UnityEvent<int> onTakeDamageEvent;
    [SerializeField] private UnityEvent onDeathEvent;

    [Header("Navigation")]
    [SerializeField] private Transform[] pathWaypoints;
    [SerializeField] private bool loopPath = true;
    [SerializeField] private Transform finishPoint;

    [Header("Animation")]
    [SerializeField] private string deathTriggerName = "Death";

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Camera _mainCamera;
    private Collider _collider;

    private bool _isMoving = true;
    private bool _isDead;
    private int _currentWaypointIndex = 0;
    private bool _goingToFinish;
    private bool _hasBloodyBoots = true;
    private int _hp;
    private int _startingHp;

    public bool IsDead => _isDead;

    public int Hp { get => _hp; set => _hp = value; }
    public int CurrentWaypointIndex { get => _currentWaypointIndex; set => _currentWaypointIndex = value; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _mainCamera = Camera.main;
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _hp = 100;
        _startingHp = _hp;
        SetMudAreaCost();
        ToggleMoving(true);
        if (pathWaypoints != null && pathWaypoints.Length > 0)
            SetDestination(pathWaypoints[0]);
    }

    public void ToggleMoving(bool shouldMove)
    {
        _isMoving = shouldMove;
        if (_navMeshAgent != null)
            _navMeshAgent.enabled = shouldMove;
    }

    public void SetDestination(Transform targetTransformWaypoint)
    {
        if (_navMeshAgent != null && _navMeshAgent.isActiveAndEnabled)
            _navMeshAgent.SetDestination(targetTransformWaypoint.position);
    }

    public void SetDestination(int waypointIndex)
    {
        if (pathWaypoints != null && waypointIndex >= 0 && waypointIndex < pathWaypoints.Length)
            SetDestination(pathWaypoints[waypointIndex]);
    }

    public void TakeDamage(int damageAmount)
    {
        if (_isDead) return;

        _hp -= damageAmount;
        _hp = Mathf.Clamp(_hp, 0, _startingHp);

        float hpPercentLeft = _startingHp > 0 ? (float)_hp / _startingHp : 0f;
        if (_animator != null)
            _animator.SetLayerWeight(1, 1f - hpPercentLeft);

        onTakeDamageEvent?.Invoke(_hp);
        onTakeDamageEventAction?.Invoke(_hp);

        if (_hp <= 0)
            Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        ToggleMoving(false);
        if (_collider != null)
            _collider.enabled = false;

        if (_animator != null && !string.IsNullOrEmpty(deathTriggerName))
            _animator.SetTrigger(deathTriggerName);

        onDeathEvent?.Invoke();
        onDeathEventAction?.Invoke();
    }

    private void SetMudAreaCost()
    {
        if (_hasBloodyBoots && _navMeshAgent != null)
            _navMeshAgent.SetAreaCost(3, 1);
    }

    public void OnSkillUsed(int skillIndex)
    {
        if (_isDead) return;
        int heal = skillIndex switch { 0 => 10, 1 => 15, 2 => 20, _ => 10 };
        TakeDamage(-heal);
    }

    [ContextMenu("Take Damage Test")]
    private void TakeDamageTesting()
    {
        TakeDamage(10);
    }

    private void Update()
    {
        if (_isDead || _navMeshAgent == null) return;

        if (_isMoving && !_navMeshAgent.isStopped && _navMeshAgent.remainingDistance <= 0.1f)
        {
            if (_goingToFinish)
            {
                ToggleMoving(false);
                _goingToFinish = false;
                return;
            }

            _currentWaypointIndex++;
            if (pathWaypoints == null || _currentWaypointIndex >= pathWaypoints.Length)
            {
                if (loopPath)
                {
                    _currentWaypointIndex = 0;
                    SetDestination(pathWaypoints[_currentWaypointIndex]);
                }
                else
                {
                    if (finishPoint != null)
                    {
                        _goingToFinish = true;
                        _navMeshAgent.SetDestination(finishPoint.position);
                    }
                    else
                        ToggleMoving(false);
                }
            }
            else
                SetDestination(pathWaypoints[_currentWaypointIndex]);
        }

        if (_animator != null)
            _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);

    }
}