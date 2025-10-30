using UnityEngine;

public class SpeedCollisionDestroyer : MonoBehaviour
{
    [Header("碰撞设置")]
    public float minRelativeSpeed = 50f;
    public GameObject flashPrefab;
    public GameObject firePrefab;

    void OnCollisionEnter(Collision collision)
    {
        float relativeSpeed = collision.relativeVelocity.magnitude;

        if (relativeSpeed > minRelativeSpeed)
        {
            // 检查游戏是否在进行中（可选，避免菜单或结束时加分）
            if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
            {
                GameManager.Instance.AddScore(10);  // 每次有效碰撞增加 10 分
                Debug.Log("销毁加分：+10，当前分数：" + GameManager.Instance.GetScore());
            }

            ContactPoint contact = collision.contacts[0];
            Vector3 collisionPoint = contact.point;

            // 修改：使用 identity 旋转，让局部 Y 轴对齐全局 Y 轴（默认无旋转）
            Quaternion defaultRotation = Quaternion.identity;  // 或 Quaternion.Euler(0, 0, 0)

            if (flashPrefab != null)
            {
                GameObject flash = Instantiate(flashPrefab, collisionPoint, defaultRotation);
            }

            if (firePrefab != null)
            {
                GameObject fire = Instantiate(firePrefab, collisionPoint, defaultRotation);
            }

            Destroy(gameObject);
        }
    }
}