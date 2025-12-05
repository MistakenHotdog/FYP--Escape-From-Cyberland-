using UnityEngine;

public class IdleState : IState
{
    private EnemyAI enemy;
    private float timer;
    private float idleDuration;

    public IdleState(EnemyAI enemy, float idleDuration)
    {
        this.enemy = enemy;
        this.idleDuration = idleDuration;
    }

    public void Enter()
    {
        timer = 0f;
        enemy.Agent.isStopped = true;

        enemy.animator.ResetTrigger("Walk");
        enemy.animator.ResetTrigger("Run");
        enemy.animator.SetTrigger("Idle");
    }

    public void Tick()
    {
        timer += Time.deltaTime;

        var player = enemy.GetPlayer();
        if (Vector3.Distance(enemy.transform.position, player.position) <= enemy.chaseDistance)
        {
            enemy.StateMachine.ChangeState(enemy.chaseState);
            return;
        }

        if (timer >= idleDuration)
        {
            enemy.StateMachine.ChangeState(enemy.patrolState);
            return;
        }
    }

    public void Exit()
    {
        enemy.Agent.isStopped = false;
    }
}
