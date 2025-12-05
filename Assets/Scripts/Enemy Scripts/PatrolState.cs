using UnityEngine;

public class PatrolState : IState
{
    private EnemyAI enemy;
    private int currentPatrolIndex;
    private float waypointReachDistance = 0.5f;

    public PatrolState(EnemyAI enemy)
    {
        this.enemy = enemy;
        currentPatrolIndex = 0;
    }

    public void Enter()
    {
        enemy.Agent.isStopped = false;
        enemy.Agent.speed = enemy.patrolSpeed;
        enemy.SetWalkAnimation();

        // Set destination to first patrol point
        if (enemy.patrolPoints != null && enemy.patrolPoints.Length > 0)
        {
            SetNextPatrolPoint();
        }
    }

    public void Tick()
    {
        // Check if player is in chase range
        if (enemy.IsPlayerInChaseRange())
        {
            enemy.StateMachine.ChangeState(enemy.chaseState);
            return;
        }

        // Check if reached patrol point
        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= waypointReachDistance)
        {
            // Move to next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % enemy.patrolPoints.Length;
            SetNextPatrolPoint();
        }
    }

    private void SetNextPatrolPoint()
    {
        if (enemy.patrolPoints[currentPatrolIndex] != null)
        {
            enemy.Agent.SetDestination(enemy.patrolPoints[currentPatrolIndex].position);
        }
    }

    public void Exit()
    {
        // Nothing specific needed
    }
}
