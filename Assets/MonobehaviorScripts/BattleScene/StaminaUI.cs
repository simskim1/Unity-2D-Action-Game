using UnityEngine;
using UnityEngine.UI; // 유니티 UI 컴포넌트(Image 등)를 제어하려면 이 네임스페이스가 필수야!

public class StaminaUI : MonoBehaviour
{
    [Header("References")]
    public StatManager playerStats;   // 플레이어의 정보(데이터)를 가져올 곳
    public Image staminaFillImage;    // 조작할 게이지 바 이미지 (뷰)

    private void Update()
    {
        // 참조가 비어있지 않은지 안전하게 체크
        if (playerStats != null && staminaFillImage != null)
        {
            // fillAmount는 0.0(0%)부터 1.0(100%) 사이의 소수를 받음
            // 현재 스태미나를 최대 스태미나로 나누어서 비율을 구함
            staminaFillImage.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
        }
    }
}