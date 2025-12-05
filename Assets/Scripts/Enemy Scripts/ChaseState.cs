using UnityEngine;

public class ChaseState : IState
{
    private EnemyAI enemy;

    public ChaseState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Agent.isStopped = false;
        enemy.Agent.speed = enemy.chaseSpeed;
        enemy.SetRunAnimation();
    }

    public void Tick()
    {
        Transform player = enemy.GetPlayer();
        if (player == null) return;

        // Update destination to player position
        enemy.Agent.SetDestination(player.position);

        // Check if player is in attack range
        if (enemy.IsPlayerInAttackRange())
        {
            enemy.StateMachine.ChangeState(enemy.attackState);
            return;
        }

        // Check if player escaped chase range
        if (!enemy.IsPlayerInChaseRange())
        {
            enemy.StateMachine.ChangeState(enemy.idleState);
            return;
        }
    }

    public void Exit()
    {
        // Nothing specific needed
    }
}
