using UnityEngine;

public class MeleeAttackState : IState // 기존 상태 기본 클래스 상속
{
    private BossController boss;

    public MeleeAttackState(BossController boss)
    {
        this.boss = boss;
    }

    // 상태에 진입할 때 1번 호출됨
    public void Enter()
    {
        // 페이즈에 따라 다른 애니메이션 실행
        if (boss.currentPhase == 1)
        {
            // 1페이즈: 묵직한 1타 베기
            boss.animator.Play("Boss_Melee_1");
        }
        else
        {
            // 2페이즈: 2연속 베기 + 검기 발사
            boss.animator.Play("Boss_Melee_2");
        }
    }

    public void Update()
    {
        // 주의: 액션 게임에서는 애니메이션이 끝나는 시점을 보통 
        // 'Animation Event'를 통해 체크하여 상태를 전환합니다.
        // 여기서는 예시로 임의의 조건(isAttackFinished)을 넣었습니다.
        if (boss.isAttackFinished)
        {
            boss.isAttackFinished = false;
            boss.stateMachine.TransitionTo(boss.IdleState); // 공격이 끝나면 대기 상태로 복귀
        }
    }

    public void Exit()
    {

    }
}