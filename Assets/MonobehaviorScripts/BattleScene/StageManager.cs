using UnityEngine;

// 1. 스테이지의 종류를 정의하는 열거형 (가독성을 엄청 높여줘!)
public enum StageType
{
    Normal,
    Rest,
    Boss
}

public class StageManager : MonoBehaviour
{
    // 각 스테이지별 맵 프리팹들을 인스펙터에서 넣어줄 수 있는 배열
    public GameObject[] mapPrefabs;

    private GameObject currentMap; // 현재 화면에 떠있는 맵
    public Transform player;       // 플레이어 위치를 초기화하기 위해 연결

    private int activeEnemyCount = 0;
    private GameObject currentPortal;

    // 2. 1레벨의 전체 스테이지 청사진 (총 7개)
    // 인스펙터 창에서도 배열 내용이 바로 보여서 기획 수정하기 좋아!
    public StageType[] levelBlueprint = new StageType[7] {
        StageType.Normal, // 1스테이지
        StageType.Normal, // 2스테이지
        StageType.Normal, // 3스테이지
        StageType.Rest,   // 4스테이지 (휴식)
        StageType.Normal, // 5스테이지
        StageType.Normal, // 6스테이지
        StageType.Boss    // 7스테이지 (보스)
    };

    // 현재 진행 중인 스테이지 번호 (0부터 시작하므로 0 = 1스테이지)
    public int currentStageIndex = 0;

    void Start()
    {
        // 게임 시작 시 첫 스테이지 로드
        LoadStage(currentStageIndex);
    }

    // 적을 모두 잡거나 포탈을 탔을 때 호출할 함수
    public void StageClear()
    {
        currentStageIndex++; // 다음 스테이지로!

        // 만약 보스까지 다 잡고 배열 범위를 넘어갔다면? -> 레벨 클리어!
        if (currentStageIndex >= levelBlueprint.Length)
        {
            Debug.Log("1레벨 클리어! 다음 지역으로 이동하거나 엔딩!");
            return;
        }

        // 아직 남았다면 다음 스테이지 로드
        LoadStage(currentStageIndex);
    }

    // 현재 인덱스에 맞춰 맵과 몬스터를 세팅해주는 함수
    private void LoadStage(int index)
    {
        // 1. 기존 맵이 있다면 파괴해서 화면에서 치워버림
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        // 2. 새로운 맵 프리팹을 화면에 생성 (Instantiate)
        currentMap = Instantiate(mapPrefabs[index], Vector3.zero, Quaternion.identity);

        MapData newMapData = currentMap.GetComponent<MapData>();

        if (newMapData != null && newMapData.spawnPoint != null)
        {
            // 플레이어를 그 맵의 정해진 위치로 순간이동!
            player.position = newMapData.spawnPoint.position;

            if (newMapData.exitPortal != null)
            {
                currentPortal = newMapData.exitPortal;
                currentPortal.SetActive(false);
            }

            activeEnemyCount = 0;

            for (int i = 0; i < newMapData.enemySpawnPoints.Length; i++)
            {
                // 적을 생성하고, 맵 오브젝트의 자식으로 설정해 둠
                // (이렇게 하면 나중에 맵이 Destroy될 때 적들도 한방에 같이 청소돼서 아주 깔끔해!)
                Instantiate(newMapData.enemyPrefab[i], newMapData.enemySpawnPoints[i].position, Quaternion.identity, currentMap.transform);
                activeEnemyCount++;
            }

            if (activeEnemyCount == 0 && currentPortal != null)
            {
                currentPortal.SetActive(true);
            }
        }
        else
        {
            // 혹시 시작점을 안 만들어뒀다면 기본 위치로
            player.position = Vector3.zero;
        }

        StageType currentType = levelBlueprint[index];
        Debug.Log($"현재 스테이지: {index + 1} / 타입: {currentType}");

        // 스테이지 타입에 따라 다른 동작 실행
        switch (currentType)
        {
            case StageType.Normal:
                LoadNormalStage();
                break;
            case StageType.Rest:
                LoadRestStage();
                break;
            case StageType.Boss:
                LoadBossStage();
                break;
        }
    }

    private void LoadNormalStage()
    {
        Debug.Log("일반 몬스터들을 스폰합니다.");
        // TODO: 웨이브별 몬스터 소환, 포탈 닫기 등
    }

    private void LoadRestStage()
    {
        Debug.Log("모닥불과 상인을 스폰합니다.");
        // TODO: 체력 회복 NPC, 상점 UI 활성화, 몬스터 스폰 안 함
    }

    private void LoadBossStage()
    {
        Debug.Log("보스 몬스터 등장!");
        // TODO: 보스룸 전용 맵 세팅, 보스 BGM 재생, 보스 객체 생성
    }

    public void OnEnemyDefeated()
    {
        activeEnemyCount--; // 적 숫자 1 감소
        Debug.Log($"적 처치! 남은 적: {activeEnemyCount}");

        // 남은 적이 0마리 이하가 되면 포탈 활성화!
        if (activeEnemyCount <= 0 && currentPortal != null)
        {
            Debug.Log("포탈이 열렸습니다!");
            currentPortal.SetActive(true);
        }
    }
}