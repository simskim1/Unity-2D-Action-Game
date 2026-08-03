using UnityEngine;

public interface IDamageable
{
    // 체력을 가진 모든 객체는 이 함수를 반드시 구현해야 함
    void TakeDamage(DamageInfo info);
}