using UnityEngine;

public class EnemyIdleState : IState
{
    private EnemyController enemy;

    public EnemyIdleState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        // 대기 애니메이션 재생
        enemy.Animator.Play("Enemy_Idle");
    }

    public void Update()
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.PlayerTarget.position);
        if (distanceToPlayer < enemy.detectRange)
        {
            enemy.StateMachine.TransitionTo(enemy.ChaseState);
            return;
        }
    }

    public void Exit()
    {
        // 대기 상태를 벗어날 때 할 일
    }
}