using UnityEngine;

public class PlayerJumpState : IState
{
    private PlayerController player;
    private float jumpPower = 7f; // 점프력 (원하는 만큼 조절)

    public PlayerJumpState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // 1. 점프 애니메이션 재생
        player.Animator.Play("Player_Jump");

        // 2. 점프 실행: X축 속도는 그대로 유지하고, Y축 속도만 위로 쏘아 올림
        player.Rb.linearVelocity = new Vector2(player.Rb.linearVelocity.x, jumpPower);
    }

    public void Update()
    {
        // 3. 공중에서도 좌우로 이동할 수 있도록 조작 허용 (MoveState와 동일한 로직)
        float moveInput = Input.GetAxisRaw("Horizontal");
        player.Rb.linearVelocity = new Vector2(moveInput * player.MoveSpeed, player.Rb.linearVelocity.y);
        FlipSprite(moveInput);

        // 4. 착지 판정 (아래로 떨어지고 있을 때 + 바닥에 닿았을 때)
        // linearVelocity.y <= 0f 조건이 없으면, 점프하자마자 바닥에 닿아있다고 판정될 수 있음
        if (player.Rb.linearVelocity.y <= 0f && player.IsGrounded())
        {
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
    }

    public void Exit()
    {
        // 착지 시 특별히 해야 할 일이 있다면 여기에 작성 (예: 착지 먼지 이펙트 생성)
    }

    // 캐릭터 방향 전환 헬퍼 함수
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