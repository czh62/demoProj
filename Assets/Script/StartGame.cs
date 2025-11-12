using UnityEngine;

public class StartGame : MonoBehaviour
{
    public void OnDifficultySliderChanged(float value)
    {
        // 根据滑块值更新难度
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(value);
        }
        else
        {
            Debug.LogError("GameManager not found! 请确保场景中有 GameManager。");
        }
        Debug.Log($"难度滑块变化：{value}，难度已更新...");
    }
    public void OnStartButtonClick()
    {
        // 切换 GM 状态为 Playing
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameManager.GameState.Playing);
        }
        else
        {
            Debug.LogError("GameManager not found! 请确保场景中有 GameManager。");
        }
        Debug.Log("Play Button Clicked! Game starting...");
    }
}
