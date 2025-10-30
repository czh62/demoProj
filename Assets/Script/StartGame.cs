using UnityEngine;

public class StartGame : MonoBehaviour
{
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
