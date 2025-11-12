using UnityEngine;

// 如果需要UI组件，可以导入

public class WorldSpaceUIManager : MonoBehaviour
{
    private void Start()
    {
        // 订阅游戏开始事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameStart.AddListener(HideWorldSpaceUI);
            GameManager.Instance.onGameOver.AddListener(ShowMenuAndResetState);
        }
        else
            Debug.LogWarning("GameManager.Instance 未找到，请确保GameManager已正确初始化！");
    }

    private void OnDestroy()
    {
        // 清理订阅，避免内存泄漏
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameStart.RemoveListener(HideWorldSpaceUI);
            GameManager.Instance.onGameOver.RemoveListener(ShowMenuAndResetState);
        }
    }

    // 事件回调：隐藏当前世界空间UI物体
    private void HideWorldSpaceUI()
    {
        gameObject.SetActive(false);
        Debug.Log("当前世界空间UI已隐藏（游戏开始）");
    }

    // 事件回调：显示菜单UI并重置状态为Menu
    private void ShowMenuAndResetState()
    {
        gameObject.SetActive(true);
        GameManager.Instance.SetState(GameManager.GameState.Menu);
        Debug.Log("菜单UI已显示，状态切换为Menu（游戏结束）");
    }

    // 可选：如果需要手动测试或在其他地方调用
    [ContextMenu("手动隐藏当前世界空间UI")]
    private void ManualHide()
    {
        HideWorldSpaceUI();
    }

    [ContextMenu("手动显示菜单并重置状态")]
    private void ManualShowMenu()
    {
        ShowMenuAndResetState();
    }
}