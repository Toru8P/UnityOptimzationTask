using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float closedAngleY = 0f;
    [SerializeField] private float openAngleY = -95f;
    [SerializeField] [Min(0.01f)] private float openCloseDuration = 0.5f;

    [Header("Trigger")]
    [SerializeField] private string openTriggerTag = "PlayerCharacter";

    private float _currentAngleY;
    private float _targetAngleY;
    private bool _isOpen;
    private NavMeshObstacle _navObstacle;
    private readonly List<Collider> _doorColliders = new List<Collider>();

    private void Awake()
    {
        if (doorTransform == null)
            doorTransform = transform;

        _navObstacle = GetComponent<NavMeshObstacle>();
        CollectDoorColliders();
        _currentAngleY = closedAngleY;
        _targetAngleY = closedAngleY;
        ApplyAngle(_currentAngleY);
    }

    private void CollectDoorColliders()
    {
        _doorColliders.Clear();
        Collider c = GetComponent<Collider>();
        if (c != null && !c.isTrigger)
            _doorColliders.Add(c);
        if (doorTransform != null)
        {
            foreach (Collider col in doorTransform.GetComponentsInChildren<Collider>(true))
            {
                if (col != null && !col.isTrigger)
                    _doorColliders.Add(col);
            }
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(_currentAngleY, _targetAngleY)) return;

        float step = (openCloseDuration > 0f)
            ? (Mathf.Abs(openAngleY - closedAngleY) / openCloseDuration) * Time.deltaTime
            : Mathf.Abs(openAngleY - closedAngleY);
        _currentAngleY = Mathf.MoveTowards(_currentAngleY, _targetAngleY, step);
        ApplyAngle(_currentAngleY);
    }

    private void ApplyAngle(float angleY)
    {
        if (doorTransform == null) return;
        Vector3 euler = doorTransform.localEulerAngles;
        doorTransform.localEulerAngles = new Vector3(euler.x, angleY, euler.z);
    }

    public void Open()
    {
        _targetAngleY = openAngleY;
        _isOpen = true;
        if (_navObstacle != null)
            _navObstacle.enabled = false;
        for (int i = 0; i < _doorColliders.Count; i++)
        {
            if (_doorColliders[i] != null)
                _doorColliders[i].enabled = false;
        }
        StartCoroutine(RefreshAgentPathNextFrame());
    }

    public void Close()
    {
        _targetAngleY = closedAngleY;
        _isOpen = false;
        if (_navObstacle != null)
            _navObstacle.enabled = true;
        for (int i = 0; i < _doorColliders.Count; i++)
        {
            if (_doorColliders[i] != null)
                _doorColliders[i].enabled = true;
        }
    }

    private IEnumerator RefreshAgentPathNextFrame()
    {
        yield return null;
        if (string.IsNullOrEmpty(openTriggerTag)) yield break;
        GameObject player = GameObject.FindWithTag(openTriggerTag);
        if (player == null) yield break;
        var playerController = player.GetComponent<PlayerCharacterController>();
        if (playerController != null)
        {
            playerController.RefreshPathToCurrentWaypoint();
            yield break;
        }
        var agent = player.GetComponent<NavMeshAgent>();
        if (agent == null || !agent.isActiveAndEnabled) yield break;
        if (agent.hasPath)
        {
            Vector3 dest = agent.destination;
            agent.ResetPath();
            agent.SetDestination(dest);
        }
    }

    public void Toggle()
    {
        if (_isOpen) Close();
        else Open();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(openTriggerTag)) return;
        if (other.CompareTag(openTriggerTag))
            Open();
    }
}
