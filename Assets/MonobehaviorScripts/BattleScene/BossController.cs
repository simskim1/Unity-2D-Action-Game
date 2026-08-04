using UnityEngine;

public class BossController : MonoBehaviour, IDamageable
{
    [Header("보스 상태 정보")]
    public int currentPhase = 1;
    public bool isTransitioning = false; // 페이즈 전환 연출 중인지 체크

    // 보스 상태 머신 (기존에 쓰시던 구조 활용)
    public Animator Animator { get; private set; }
    public StateMachine stateMachine;
    public StatManager Stats;

    // 공격 쿨타임 제어
    private float attackTimer = 0f;
    public float timeBetweenAttacks = 3f;

    void Awake()
    {
        stateMachine = new StateMachine();
        Stats = GetComponent<StatManager>();
        Animator = GetComponent<Animator>();
    }
    void Update()
    {
        // 페이즈 전환 중이거나 죽었을 때는 패턴을 실행하지 않음
        if (isTransitioning || Stats.currentHealth <= 0) return;

        attackTimer -= Time.deltaTime;

        // 쿨타임이 다 돌았고, 현재 보스가 대기/추적 상태일 때 패턴 실행*******************************************************************************
        //if (attackTimer <= 0f && (stateMachine.CurrentState == IdleState || stateMachine.CurrentState == ChaseState))
        {
            ChooseNextPattern();
            attackTimer = timeBetweenAttacks; // 2페이즈가 되면 이 값을 줄여서 더 자주 공격하게 할 수도 있습니다.
        }
    }

    // 다음 공격 패턴을 결정하는 함수
    private void ChooseNextPattern()
    {
        // 1~4번 패턴 중 랜덤하게 선택 (거리에 따라 확률을 다르게 줄 수도 있습니다)
        int patternIndex = Random.Range(1, 5);

        switch (patternIndex)
        {/*******************************************************************************************
            case 1: stateMachine.TransitionTo(MeleeAttackState); break;
            case 2: stateMachine.TransitionTo(DashAttackState); break;
            case 3: stateMachine.TransitionTo(ShootAttackState); break;
            case 4: stateMachine.TransitionTo(SlamAttackState); break;
            */////////////////////////////////////////////////////////////////////////////////////////////
        }
    }

    // 데미지 처리 및 페이즈 전환 체크
    public void TakeDamage(DamageInfo info)
    {
        if (isTransitioning) return; // 변신 중엔 무적! (선택 사항)

        Stats.Damage(info.damage);

        // --- [페이즈 2 전환 체크] ---
        // 체력이 절반 이하로 떨어졌고, 아직 1페이즈라면?
        if (currentPhase == 1 && Stats.currentHealth <= Stats.maxHealth * 0.5f)
        {
            EnterPhase2();
        }
        else
        {
            // 평소 피격 리액션 (보스는 보통 슈퍼아머가 있으므로 넉백을 안 주거나, 약하게 줍니다)********************************************************************************************
            //stateMachine.TransitionTo(HitState, true);
        }
    }

    private void EnterPhase2()
    {
        Debug.Log("보스 2페이즈 돌입!");
        currentPhase = 2;
        isTransitioning = true; // 패턴 일시 정지

        // 2페이즈 전환 상태로 넘어가서 포효하는 애니메이션 재생, 카메라 진동, 이펙트 등 처리**********************************************************************************
        //stateMachine.TransitionTo(PhaseTransitionState, true);
    }
}