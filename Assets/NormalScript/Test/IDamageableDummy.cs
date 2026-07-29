using UnityEngine;

public class IDamageableDummy : MonoBehaviour, IDamageable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage)
    {
        Debug.Log($"더미가 맞았습니다! 들어온 데미지: {damage}");
    }
}
