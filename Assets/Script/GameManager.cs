using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
// 用于事件系统

// 用于重载场景

public class GameManager : MonoBehaviour
{
    // 游戏状态枚举
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    [Header("游戏配置")] public int maxHealth = 10; // 最大生命

    public int initialScore; // 初始分数

    // 全局变量（运行时数据）
    [Header("当前状态")] public GameState currentState = GameState.Playing;

    public int currentHealth;
    public int currentScore;
    public int currentWave = 1; // 当前波次
    public int maxWaves = 3; // MVP总波次

    // 事件：通知其他脚本（如UI、Spawner）
    public UnityEvent onScoreChanged; // 分数变事件
    public UnityEvent onHealthChanged; // 生命变事件
    public UnityEvent onWaveChanged; // 波次变事件
    public UnityEvent onGameOver; // 游戏结束事件

    public UnityEvent onVictory; // 胜利事件

    // 单例实例
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // 单例逻辑：确保只有一个
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景不销毁
            InitializeGame(); // 初始化
        }
        else
        {
            Destroy(gameObject); // 多余的销毁
        }
    }

    private void InitializeGame()
    {
        currentHealth = maxHealth;
        currentScore = initialScore;
        currentWave = 1;
        currentState = GameState.Playing;
        Time.timeScale = 1f; // 确保游戏运行
        Debug.Log("游戏初始化：生命=" + currentHealth + ", 波次=1");
    }

    // 加分方法（拦截陨石调用）
    public void AddScore(int amount)
    {
        if (currentState != GameState.Playing) return;
        currentScore += amount;
        onScoreChanged?.Invoke(); // 触发UI更新
        Debug.Log("分数+ " + amount + "，当前: " + currentScore);
    }

    // 扣血方法（陨石撞地球调用）
    public void TakeDamage(int damage = 1)
    {
        if (currentState != GameState.Playing) return;
        currentHealth -= damage;
        onHealthChanged?.Invoke();

        if (currentHealth <= 0)
            GameOver();
        else
            Debug.Log("生命- " + damage + "，剩余: " + currentHealth);
    }

    // 下一波方法（波次结束时调用）
    public void NextWave()
    {
        if (currentState != GameState.Playing) return;
        currentWave++;
        onWaveChanged?.Invoke();

        if (currentWave > maxWaves)
            Victory();
        else
            Debug.Log("进入波次 " + currentWave);
        // 这里可触发Spawner生成新陨石
    }

    // 暂停/恢复
    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;
        }
        else if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
        }
    }

    // 游戏结束
    private void GameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        onGameOver?.Invoke();
        Debug.Log("游戏结束！最终分数: " + currentScore);
    }

    // 胜利
    private void Victory()
    {
        currentState = GameState.Victory;
        Time.timeScale = 0f;
        onVictory?.Invoke();
        Debug.Log("胜利！拦截率高，最终分数: " + currentScore);
    }

    // 重启游戏（UI按钮调用）
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 重载当前场景
    }
}