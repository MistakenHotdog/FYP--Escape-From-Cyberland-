using UnityEngine;

public class AttackState : IState
{
    private EnemyAI enemy;
    private float attackCooldown = 1.5f;
    private float attackTimer;

    public AttackState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Agent.isStopped = true;
        enemy.SetIdleAnimation();
        attackTimer = 0f;
    }

    public void Tick()
    {
        Transform player = enemy.GetPlayer();
        if (player == null) return;

        // Face the player
        Vector3 direction = (player.position - enemy.transform.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                lookRotation,
                Time.deltaTime * 5f
            );
        }

        // Attack cooldown
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            PerformAttack();
            attackTimer = 0f;
        }

        // Check if player moved out of attack range
        if (!enemy.IsPlayerInAttackRange())
        {
            if (enemy.IsPlayerInChaseRange())
            {
                enemy.StateMachine.ChangeState(enemy.chaseState);
            }
            else
            {
                enemy.StateMachine.ChangeState(enemy.idleState);
            }
            return;
        }
    }

    private void PerformAttack()
    {
        enemy.TriggerAttackAnimation();

        // Add your attack logic here
        // Example: Raycast, apply damage, etc.
        Debug.Log("Enemy attacking player!");

        // Optional: Deal damage to player
        // PlayerHealth playerHealth = enemy.GetPlayer().GetComponent<PlayerHealth>();
        // if (playerHealth != null) playerHealth.TakeDamage(10);
    }

    public void Exit()
    {
        attackTimer = 0f;
    }
}