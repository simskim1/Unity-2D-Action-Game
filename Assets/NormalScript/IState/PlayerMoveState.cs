using UnityEngine;

public class PlayerMoveState : IState
{
    private PlayerController player;
    float rollStaminaCost = 25f;

    public PlayerMoveState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // 이동 애니메이션 재생
        player.Animator.Play("Player_Move");
    }

    public void Update()
    {
        // 이동 키 입력이 감지되지 않으면 Idle 상태로 전환
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) < 0.1f)
        {
            player.StateMachine.TransitionTo(player.IdleState);
            return;
        }

        //실제 물리 이동 처리
        player.Rb.linearVelocity = new Vector2(moveInput * player.MoveSpeed, player.Rb.linearVelocity.y);

        //바ㅏ보는 방향 전환
        FlipSprite(moveInput);

        // 방어 키 입력처리
        if (Input.GetKeyDown(KeyCode.G))
        {
            player.StateMachine.TransitionTo(player.GuardState);
            return;
        }
        //회피 키 입력처리
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
        //이동 상태를 벗어날 때 할 일
        player.Rb.linearVelocity = new Vector2(0, player.Rb.linearVelocity.y);
    }

    //방향 전환
    private void FlipSprite(float moveInput)
    {
        // 1. 현재 캐릭터의 스케일 값을 그대로 가져옵니다.
        Vector3 currentScale = player.transform.localScale;

        // 2. Mathf.Abs를 이용해 X축 크기의 절댓값(원래 크기)을 구합니다.
        float originalSize = Mathf.Abs(currentScale.x);

        // 3. 입력 방향에 따라 부호만 바꿔서 다시 적용합니다.
        if (moveInput > 0.1f)
        {
            // 오른쪽 이동: 양수 유지, y와 z는 원래 값 그대로
            player.transform.localScale = new Vector3(originalSize, currentScale.y, currentScale.z);
        }
        else if (moveInput < -0.1f)
        {
            // 왼쪽 이동: 음수로 반전, y와 z는 원래 값 그대로
            player.transform.localScale = new Vector3(-originalSize, currentScale.y, currentScale.z);
        }
    }
}