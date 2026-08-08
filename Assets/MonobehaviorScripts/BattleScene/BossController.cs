using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour, IDamageable
{
    [Header("보스 상태 정보")]
    public int currentPhase = 1;
    public bool isTransitioning = false; // 페이즈 전환 연출 중인지 체크

    // 보스 상태 머신 (기존에 쓰시던 구조 활용)
    public Animator animator { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public StateMachine stateMachine;
    public StatManager Stats;

    public Transform PlayerTarget { get; private set; }
    public float detectRange = 4f;
    public float moveSpeed = 2f;

    public BossIdleState IdleState { get; private set; }
    public BossChaseState ChaseState { get; private set; }

    // 공격 쿨타임 제어
    [Header("Attack Settings")]
    private float attackTimer = 0f;
    public float timeBetweenAttacks = 3f;
    public LayerMask playerLayer;

    public bool isAttackFinished = false;
    public bool endAnimation = false;

    [Header("Boss Patterns")]
    public bool hasMelee = false;
    public bool hasDash = false;
    public bool hasSlam = false;

    [Header("MeleeAttack")]
    public Transform meleeAttackPoint;
    public float meleeAttackRange = 0.8f;

    [Header("Dash Attack")]
    public GameObject dashHitbox;

    [Header("SlamAttack")]
    public Transform slamAttackPoint;
    public float slamAttackRange = 0.8f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint; // 캐릭터 발끝에 위치할 빈 게임오브젝트
    [SerializeField] private float groundCheckRadius = 0.2f; // 감지 반경
    [SerializeField] private LayerMask groundLayer; // 바닥으로 인식할 레이어 (Tilemap 등에 설정)

    [SerializeField]
    private DamageInfo damageInfo;

    void Awake()
    {
        stateMachine = new StateMachine();
        Stats = GetComponent<StatManager>();
        animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        IdleState = new BossIdleState(this);
        ChaseState = new BossChaseState(this);

        PlayerTarget = GameObject.FindGameObjectWithTag("Player").transform;
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
            case 3: stateMachine.TransitionTo(SlamAttackState); break;
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

    public void MeleeAttack()
    {
        Debug.Log("적 공격 실행");
        // 1. attackPoint를 중심으로 attackRange 반경 내에 있는 'enemyLayer'를 가진 모든 콜라이더를 배열로 가져옴
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, playerLayer);

        if (hitPlayer.Length == 0)
        {
            Debug.Log("대상 없음");
        }
        // 2. 감지된 모든 적들에게 데미지 전달
        foreach (Collider2D player in hitPlayer)
        {
            // 우리가 만든 IDamageable 인터페이스를 찾아냄! 
            // 적의 종류(슬라임, 고블린, 상자)가 뭐든 상관없이 '맞을 수 있는 애'면 무조건 가져옴.
            IDamageable damageable = player.GetComponent<IDamageable>();

            DamageInfo info = damageInfo;
            info.attacker = transform;
            info.damage = Stats.baseAttackPower;
            info.knockbackPower = 0.1f;

            if (damageable != null)
            {
                damageable.TakeDamage(info);
            }
        }
        isAttackFinished = true;
    }

    public void EnableDashHitbox()
    {
        dashHitbox.SetActive(true);
    }

    public void DisableDashHitbox()
    {
        dashHitbox.SetActive(false);
    }

    public void SlamAttack()
    {
        Debug.Log("적 공격 실행");
        if (isAttackFinished == true)
        {
                return;
        }
        // 1. attackPoint를 중심으로 attackRange 반경 내에 있는 'enemyLayer'를 가진 모든 콜라이더를 배열로 가져옴
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(slamAttackPoint.position, slamAttackRange, playerLayer);

        if (hitPlayer.Length == 0)
        {
            Debug.Log("대상 없음");
        }
        // 2. 감지된 모든 적들에게 데미지 전달
        foreach (Collider2D player in hitPlayer)
        {
            // 우리가 만든 IDamageable 인터페이스를 찾아냄! 
            // 적의 종류(슬라임, 고블린, 상자)가 뭐든 상관없이 '맞을 수 있는 애'면 무조건 가져옴.
            IDamageable damageable = player.GetComponent<IDamageable>();

            DamageInfo info = damageInfo;
            info.attacker = transform;
            info.damage = Stats.baseAttackPower * 2;
            info.knockbackPower = 0.5f;

            if (damageable != null)
            {
                damageable.TakeDamage(info);
            }
        }
        isAttackFinished = true;
    }

    public void Attacking()
    {
        isAttackFinished = true;
    }

    public void EndAnimation()
    {
        endAnimation = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(slamAttackPoint.position, slamAttackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }

    public bool IsGrounded()
    {
        // 캐릭터 발끝 위치(groundCheckPoint)에서 원(Circle)을 그려서 groundLayer와 겹치는지 확인
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }
}