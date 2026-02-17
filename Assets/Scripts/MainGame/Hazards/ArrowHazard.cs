using System;
using UnityEngine;
using UnityEngine.Pool;

public class ArrowHazard : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float shootInterval = 1f;

    private float _shootTimer;
    private IObjectPool<ArrowObject> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<ArrowObject>(
            CreateArrow,
            OnTakeFromPool,
            OnReturnToPool,
            OnDestroyArrow,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 50
        );
    }

    private void Start()
    {
        _shootTimer = shootInterval;
    }

    private void Update()
    {
        if (arrowPrefab == null) return;
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0f)
        {
            _shootTimer = shootInterval;
            SpawnArrow();
        }
    }

    private void SpawnArrow()
    {
        if (arrowPrefab == null) return;
        ArrowObject arrow = _pool.Get();
        if (arrow != null)
        {
            arrow.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            arrow.transform.Rotate(0f, 180f, 0f);
        }
    }

    private ArrowObject CreateArrow()
    {
        GameObject go = Instantiate(arrowPrefab);
        ArrowObject arrow = go.GetComponent<ArrowObject>();
        if (arrow == null)
        {
            Destroy(go);
            throw new InvalidOperationException($"Arrow prefab must have an {nameof(ArrowObject)} component.");
        }
        arrow.SetPool(_pool);
        return arrow;
    }

    private void OnTakeFromPool(ArrowObject arrow)
    {
        arrow.gameObject.SetActive(true);
    }

    private void OnReturnToPool(ArrowObject arrow)
    {
        arrow.gameObject.SetActive(false);
    }

    private void OnDestroyArrow(ArrowObject arrow)
    {
        Destroy(arrow.gameObject);
    }
}
