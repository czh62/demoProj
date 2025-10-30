using UnityEngine;
using System.Collections;  // 用于 Coroutine

public class MeteorSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject meteorPrefab;      // 陨石预制体
    public int meteorCount = 20;         // 生成数量
    public float minSpeed = 50f;         // 最小速度
    public float maxSpeed = 150f;        // 最大速度
    public float spawnInterval = 0.5f;   // 发射间隔（秒，每枚陨石）
    [Header("平面设置")]
    public Vector3 planeCenter = Vector3.zero;  // 平面中心位置（默认原点，可设为 transform.position）
    public float planeSizeX = 5f;        // 平面 X 方向尺寸
    public float planeSizeZ = 5f;        // 平面 Z 方向尺寸（XZ 平面）
    public float deviationAngle = 5f;    // 可选小偏离角度（度，避免完全平行；设 0 则无偏离）

    void Start()
    {
        // 可选：如果想用当前对象位置作为平面中心，取消注释下面这行
        // planeCenter = transform.position;

        // 启动协程，每隔 spawnInterval 秒发射一枚
        StartCoroutine(SpawnMeteorsOverTime());
    }

    IEnumerator SpawnMeteorsOverTime()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // 1. 在平面上随机生成位置（XZ 平面，Y 固定为 planeCenter.y）
            float randomX = Random.Range(-planeSizeX / 2f, planeSizeX / 2f);
            float randomZ = Random.Range(-planeSizeZ / 2f, planeSizeZ / 2f);
            Vector3 spawnPos = planeCenter + new Vector3(randomX, 0f, randomZ);

            // 2. 实例化陨石
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            // 3. 获取 Rigidbody（假设有）
            Rigidbody rb = meteor.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("陨石 Prefab 缺少 Rigidbody 组件！");
                yield break;  // 如果出错，停止协程
            }

            // 4. 方向：自动对准原点 (0,0,0)，并可选小偏离
            Vector3 baseDirection = (Vector3.zero - spawnPos).normalized;  // 指向原点

            // 可选：加小偏离（如果 deviationAngle > 0）
            if (deviationAngle > 0f)
            {
                // 生成垂直于 baseDirection 的随机旋转轴
                Vector3 randomAxis = Vector3.Cross(baseDirection, Random.onUnitSphere).normalized;
                if (randomAxis == Vector3.zero)
                    randomAxis = Vector3.up;  // 备用轴

                // 随机小角度偏离
                float angle = Random.Range(0f, deviationAngle);
                Quaternion rotation = Quaternion.AngleAxis(angle, randomAxis);
                baseDirection = rotation * baseDirection;
            }

            Vector3 direction = baseDirection;  // 最终方向

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