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
        Debug.Log(enemy.Rb.linearVelocity);

        // 피격 애니메이션 재생
        enemy.Animator.Play("Enemy_Hit", 0, 0f);
        //enemy.Animator.Update(0f);
        enemy.KnockBackSetter(true);
        currentTimer = 0f;
        Debug.Log($"Hit Enter 속도 : {enemy.Rb.linearVelocity}");

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
        enemy.KnockBackSetter(false);
    }
}