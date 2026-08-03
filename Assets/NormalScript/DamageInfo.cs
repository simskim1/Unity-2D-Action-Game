using UnityEngine;
[System.Serializable]
public struct DamageInfo
{
    public float damage;
    public float knockbackPower; // 공격 종류(약공격, 강공격)에 따라 넉백 강도를 다르게 줄 수 있음!
    [HideInInspector]
    public Transform attacker; // 넉백 방향을 계산하기 위한 공격자 위치

    // 나중에 이런 걸 쉽게 추가할 수 있습니다!
    // public bool isCritical;
    // public DamageType type; (예: 물리, 마법)
}