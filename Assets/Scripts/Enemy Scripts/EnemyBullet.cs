using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 25f;
    public float damage = 10f;
    public float lifetime = 3f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 🔥 Use velocity in CURRENT forward direction
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}