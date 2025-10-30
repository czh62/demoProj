using UnityEngine;
using System.Collections;  // 用于 Coroutine

public class MeteorSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject meteorPrefab;      // 陨石预制体
    public int meteorCount = 20;         // 生成数量
    public float minSpeed = 50f;         // 最小速度
    public float maxSpeed = 150f;        // 最大速度
    public float spawnRadius = 0.5f;     // 生成半径（避免重叠）
    public float maxDeviationAngle = 30f; // 最大偏离角度（度，围绕对准方向）
    public float spawnInterval = 0.5f;   // 发射间隔（秒，每枚陨石）

    void Start()
    {
        // 启动协程，每隔 spawnInterval 秒发射一枚
        StartCoroutine(SpawnMeteorsOverTime());
    }

    IEnumerator SpawnMeteorsOverTime()
    {
        // 生成点设置为当前对象的位置
        Vector3 spawnPoint = transform.position;

        for (int i = 0; i < meteorCount; i++)
        {
            // 1. 随机生成位置（围绕当前对象位置，小偏移避免重叠）
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPos = spawnPoint + randomOffset;

            // 2. 实例化陨石
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            // 3. 获取 Rigidbody（假设有）
            Rigidbody rb = meteor.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("陨石 Prefab 缺少 Rigidbody 组件！");
                yield break;  // 如果出错，停止协程
            }

            // 4. 基于对准 (0,0,0) 的随机方向（锥形偏离）
            Vector3 baseDirection = (Vector3.zero - spawnPos).normalized;  // 从 spawnPos 到原点的方向
            float deviationAngle = Random.Range(0f, maxDeviationAngle);  // 随机偏离角度

            // 生成垂直于 baseDirection 的随机旋转轴
            Vector3 randomAxis = Vector3.Cross(baseDirection, Random.onUnitSphere).normalized;
            if (randomAxis == Vector3.zero)  // 避免零向量（极少发生）
                randomAxis = Vector3.up;     // 备用轴

            // 旋转 baseDirection 以实现偏离
            Quaternion rotation = Quaternion.AngleAxis(deviationAngle, randomAxis);
            Vector3 direction = rotation * baseDirection;

            // 5. 随机速度
            float speed = Random.Range(minSpeed, maxSpeed);
            rb.velocity = direction * speed;

            // 6. 可选：设置旋转（让陨石翻滚）
            rb.angularVelocity = Random.insideUnitSphere * 5f;  // 随机角速度

            // 7. 可选：设置生命周期（e.g., 120 秒后销毁）
            Destroy(meteor, 120f);

            // 等待下一枚发射（除了最后一枚）
            if (i < meteorCount - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}