using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public Animator Animator { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public StatManager Stats { get; private set; }

    // 1. 상태 머신과 상태들
    public StateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyHitState HitState { get; private set; }


    // 2. AI 판단에 필요한 변수들
    public Transform PlayerTarget { get; private set; }
    public float detectRange = 4f; // 플레이어를 발견하는 시야 거리
    public float moveSpeed = 2f;

    [Header("Attack Settings")]
    public Transform attackPoint;    // 방금 만든 AttackPoint 오브젝트 연결
    public float attackRange = 0.8f; // 공격을 시작하는 타격 거리
    public LayerMask playerLayer;
    public float atttackPower;

    public float attackCooldown = 2.0f; // 2초마다 공격 가능
    public float lastAttackTime = -9999f; // 시작하자마자 때릴 수 있도록 아주 작은 값으로 초기화

    void Awake()
    {
        StateMachine = new StateMachine();
        IdleState = new EnemyIdleState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
        HitState = new EnemyHitState(this);

        Animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        Stats = GetComponent<StatManager>();


        // 플레이어 찾기 (태그가 "Player"인 오브젝트)
        PlayerTarget = GameObject.FindGameObjectWithTag("Player").transform;

        // 첫 시작은 대기 상태로!
        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.Update();
    }

    // 인터페이스 구현부 (아까 더미에서 만들었던 그 부분!)
    public void TakeDamage(float damage)
    {
        Debug.Log($"적이 데미지를 입음: {damage}");
        float damageTake = damage;

        Stats.Damage(damageTake);

        // 체력이 0 이하가 되어 죽을 때
        if (Stats.currentHealth <= 0)
        {
            //매니저를 찾아서 적이 죽었다고 신고!
            StageManager stageManager = FindFirstObjectByType<StageManager>();
            if (stageManager != null)
            {
                stageManager.OnEnemyDefeated();
            }

            // 이후 적 사망 처리 (애니메이션 재생, 파괴 등)
            // StateMachine.TransitionTo(DeadState); 
            // Destroy(gameObject, 1f); 
        }

        StateMachine.TransitionTo(HitState, true);
    }

    // 디버그용: 씬(Scene) 창에서 시야와 공격 범위를 원으로 그려주는 유용한 기능!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void PerformAttack(float damageMultiplier)
    {
        Debug.Log("적 공격 실행");
        // 1. attackPoint를 중심으로 attackRange 반경 내에 있는 'enemyLayer'를 가진 모든 콜라이더를 배열로 가져옴
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

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

            if (damageable != null)
            {
                // 기본 공격력 * 콤보 배율
                float finalDamage = atttackPower * damageMultiplier;

                // 인터페이스의 데미지 함수 호출!
                damageable.TakeDamage(finalDamage);

                Debug.Log($"적 타격 성공! 데미지: {finalDamage}");
            }
        }
    }

    public void EndAttack()
    {
        StateMachine.TransitionTo(IdleState);
    }

    public void EndHit()
    {
        StateMachine.TransitionTo(IdleState);
        Debug.Log("EndAttack");
    }
}