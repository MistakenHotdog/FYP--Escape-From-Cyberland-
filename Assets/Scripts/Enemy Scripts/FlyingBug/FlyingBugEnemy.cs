using UnityEngine;

public class FlyingBugEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Hit Effect")]
    public float slowAmount = 0.35f;
    public float effectDuration = 1.2f;

    private PlayerEffectsController effectsController;

    private Transform player;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Find effects controller in the scene
        effectsController = FindObjectOfType<PlayerEffectsController>();
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.LookAt(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Trigger player effects
        if (effectsController != null)
            effectsController.ApplyHitEffect(slowAmount, effectDuration);

        // Destroy enemy
        Destroy(gameObject);
    }
}
