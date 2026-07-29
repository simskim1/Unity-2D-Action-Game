using UnityEngine;

public class PlayerGuardState : IState
{
    private PlayerController player;
    float rollStaminaCost = 25f;

    public PlayerGuardState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // 이동 애니메이션 재생
        player.Animator.Play("Player_Guard");
    }

    public void Update()
    {
        // 플레이어의 스태미나를 깎는 처리(데미지 처리쪽으로 옮겨 그곳에서 스태미나가 0 이하가 되면 가드브레이크?)

        //방어키를 손에서 땠을떄 처리
        if (Input.GetKeyUp(KeyCode.G))
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
        // 회피 키 입력처리
        if (Input.GetKeyUp(KeyCode.R) && player.Stats.CanUseStamina(rollStaminaCost))
        {
            player.Stats.UseStamina(rollStaminaCost);
            player.StateMachine.TransitionTo(player.RollState);
            return;
        }

        //점프 키 입력 처리
        if (player.jumpBufferCounter > 0f && player.coyoteTimeCounter > 0f && Input.GetKeyUp(KeyCode.Space))
        {
            player.UseJump(); // 점프를 사용했으니 타이머들을 0으로 초기화
            player.StateMachine.TransitionTo(player.JumpState);
            return;
        }
    }

    public void Exit()
    {

    }
}