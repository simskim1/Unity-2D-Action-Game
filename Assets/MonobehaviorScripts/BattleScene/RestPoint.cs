using UnityEngine;

public class RestPoint : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject restUI; // 인스펙터에서 휴식 선택지 캔버스를 연결

    [Header("애니메이터")]
    public Animator animator;

    private bool isPlayerNearby = false;
    private bool hasRested = false; // 한 번만 쉴 수 있도록 체크

    public void Start()
    {
        // 대기 애니메이션 재생
        animator.Play("Campfire_Idle");
    }
    void Update()
    {
        // 플레이어가 근처에 있고, 아직 쉬지 않았으며, F키를 눌렀을 때
        if (isPlayerNearby && !hasRested && Input.GetKeyDown(KeyCode.F))
        {
            OpenRestUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }

    void OpenRestUI()
    {
        restUI.SetActive(true);
        Time.timeScale = 0f; // UI가 켜져 있는 동안 게임 일시정지 (선택사항)
    }

    public void Rested()
    {
        hasRested = true;
    }
}