using UnityEngine;

public class BossChaseState : IState
{
    private BossController boss;

    public BossChaseState(BossController boss)
    {
        this.boss = boss;
    }

    public void Enter()
    {
        // 추적(이동) 애니메이션 재생
        boss.animator.Play("Enemy_Idle");
    }

    public void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(boss.transform.position, boss.PlayerTarget.position);


        // 플레이어를 향해 이동 및 방향(Flip) 전환
        MoveTowardsPlayer();
    }

    public void Exit()
    {

        // 추적 상태를 벗어날 때(공격하거나 포기할 때) 얼음판처럼 미끄러지지 않게 속도를 0으로 멈춤
        //enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);
    }

    private void MoveTowardsPlayer()
    {
        // 방향 구하기: 플레이어가 적보다 오른쪽에 있으면 1, 왼쪽에 있으면 -1
        float directionX = boss.PlayerTarget.position.x > boss.transform.position.x ? 1f : -1f;

        // 이동: X축으로는 지정한 속도만큼 이동, Y축 속도(중력/낙하)는 그대로 유지
        boss.Rb.linearVelocity = new Vector2(directionX * boss.moveSpeed, boss.Rb.linearVelocity.y);

        // 방향 뒤집기(Flip): 로컬 스케일의 X값을 방향에 맞춰 1 또는 -1로 변경
        // 주의: 원래 적 오브젝트의 크기가 1이 아니라면, 원본 스케일 값에 맞춰 곱해줘야 해!
        boss.transform.localScale = new Vector3(directionX * 2, 1f * 3, 1f);
    }
}