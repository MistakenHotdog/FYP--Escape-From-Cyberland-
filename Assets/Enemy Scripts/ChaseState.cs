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
        enemy.Agent.speed = 4f;

        enemy.animator.ResetTrigger("Idle");
        enemy.animator.ResetTrigger("Walk");
        enemy.animator.SetTrigger("Run");
    }

    public void Tick()
    {
        var player = enemy.GetPlayer();
        float dist = Vector3.Distance(enemy.transform.position, player.position);

        if (dist > enemy.chaseDistance * 1.1f)
        {
            enemy.StateMachine.ChangeState(enemy.patrolState);
            return;
        }

        if (dist <= enemy.attackDistance)
        {
            enemy.StateMachine.ChangeState(enemy.attackState);
            return;
        }

        enemy.Agent.SetDestination(player.position);
    }

    public void Exit()
    {
        enemy.Agent.speed = 2f;
    }
}
