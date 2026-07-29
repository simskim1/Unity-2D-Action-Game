using UnityEngine;

public class PlayerAttackState : IState
{
    private PlayerController player;
    private int comboStep;           // 현재 몇 타째인지 (1, 2, 3)
    private bool hasBufferedInput;   // 선입력(버퍼링)이 들어왔는지 기억하는 변수
    private float comboAbleTimer = 0.1f; //다음동작으로 이어지기 전 무조건 애니메이션을 트는 시간
    private float comboCheck;

    // 데미지가 이미 들어갔는지 체크하는 변수 (다단 히트 방지)
    private bool hasDealtDamage;
    // 애니메이션 시작 후 몇 초 뒤에 타격 판정을 할지
    private float attackImpactTime = 0.15f;

    public PlayerAttackState(PlayerController player, int comboStep, float comboAbleTime)
    {
        this.player = player;
        this.comboStep = comboStep;
        this.comboAbleTimer = comboAbleTime;
    }

    public void Enter()
    {
        // 공격 시작 시 초기화
        player.canCombo = false;
        hasBufferedInput = false;
        comboCheck = 0.0f;

        hasDealtDamage = false; // 타격 초기화

        // 공격할 땐 미끄러지지 않게 제자리에 멈춤 (필요시 앞으로 살짝 전진하는 속도를 줘도 됨)
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocity.y);

        // comboStep에 맞춰 Attack1, Attack2, Attack3 애니메이션 재생
        player.Animator.Play("Player_Attack" + comboStep);
    }

    public void Update()
    {
        // 선입력(Input Buffering) 로직: 
        // 애니메이션이 끝나기 전이더라도 공격 키(예: Z)를 누르면 눌렀다는 사실을 기억해 둠!
        if (Input.GetKeyDown(KeyCode.Z))
        {
            hasBufferedInput = true;
        }

        comboCheck += Time.deltaTime;

        if (!hasDealtDamage && comboCheck >= attackImpactTime)
        {
            float multiplier = 0.8f + (comboStep * 0.2f);// 1타: 1배 2타: 1.2배 3타: 1.4배
            player.PerformAttack(multiplier);

            hasDealtDamage = true; // 플래그를 켜서 이번 공격에서 더 이상 데미지가 안 들어가게 막음
        }

        if (comboCheck >= comboAbleTimer && !player.canCombo)
        {
            player.EnableComboWindow();
        }

        // 애니메이션의 특정 프레임을 지나 canCombo가 켜졌고, 선입력된 키가 있다면?
        if (player.canCombo && hasBufferedInput)
        {
            // 다음 콤보로 부드럽게 넘어감!
            if (comboStep == 1)
                player.StateMachine.TransitionTo(player.Attack2State);
            else if (comboStep == 2)
                player.StateMachine.TransitionTo(player.Attack3State);

            // 3타째는 다음 콤보가 없으므로 무시됨 (자연스럽게 EndAttack 이벤트가 발생해 Idle로 복귀)
        }
    }

    public void Exit()
    {
        // 상태를 빠져나갈 때 콤보 창을 닫아줌
        player.canCombo = false;
    }
}