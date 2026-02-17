using UnityEngine;
using UnityEngine.Pool;

public class ArrowObject : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float maxLifetime = 5f;

    private IObjectPool<ArrowObject> _pool;
    private float _lifetimeLeft;

    public void SetPool(IObjectPool<ArrowObject> pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        EnsureRigidbodyAndCollider();
    }

    private void OnEnable()
    {
        _lifetimeLeft = maxLifetime;
        gameObject.layer = 8; // Projectile
    }

    private void EnsureRigidbodyAndCollider()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (GetComponent<Collider>() == null)
        {
            var col = gameObject.AddComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = 0.1f;
            col.height = 0.5f;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        _lifetimeLeft -= Time.deltaTime;
        if (_lifetimeLeft <= 0f)
            ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_pool != null)
            _pool.Release(this);
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamageAndReturn(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamageAndReturn(collision.gameObject);
    }

    private void TryDamageAndReturn(GameObject other)
    {
        var controller = other.GetComponent<PlayerCharacterController>()
            ?? other.GetComponentInParent<PlayerCharacterController>();
        if (controller == null || controller.IsDead) return;

        controller.TakeDamage(Mathf.RoundToInt(damage));
        ReturnToPool();
    }
}
