using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // 游戏状态枚举
    public enum GameState
    {
        Menu,      // 主菜单（游戏未开始）
        Playing,   // 游戏进行中
        GameOver,  // 游戏结束
        Victory    // 胜利（可选）
    }

    [Header("当前状态")]
    public GameState currentState = GameState.Menu;  // 默认主菜单状态

    // 事件：通知其他脚本（如UI、Spawner）状态变化
    public UnityEvent onStateChanged;    // 通用状态变化事件
    public UnityEvent onGameStart;       // 游戏开始（Menu -> Playing）
    public UnityEvent onGameOver;        // 游戏结束事件
    public UnityEvent onVictory;         // 胜利事件

    // 单例模式（可选，便于全局访问）
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 场景切换时保留
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初始状态通知
        NotifyStateChange();
    }

    // 公共方法：切换游戏状态
    public void SetState(GameState newState)
    {
        if (currentState == newState)
            return;  // 避免重复切换

        GameState oldState = currentState;
        currentState = newState;

        // 特定状态处理
        switch (newState)
        {
            case GameState.Playing:
                if (oldState == GameState.Menu)
                {
                    onGameStart?.Invoke();  // 触发游戏开始事件
                }
                break;
            case GameState.GameOver:
                onGameOver?.Invoke();    // 触发游戏结束事件
                break;
            case GameState.Victory:
                onVictory?.Invoke();     // 触发胜利事件
                break;
        }

        // 通用状态变化通知
        NotifyStateChange();
    }

    // 公共方法：获取当前状态（bool 包装，便于旧代码兼容）
    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }

    // 私有方法：通知状态变化（可扩展为事件订阅）
    private void NotifyStateChange()
    {
        onStateChanged?.Invoke();
    }
}