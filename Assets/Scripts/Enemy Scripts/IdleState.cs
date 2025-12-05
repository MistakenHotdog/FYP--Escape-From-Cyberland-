using UnityEngine;

public class IdleState : IState
{
    private EnemyAI enemy;
    private float idleTime;
    private float idleTimer;

    public IdleState(EnemyAI enemy, float idleTime)
    {
        this.enemy = enemy;
        this.idleTime = idleTime;
    }

    public void Enter()
    {
        enemy.Agent.isStopped = true;
        enemy.SetIdleAnimation();
        idleTimer = 0f;
    }

    public void Tick()
    {
        // Check if player is in chase range
        if (enemy.IsPlayerInChaseRange())
        {
            enemy.StateMachine.ChangeState(enemy.chaseState);
            return;
        }

        // Wait for idle time before patrolling
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime)
        {
            if (enemy.patrolPoints != null && enemy.patrolPoints.Length > 0)
            {
                enemy.StateMachine.ChangeState(enemy.patrolState);
            }
        }
    }

    public void Exit()
    {
        idleTimer = 0f;
    }
}
