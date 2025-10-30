using UnityEngine;

public class CollisionScoreDeductor : MonoBehaviour
{
    [Header("碰撞设置")]
    [SerializeField] private string targetTag = "Enemy";  // 指定物体的 Tag（在 Inspector 中设置）

    private void OnCollisionEnter(Collision collision)
    {
        // 检查是否为指定物体
        if (collision.gameObject.CompareTag(targetTag))
        {
            // 检查游戏是否在进行中
            if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
            {
                // 扣除 10 分
                GameManager.Instance.AddLives(-1);
                Debug.Log($"被 {targetTag} 撞击，扣除 1 命！");
                
                // 可选：添加其他效果，如销毁或伤害动画
                // Destroy(collision.gameObject);  // 示例：销毁撞击物体
            }
        }
    }
    
}