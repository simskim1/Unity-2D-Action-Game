using UnityEngine;

public class EnemyHitState : IState
{
    private EnemyController enemy;

    // 피격 경직 시간 (보통 0.2초 ~ 0.5초 사이로 짧게 설정)
    private float hitDuration = 0.4f;
    private float currentTimer;

    public EnemyHitState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Hit Enter");
        // 피격 애니메이션 재생
        enemy.Animator.Play("Enemy_Hit", 0, 0f);
        enemy.Animator.Update(0f);
        currentTimer = 0f;

        // 피격 시 물리 제어: 
        // 하던 행동(이동, 구르기 등)을 강제로 끊고 멈춰 세움.
        // 만약 '넉백(Knockback)'을 주고 싶다면 0f 대신 밀려날 방향의 속도를 넣어주면 돼.
        enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);
    }

    public void Update()
    {
        //currentTimer += Time.deltaTime;

        // 경직 시간이 끝나면 다시 대기 상태로 복귀
        //if (currentTimer >= hitDuration)
        //{
        //    currentTimer = 0;
        //    enemy.StateMachine.TransitionTo(enemy.IdleState);
        //    Debug.Log("HitState Ends");
        //}
    }

    public void Exit()
    {

    }
}