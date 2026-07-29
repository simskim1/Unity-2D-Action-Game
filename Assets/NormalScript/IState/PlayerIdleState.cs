using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerController player;
    float rollStaminaCost = 25f;

    public PlayerIdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // 대기 애니메이션 재생
        player.Animator.Play("Player_Idle");
    }

    public void Update()
    {
        // 이동 키 입력이 감지되면 Move 상태로 전환
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            player.StateMachine.TransitionTo(player.MoveState);
            return;
        }

        // 방어 키 입력 처리
        if (Input.GetKeyDown(KeyCode.G))
        {
            player.StateMachine.TransitionTo(player.GuardState);
            return;
        }

        //회피 키 입력 처리
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

        //공격 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Z))
        {
            player.StateMachine.TransitionTo(player.Attack1State);
            return;
        }
    }

    public void Exit()
    {
        // 대기 상태를 벗어날 때 할 일
    }
}