using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    public StateMachine StateMachine { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    //레이어를 저장
    public int PlayerLayer { get; private set; }
    public int EnemyLayer { get; private set; }

    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 5.0f;
    public float MoveSpeed => moveSpeed;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint; // 캐릭터 발끝에 위치할 빈 게임오브젝트
    [SerializeField] private float groundCheckRadius = 0.2f; // 감지 반경
    [SerializeField] private LayerMask groundLayer; // 바닥으로 인식할 레이어 (Tilemap 등에 설정)

    [Header("Jump Feel Settings")]
    public float coyoteTime = 0.15f;      // 발판을 벗어나도 점프가 가능한 시간
    public float coyoteTimeCounter;       // 코요테 타임 측정용 변수

    public float jumpBufferTime = 0.2f;   // 미리 입력된 점프를 기억하는 시간
    public float jumpBufferCounter;       // 점프 버퍼링 측정용 변수

    [Header("Invincibility Settings")]
    public float invincibilityTimer = 0f;

    [Header("Combo Attack Settings")]
    public bool canCombo = false;

    [Header("Attack Hitbox Settings")]
    public Transform attackPoint;    // 방금 만든 AttackPoint 오브젝트 연결
    public float attackRange = 0.8f; // 공격 판정 반경(크기)
    public LayerMask enemyLayer;     // 적이 속한 레이어 지정


    // 사용할 상태들 미리 선언
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerRollState RollState { get; private set; }
    public PlayerGuardState GuardState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerGuardBreakState GuardBreakState { get; private set; }
    public StatManager Stats { get; private set; }
    // 공격 상태 3가지를 저장할 변수
    public PlayerAttackState Attack1State { get; private set; }
    public PlayerAttackState Attack2State { get; private set; }
    public PlayerAttackState Attack3State { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        StateMachine = new StateMachine();

        Rb = GetComponent<Rigidbody2D>();
        Stats = GetComponent<StatManager>();

        PlayerLayer = LayerMask.NameToLayer("Player");
        EnemyLayer = LayerMask.NameToLayer("Enemy");

        // 상태 객체 생성 (this를 넘겨주어 상태 클래스에서 플레이어 컴포넌트에 접근 가능하게 함)
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        JumpState = new PlayerJumpState(this);
        RollState = new PlayerRollState(this);
        GuardState = new PlayerGuardState(this);
        HitState = new PlayerHitState(this);
        GuardBreakState = new PlayerGuardBreakState(this);
        Attack1State = new PlayerAttackState(this, 1, 0.15f);
        Attack2State = new PlayerAttackState(this, 2, 0.15f);
        Attack3State = new PlayerAttackState(this, 3, 0.15f);

        // 첫 시작 상태를 Idle로 초기화
        StateMachine.Initialize(IdleState);
    }

    private void Start()
    {
        StageManager.Instance.Player = this;
    }

    private void Update()
    {
        //가드브레이크 상태 테스트
        if (Input.GetKeyUp(KeyCode.P))
        {
            StateMachine.TransitionTo(GuardBreakState);
            return;
        }

        //피격 상태 테스트
        if(Input.GetKeyUp(KeyCode.L))
        {
            StateMachine.TransitionTo(HitState);
            return;
        }

        UpdateJumpTimers();

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        // 플레이어의 Update에서는 단 한 줄만 실행하면 됨!
        // 현재 상태가 무엇이든 알아서 해당 상태의 Update가 실행됨.
        StateMachine.Update();
    }

    public bool IsGrounded()
    {
        // 캐릭터 발끝 위치(groundCheckPoint)에서 원(Circle)을 그려서 groundLayer와 겹치는지 확인
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    // 유니티 에디터 상에서 감지 반경(원)을 눈으로 보기 위한 기믹
    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }

    private void UpdateJumpTimers()
    {
        // 1. 코요테 타임 계산
        if (IsGrounded())
        {
            // 땅에 있으면 타이머를 꽉 채워둠
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            // 허공에 있으면 타이머가 서서히 줄어듦
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 2. 점프 버퍼링 계산 (Input.GetButtonDown("Jump") 대신 쓰기 편한 키로 설정 가능)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 점프 키를 누르면 타이머를 꽉 채움
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            // 누르지 않을 때는 서서히 줄어듦
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    // 점프를 실행하고 나면 타이머를 초기화해주는 헬퍼 함수
    public void UseJump()
    {
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }

    public void TakeDamage(DamageInfo info)
    {
        bool isGuard = false;
        float damageTake = info.damage;
        if (StateMachine.CurrentState == RollState || invincibilityTimer > 0f)
        {
            Debug.Log("회피 성공! 데미지 무시");
            return;
        }
        else if (HitState.isInvincible)
        {
            Debug.Log("피격무적중!");
            return;
        }
        if (StateMachine.CurrentState == GuardState)
        {
            float guardStaminaCost = 1.5f * info.damage;
            if (Stats.CanUseStamina(guardStaminaCost))
            {
                Debug.Log("가드 성공");
                damageTake *= 0.2f;
                Stats.UseStamina(guardStaminaCost);
                isGuard = true;
            }
            else
            {
                Debug.Log("가드 브레이크");
                Stats.UseStamina(Stats.currentStamina);
                // StateMachine.TransitionTo(GuardBreakState);
            }
        }
        else if (StateMachine.CurrentState == GuardBreakState) 
        {
            damageTake = 1.3f * info.damage;
            Stats.Damage(damageTake);
            return;
        }

        // 실제 체력을 깎는 로직 (StatManager에 health 관련 변수가 있다면 거기서 처리)
        Debug.Log($"{damageTake}의 데미지를 입었다");
        Stats.Damage(damageTake);
        invincibilityTimer = 0.6f;

        // 피격 애니메이션과 경직을 처리할 상태로 전환
        if (!isGuard) 
        {
            StateMachine.TransitionTo(HitState);
        }
    }

    public void EnableComboWindow()
    {
        canCombo = true;
    }

    public void EndAttack()
    {
        StateMachine.TransitionTo(IdleState);
    }

    public void PerformAttack(float damageMultiplier)
    {
        Debug.Log("공격 실행");
        // 1. attackPoint를 중심으로 attackRange 반경 내에 있는 'enemyLayer'를 가진 모든 콜라이더를 배열로 가져옴
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        if(hitEnemies.Length == 0)
        {
            Debug.Log("대상 없음");
        }
        // 2. 감지된 모든 적들에게 데미지 전달
        foreach (Collider2D enemy in hitEnemies)
        {
            // 우리가 만든 IDamageable 인터페이스를 찾아냄! 
            // 적의 종류(슬라임, 고블린, 상자)가 뭐든 상관없이 '맞을 수 있는 애'면 무조건 가져옴.
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            if (damageable != null)
            {
                // 기본 공격력 * 콤보 배율
                DamageInfo myAttackInfo = new DamageInfo
                {
                    damage = Stats.baseAttackPower * damageMultiplier,
                    attacker = this.transform,
                    knockbackPower = 10f // 플레이어의 공격 스킬마다 다르게 설정 가능!
                };

                // 인터페이스의 데미지 함수 호출!
                damageable.TakeDamage(myAttackInfo);

                Debug.Log($"적 타격 성공! 데미지: {myAttackInfo.damage}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        // 에디터 씬 화면에 빨간색 동그라미로 공격 범위를 그려줌 (게임 화면엔 안 보임)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}