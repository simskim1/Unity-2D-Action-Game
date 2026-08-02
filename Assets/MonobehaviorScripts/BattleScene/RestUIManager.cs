using UnityEngine;

public class RestUIManager : MonoBehaviour
{
    // 실제 게임에서 사용 중인 플레이어 스탯 클래스로 변경해야 합니다.
    
    [Header("맵 데이터 참조")]
    public MapData currentMapData; // 포탈을 열기 위해 현재 맵 데이터 참조
    public RestPoint restPoint;

    private PlayerController player;

    private void Start()
    {
        player = StageManager.Instance.Player;
    }
    // 1. 체력 회복
    public void OnClickHeal()
    {
        if(player == null)
        {
            Debug.Log("플레이어 정보 없음");
        }
        Debug.Log("힐 버튼 작동됨");
        //player.Stats.currentHealth = player.Stats.maxHealth;
        restPoint.Rested();
        FinishRest();
    }

    // 2. 스태미나 최대치 증가
    public void OnClickMaxStamina()
    {
        //player.Stats.maxStamina += 10;
        //player.Stats.currentStamina = player.Stats.maxStamina; // 최대치 올리면서 꽉 채워주기
        restPoint.Rested();
        FinishRest();
    }

    // 3. 체력 최대치 증가
    public void OnClickMaxHP()
    {
        //player.Stats.maxHealth += 20;
        //player.Stats.currentHealth += 20;
        restPoint.Rested();
        FinishRest();
    }

    // 4. 공격력 증가
    public void OnClickAttackPower()
    {
        //player.Stats.baseAttackPower += 5;
        restPoint.Rested();
        FinishRest();
    }

    // 휴식 종료 처리
    private void FinishRest()
    {
        gameObject.SetActive(false); // UI 닫기
        Time.timeScale = 1f; // 시간 재개

        // 휴식을 완료했으므로 다음 스테이지로 가는 포탈 활성화
        if (currentMapData != null && currentMapData.exitPortal != null)
        {
            Debug.Log("휴식완료");
            currentMapData.exitPortal.SetActive(true);
        }
    }
}