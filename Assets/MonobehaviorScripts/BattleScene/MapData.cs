using UnityEngine;

public class MapData : MonoBehaviour
{
    // 이 맵에서 플레이어가 시작할 위치를 인스펙터에서 연결해둠
    [Header("플레이어 설정")]
    public Transform spawnPoint;

    [Header("적 설정")]
    public GameObject[] enemyPrefab;
    public Transform[] enemySpawnPoints;

    [Header("포탈 설정")]
    public GameObject exitPortal;
}