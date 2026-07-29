using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 오브젝트의 태그가 Player라면
        if (collision.CompareTag("Player"))
        {
            // 아까 만든 StageManager를 찾아서 다음 스테이지로 넘김
            FindFirstObjectByType<StageManager>().StageClear();
        }
    }
}