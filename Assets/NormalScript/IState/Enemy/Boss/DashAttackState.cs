using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DashAttackState : IState
{
    private BossController boss;

    private Vector2 dashDirection;

    public float dashSpeed = 15f;
    public float dashDuration = 0.5f; // 돌진 지속 시간
    private float dashTimer;

    public DashAttackState(BossController boss)
    {
        this.boss = boss;
    }
    public void Enter()
    {

        boss.animator.Play("Boss_Dash");

        // 돌진 방향 설정 (플레이어가 있는 쪽을 향해)
        float dirX = (boss.PlayerTarget.position.x - boss.transform.position.x > 0) ? 1f : -1f;
        dashDirection = new Vector2(dirX, 0f);

        dashTimer = dashDuration;

        // 2페이즈 돌진 속도 강화 (선택 사항)
        if (boss.currentPhase == 2) dashSpeed = 20f;
    }

    public void Update() // 물리 이동은 FixedUpdate 사용 권장
    {

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer > 0)
        {
            // 돌진 실행!
            boss.Rb.linearVelocity = dashDirection * dashSpeed;

            // 2페이즈: 돌진하면서 일정 주기마다 바닥에 장판(프리팹) 생성
            /*
            if (boss.currentPhase == 2)
            {
                // DropFireTrail() 함수는 BossController 쪽에 구현해두면 좋습니다.
                boss.DropFireTrail();
            }*/
        }
        else
        {
            // 돌진 종료 시 멈추고 상태 전환
            boss.Rb.linearVelocity = Vector2.zero;
            boss.stateMachine.TransitionTo(boss.IdleState);
        }
    }

    public void Exit()
    {
        
    }
}