using UnityEngine;

// 如果需要UI组件，可以导入

public class WorldSpaceUIManager : MonoBehaviour
{
    private void Start()
    {
        // 订阅游戏开始事件
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStart.AddListener(HideWorldSpaceUI);
        else
            Debug.LogWarning("GameManager.Instance 未找到，请确保GameManager已正确初始化！");
    }

    private void OnDestroy()
    {
        // 清理订阅，避免内存泄漏
        if (GameManager.Instance != null) GameManager.Instance.onGameStart.RemoveListener(HideWorldSpaceUI);
    }

    // 事件回调：隐藏当前世界空间UI物体
    private void HideWorldSpaceUI()
    {
        gameObject.SetActive(false);
        Debug.Log("当前世界空间UI已隐藏（游戏开始）");
    }

    // 可选：如果需要手动测试或在其他地方调用
    [ContextMenu("手动隐藏当前世界空间UI")]
    private void ManualHide()
    {
        HideWorldSpaceUI();
    }
}