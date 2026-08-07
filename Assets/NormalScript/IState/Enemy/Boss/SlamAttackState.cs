using UnityEngine;

public class SlamAttackState : IState
{
    private BossController boss;

    private bool isGrounded = false; // 땅에 닿았는지 체크

    public SlamAttackState(BossController boss)
    {
        this.boss = boss;
    }
    public void Enter()
    {

        boss.animator.Play("Boss_Slam_Jump"); // 위로 점프하는 애니메이션

        // 점프 힘 가하기 (공중으로 솟구침)
        boss.Rb.linearVelocity = new Vector2(0f, 10f);
        isGrounded = false;
    }

    public void Update()
    {
        // 떨어지는 중이고, 아직 땅에 닿지 않았을 때 내려찍기 애니메이션으로 전환
        if (boss.Rb.linearVelocity.y < 0 && !isGrounded)
        {
            boss.animator.Play("Boss_Slam_Fall");
        }

        // 땅에 착지했는지 검사 (보통 바닥 레이캐스트나 OnTriggerEnter로 감지)
        if (boss.IsGrounded() && boss.Rb.linearVelocity.y <= 0 && !isGrounded)
        {
            isGrounded = true;
            boss.animator.Play("Boss_Slam_Land"); // 착지 애니메이션 (화면 흔들림 등)

            // 2페이즈: 착지 순간 양옆으로 지진파(투사체 프리팹) 발사!
            if (boss.currentPhase == 2)
            {
                //boss.SpawnShockwave();
            }
        }

        // 착지 애니메이션까지 모두 끝났다면 복귀
        if (isGrounded && boss.isAttackFinished)
        {
            boss.isAttackFinished = false;
            boss.stateMachine.TransitionTo(boss.IdleState);
        }
    }

    public void Exit()
    {

    }
}