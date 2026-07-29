using UnityEngine;

public class PlayerGuardBreakState : IState
{
    private PlayerController player;

    // 가드 브레이크는 치명적인 페널티이므로 경직 시간을 길게(예: 1.5초) 설정
    private float breakDuration = 1.5f;
    private float currentTimer;

    public PlayerGuardBreakState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // 헉! 하고 방어 자세가 풀리며 헐떡이는 애니메이션 재생
        player.Animator.Play("Player_GuardBreak");
        currentTimer = 0f;

        // 가드가 깨졌으므로 제자리에 멈춰 세움
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocity.y);
    }

    public void Update()
    {
        currentTimer += Time.deltaTime;

        if (currentTimer >= breakDuration)
        {
            player.StateMachine.TransitionTo(player.IdleState);
        }
    }

    public void Exit()
    {
        // 가드 브레이크가 끝났을 때 스태미나 회복 로직이 재가동되도록 연동할 수 있음
    }
}