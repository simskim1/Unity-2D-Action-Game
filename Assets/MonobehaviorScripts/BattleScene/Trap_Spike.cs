using UnityEngine;

public class Trap_Spike : MonoBehaviour
{
    private DamageInfo damageInfo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Triggered Trap Spike");
            // 플레이어에게 데미지를 줄 수 있는 인터페이스(IDamageable)가 있는지 확인
            var damageable = collision.GetComponent<IDamageable>();
            // 있다면 데미지 주기 (물음표는 null이 아닐 때만 실행하라는 뜻!)
            DamageInfo info = damageInfo;
            info.attacker = transform;

            if (damageable != null)
            {
                damageable.TakeDamage(info);
            }
        }
    }
}
