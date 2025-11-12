using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // 游戏状态枚举
    public enum GameState
    {
        Menu, // 主菜单（游戏未开始）
        Playing, // 游戏进行中
        GameOver, // 游戏结束
        Victory // 胜利（可选）
    }

    // ==================== 状态系统 ====================
    [Header("状态系统")] [SerializeField] private GameState currentState = GameState.Menu; // 当前状态（私有，避免外部直接修改）

    [Header("状态变化事件")] public UnityEvent onStateChanged; // 通用状态变化事件

    public UnityEvent onGameStart; // 游戏开始（Menu -> Playing）
    public UnityEvent onGameOver; // 游戏结束事件
    public UnityEvent onVictory; // 胜利事件

    // ==================== 难度系统 ====================
    [Header("难度系统")] [SerializeField] private float difficulty = 0.5f; // 当前难度值（作为分数乘数，私有，避免外部直接修改）

    public UnityEvent<float> onDifficultyChanged; // 难度变化事件（参数：新难度值）

    // ==================== 分数系统 ====================
    [Header("分数系统")] [SerializeField] private int score; // 当前分数（私有，避免外部直接修改）

    public UnityEvent<int> onScoreChanged; // 分数变化事件（参数：新分数）

    // ==================== 生命系统 ====================
    [Header("生命系统")] [SerializeField] private int lives = 10; // 当前生命值（初始 10，私有，避免外部直接修改）

    public UnityEvent<int> onLivesChanged; // 生命变化事件（参数：新生命值）
    public UnityEvent onLivesDepleted; // 生命耗尽事件（可选，用于额外处理，如音效）

    // ==================== 单例系统 ====================
    public static GameManager Instance { get; private set; }

    // ==================== 核心生命周期方法 ====================
    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时保留
            ResetGameData(); // 初始化游戏数据（分数、生命和难度）
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始状态通知
        NotifyStateChange();
    }

    // 状态系统方法
    // 公共方法：切换游戏状态
    public void SetState(GameState newState)
    {
        if (currentState == newState)
            return; // 避免重复切换

        var oldState = currentState;
        currentState = newState;

        // 特定状态处理
        switch (newState)
        {
            case GameState.Playing:
                if (oldState == GameState.Menu)
                {
                    ResetGameData(); // 游戏开始时重置分数和生命
                    onGameStart?.Invoke(); // 触发游戏开始事件
                }
                break;
            case GameState.GameOver:
                onGameOver?.Invoke(); // 触发游戏结束事件
                break;
            case GameState.Victory:
                onVictory?.Invoke(); // 触发胜利事件
                break;
        }

        // 通用状态变化通知
        NotifyStateChange();
    }

    // 公共方法：检查是否在游戏进行中
    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }

    // 获取当前状态（只读）
    public GameState GetState()
    {
        return currentState;
    }

    // 难度系统方法
    // 公共方法：设置难度值（直接设置，并触发事件；不检查状态，便于重置）
    public void SetDifficulty(float newDifficulty)
    {
        difficulty = newDifficulty;
        onDifficultyChanged?.Invoke(difficulty); // 触发难度变化事件
        Debug.Log($"难度设置：{newDifficulty}，当前难度：{difficulty}");
    }

    // 获取当前难度（只读）
    public float GetDifficulty()
    {
        return difficulty;
    }

    // 分数系统方法
    // 公共方法：添加分数（可正可负，应用难度乘数）
    public void AddScore(int amount)
    {
        if (!IsPlaying()) return; // 只在游戏进行中更新分数

        var multipliedAmount = Mathf.RoundToInt(amount * (difficulty + 1));
        score += multipliedAmount;
        onScoreChanged?.Invoke(score); // 触发分数变化事件
        Debug.Log($"分数更新：{multipliedAmount} (x{difficulty + 1})，当前分数：{score}");
    }

    // 公共方法：设置当前分数（直接设置，并触发事件；不检查状态，便于重置，不应用乘数）
    public void SetScore(int newScore)
    {
        score = newScore;
        onScoreChanged?.Invoke(score); // 触发分数变化事件
        Debug.Log($"分数设置：{newScore}，当前分数：{score}");
    }

    // 获取当前分数（只读）
    public int GetScore()
    {
        return score;
    }

    // 生命系统方法
    // 公共方法：添加生命值（可正可负，通常 -1）
    public void AddLives(int amount)
    {
        if (!IsPlaying()) return; // 只在游戏进行中更新生命值

        lives += amount;
        onLivesChanged?.Invoke(lives); // 触发生命变化事件
        Debug.Log($"生命更新：{amount}，当前生命：{lives}");

        // 如果生命耗尽，触发游戏结束
        if (lives <= 0)
        {
            onLivesDepleted?.Invoke(); // 触发生命耗尽事件（可选，用于额外处理，如音效）
            SetState(GameState.GameOver); // 自动切换到游戏结束状态
        }
    }

    // 公共方法：设置当前生命（直接设置，并触发事件；不检查状态，便于重置）
    public void SetLives(int newLives)
    {
        lives = newLives;
        onLivesChanged?.Invoke(lives); // 触发生命变化事件
        Debug.Log($"生命设置：{newLives}，当前生命：{lives}");

        // 如果生命耗尽，触发游戏结束
        if (lives <= 0)
        {
            onLivesDepleted?.Invoke(); // 触发生命耗尽事件（可选，用于额外处理，如音效）
            SetState(GameState.GameOver); // 自动切换到游戏结束状态
        }
    }

    // 获取当前生命（只读）
    public int GetLives()
    {
        return lives;
    }

    // ==================== 私有辅助方法 ====================
    // 私有方法：重置游戏数据（分数、生命和难度）
    private void ResetGameData()
    {
        score = 0;
        lives = 10;
        onScoreChanged?.Invoke(score); // 通知分数重置
        onLivesChanged?.Invoke(lives); // 通知生命重置
    }

    // 私有方法：通知状态变化（可扩展为事件订阅）
    private void NotifyStateChange()
    {
        onStateChanged?.Invoke();
    }
}