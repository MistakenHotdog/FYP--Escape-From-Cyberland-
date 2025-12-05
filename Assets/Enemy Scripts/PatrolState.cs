using UnityEngine;

public class PatrolState : IState
{
    private EnemyAI enemy;
    private int currentPoint = 0;

    public PatrolState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        if (enemy.patrolPoints.Length == 0) return;

        enemy.animator.ResetTrigger("Idle");
        enemy.animator.ResetTrigger("Run");
        enemy.animator.SetTrigger("Walk");

        enemy.Agent.isStopped = false;
        enemy.Agent.speed = 2f;

        enemy.Agent.SetDestination(enemy.patrolPoints[currentPoint].position);
    }

    public void Tick()
    {
        if (enemy.patrolPoints.Length == 0) return;

        var player = enemy.GetPlayer();

        if (Vector3.Distance(enemy.transform.position, player.position) <= enemy.chaseDistance)
        {
            enemy.StateMachine.ChangeState(enemy.chaseState);
            return;
        }

        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= 0.4f)
        {
            currentPoint = (currentPoint + 1) % enemy.patrolPoints.Length;
            enemy.Agent.SetDestination(enemy.patrolPoints[currentPoint].position);
        }
    }

    public void Exit()
    {
        // Nothing
    }
}
