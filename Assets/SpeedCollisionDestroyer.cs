using UnityEngine;

public class SpeedCollisionDestroyer : MonoBehaviour
{
    [Header("碰撞设置")]
    public float minRelativeSpeed = 5f;  // 最小相对速度阈值（m/s）
    public GameObject flashPrefab;       // 闪光粒子预制体
    public GameObject firePrefab;        // 火光粒子预制体

    void OnCollisionEnter(Collision collision)
    {
        // 计算相对速度大小
        float relativeSpeed = collision.relativeVelocity.magnitude;

        if (relativeSpeed > minRelativeSpeed)
        {
            // 获取碰撞点（第一个接触点）
            ContactPoint contact = collision.contacts[0];
            Vector3 collisionPoint = contact.point;

            // 统一向上旋转（朝向世界 Y 轴上方）
            Quaternion upwardRotation = Quaternion.LookRotation(Vector3.up);

            // 生成闪光效果（及时、短暂，朝上）
            if (flashPrefab != null)
            {
                GameObject flash = Instantiate(flashPrefab, collisionPoint, upwardRotation);
            }

            // 生成火光效果（持续燃烧，朝上）
            if (firePrefab != null)
            {
                GameObject fire = Instantiate(firePrefab, collisionPoint, upwardRotation);
            }

            // 销毁当前物体
            Destroy(gameObject);
        }
    }
}