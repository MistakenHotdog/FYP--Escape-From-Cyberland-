using UnityEngine;

public class AttackState : IState
{
    private EnemyAI enemy;
    private float attackCooldown = 1.0f;
    private float timer = 0f;

    public AttackState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Agent.isStopped = true;
        timer = 0f;

        enemy.animator.ResetTrigger("Run");
        enemy.animator.SetTrigger("Attack");
    }

    public void Tick()
    {
        timer += Time.deltaTime;

        Transform player = enemy.GetPlayer();
        float dist = Vector3.Distance(enemy.transform.position, player.position);

        if (dist > enemy.attackDistance)
        {
            enemy.StateMachine.ChangeState(enemy.chaseState);
            return;
        }

        if (timer >= attackCooldown)
        {
            timer = 0f;

            // Attack animation trigger
            enemy.animator.SetTrigger("Attack");

            // Damage logic here
            Debug.Log("Enemy Attacks Player!");
        }
    }

    public void Exit()
    {
        enemy.Agent.isStopped = false;
    }
}
