using UnityEngine;

public class PlayerRollState : IState
{
    private PlayerController player;

    private float rollDuration = 0.5f;  // 구르기 지속 시간
    private float currentRollTime;      // 현재 굴러간 시간 누적

    // 이동 속도보다 보통 1.5배 ~ 2배 정도 빠르게 설정
    private float rollSpeed = 12f;
    private float rollDirection;        // 구르는 방향 (왼쪽 -1, 오른쪽 1)

    public PlayerRollState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Physics2D.IgnoreLayerCollision(player.PlayerLayer, player.EnemyLayer, true);
        player.Animator.Play("Player_Roll");
        currentRollTime = 0f;

        // 1. 상태 진입 시 플레이어가 바라보는 방향을 고정 (캐싱)
        // transform.localScale.x 가 양수면 오른쪽(1), 음수면 왼쪽(-1)으로 판단
        rollDirection = Mathf.Sign(player.transform.localScale.x);
    }

    public void Update()
    {
        currentRollTime += Time.deltaTime;

        if (currentRollTime >= rollDuration)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            // 좌우 입력이 있으면 Move, 없으면 Idle로 자연스럽게 전환
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                player.StateMachine.TransitionTo(player.MoveState);
            }
            else
            {
                player.StateMachine.TransitionTo(player.IdleState);
            }
            return;
        }

        // 2. 물리 로직: Y축 속도(중력)는 유지하고, X축으로만 구르기 속도 적용
        player.Rb.linearVelocity = new Vector2(rollDirection * rollSpeed, player.Rb.linearVelocity.y);
    }

    public void Exit()
    {
        // 3. 구르기가 끝났을 때 얼음판처럼 미끄러지지 않도록 X축 속도를 0으로 멈춰줌
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocity.y);
        Physics2D.IgnoreLayerCollision(player.PlayerLayer, player.EnemyLayer, false);
    }
}