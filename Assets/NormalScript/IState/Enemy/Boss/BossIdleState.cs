using UnityEngine;

public class BossIdleState : IState
{
    private BossController boss;

    public BossIdleState(BossController boss)
    {
        this.boss = boss;
    }

    public void Enter()
    {
        // 대기 애니메이션 재생
        boss.animator.Play("Enemy_Idle");
    }

    public void Update()
    {
        float distanceToPlayer = Vector2.Distance(boss.transform.position, boss.PlayerTarget.position);
        if (distanceToPlayer < boss.detectRange)
        {
            boss.stateMachine.TransitionTo(boss.ChaseState);
            return;
        }
    }

    public void Exit()
    {
        // 대기 상태를 벗어날 때 할 일
    }
}