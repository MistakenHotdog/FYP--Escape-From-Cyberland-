using UnityEngine;

public class FlyingBugEnemy : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!player) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        transform.LookAt(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage the player
            other.GetComponent<PlayerHealth>().TakeDamage(damage);

            Destroy(gameObject); // bug destroyed after hit
        }
    }
}
