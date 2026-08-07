using UnityEngine;

public class DashHitbox : MonoBehaviour
{
    public BossController bossStats; // 부모에 있는 보스 스탯 참조

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 대상이 플레이어 레이어인지 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                DamageInfo info = new DamageInfo();
                info.attacker = bossStats.transform;
                info.damage = bossStats.Stats.baseAttackPower;
                info.knockbackPower = 3f; // 돌진은 넉백이 강해야 제맛!

                damageable.TakeDamage(info);
            }
        }
    }
}