using UnityEngine;

public class StatManager : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegen = 20f;
    public float regenTime = 1.0f;

    public float regenCheck = 0.0f;

    //기본 공격력
    [Header("Power Settings")]
    public float baseAttackPower = 10f;

    public bool isDead = false;

    private void Start()
    {
        //값들을 초기화
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    private void Update()
    {
        //아직 스태미나 회복 대기중이라면
        if (regenCheck > 0.0f)
        {
            regenCheck -= Time.deltaTime;
        }
        //스태미나 회복 대기시간이 끝났고 스태미나가 최대가 아니라면
        else if(currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    //스태미나가 스태미나 비용ㅇ 이사인지 검사
    public bool CanUseStamina(float amount)
    {
        return currentStamina >= amount;
    }

    // 실제로 스태미나를 깎는 함수
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // 스태미나를 방금 썼으므로, 회복 대기 타이머를 다시 꽉 채움 (회복 정지)
        regenCheck = regenTime;
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth <= 0f)
        {
            Debug.Log("사망");
            isDead = true;
            //사망 애니메이션, 게임오버 함수를 작성
        }
    }
}
