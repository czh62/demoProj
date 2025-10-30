using UnityEngine;
using System.Collections;
using System.Collections.Generic;  // 用于 List
using UnityEngine.Events;         // 用于 UnityEvent（如果需要）

public class MeteorSpawner : MonoBehaviour
{
    [Header("GameManager 引用")]
    public GameManager gameManager;    // 拖拽 GameManager 对象到此字段

    [Header("默认模式设置 (Menu)")]
    public GameObject[] defaultPrefabs;  // Menu 模式下的多种预制体数组（e.g., 背景粒子或演示陨石）
    public float defaultSpawnInterval = 0.5f;  // Menu 模式发射间隔（秒）

    [Header("游戏模式设置 (Playing)")]
    public GameObject[] gamePrefabs;     // Playing 模式下的另一种预制体数组（e.g., 游戏敌人）
    public float gameSpawnInterval = 1f;    // Playing 模式发射间隔（秒，可选调整）

    [Header("通用设置")]
    public float minSpeed = 50f;         // 最小速度
    public float maxSpeed = 150f;        // 最大速度
    public float spawnRadius = 0.5f;     // 生成半径（避免重叠）
    public float maxDeviationAngle = 30f; // 最大偏离角度（度，围绕对准方向）
    public float meteorLifetime = 120f;  // 每枚物体的生命周期（秒）

    private Coroutine spawnCoroutine;    // 当前运行的协程引用，用于停止
    private List<GameObject> activeObjects = new List<GameObject>();  // 跟踪当前生成的物体
    private GameManager.GameState lastState;         // 上次状态，用于检测变化

    void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("MeteorSpawner 需要引用 GameManager！");
            return;
        }

        lastState = gameManager.currentState;  // 初始记录状态
        HandleStateChange(gameManager.currentState);  // 初始处理
    }

    void Update()
    {
        // 每帧检查 GameManager 状态变化
        if (gameManager.currentState != lastState)
        {
            HandleStateChange(gameManager.currentState);
            lastState = gameManager.currentState;
        }
    }

    private void HandleStateChange(GameManager.GameState newState)
    {
        // 如果切换到非生成状态（GameOver 或 Victory），停止生成并清理
        if (newState == GameManager.GameState.GameOver || newState == GameManager.GameState.Victory)
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
            ClearActiveObjects();
            return;
        }

        // Menu 或 Playing：切换预制体并清理旧物体（如果模式不同）
        GameObject[] newPrefabs = (newState == GameManager.GameState.Playing) ? gamePrefabs : defaultPrefabs;
        float newInterval = (newState == GameManager.GameState.Playing) ? gameSpawnInterval : defaultSpawnInterval;

        // 停止当前协程
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // 清理之前生成的物体（模式切换时）
        ClearActiveObjects();

        // 启动新模式的无限生成协程
        spawnCoroutine = StartCoroutine(InfiniteSpawn(newPrefabs, newInterval));
    }

    private IEnumerator InfiniteSpawn(GameObject[] prefabs, float interval)
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("预制体数组为空，无法生成！");
            yield break;
        }

        Vector3 spawnPoint = transform.position;

        while (true)  // 无限循环生成
        {
            // 1. 随机生成位置（围绕当前对象位置，小偏移避免重叠）
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPos = spawnPoint + randomOffset;

            // 2. 随机选择预制体（支持多种）
            GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject obj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            activeObjects.Add(obj);  // 跟踪生成的物体

            // 3. 获取 Rigidbody（假设有）
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("预制体缺少 Rigidbody 组件！");
                Destroy(obj);
                activeObjects.Remove(obj);
                yield break;
            }

            // 4. 基于对准 (0,0,0) 的随机方向（锥形偏离）
            Vector3 baseDirection = (Vector3.zero - spawnPos).normalized;
            float deviationAngle = Random.Range(0f, maxDeviationAngle);

            Vector3 randomAxis = Vector3.Cross(baseDirection, Random.onUnitSphere).normalized;
            if (randomAxis == Vector3.zero)
                randomAxis = Vector3.up;

            Quaternion rotation = Quaternion.AngleAxis(deviationAngle, randomAxis);
            Vector3 direction = rotation * baseDirection;

            // 5. 随机速度
            float speed = Random.Range(minSpeed, maxSpeed);
            rb.velocity = direction * speed;

            // 6. 可选：设置旋转（让物体翻滚）
            rb.angularVelocity = Random.insideUnitSphere * 5f;

            // 7. 设置生命周期（自动销毁并从列表移除）
            StartCoroutine(DestroyAfterTime(obj, meteorLifetime));

            // 等待下一轮生成
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (activeObjects.Contains(obj))
        {
            activeObjects.Remove(obj);
        }
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void ClearActiveObjects()
    {
        foreach (GameObject obj in activeObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        activeObjects.Clear();
    }
}