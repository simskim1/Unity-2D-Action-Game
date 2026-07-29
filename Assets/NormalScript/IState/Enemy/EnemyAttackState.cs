using UnityEngine;

public class EnemyAttackState : IState
{
    private EnemyController enemy;

    // 데미지가 이미 들어갔는지 체크하는 변수 (다단 히트 방지)
    private bool hasDealtDamage;
    // 애니메이션 시작 후 몇 초 뒤에 타격 판정을 할지
    private float attackImpactTime = 0.15f;
    private float comboCheck;
    public EnemyAttackState(EnemyController enemy)
    {
        this.enemy = enemy;

    }

    public void Enter()
    {
        Debug.Log("공격 실행");
        hasDealtDamage = false; // 타격 초기화

        // 공격할 땐 미끄러지지 않게 제자리에 멈춤 (필요시 앞으로 살짝 전진하는 속도를 줘도 됨)
        enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);

        // comboStep에 맞춰 Attack1, Attack2, Attack3 애니메이션 재생
        enemy.Animator.Play("Enemy_Attack");

        enemy.lastAttackTime = Time.time;
    }

    public void Update()
    {
        comboCheck += Time.deltaTime;
        if (!hasDealtDamage && comboCheck >= attackImpactTime)
        {
            enemy.PerformAttack(1);

            hasDealtDamage = true; // 플래그를 켜서 이번 공격에서 더 이상 데미지가 안 들어가게 막음
        }

    }

    public void Exit()
    {
        // 상태를 빠져나갈 때 콤보 창을 닫아줌
    }
}