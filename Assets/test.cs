using TMPro;
using UnityEngine;
// Button 需要

// TMP_Text 需要

public class Test : MonoBehaviour
{
    [Header("TMP 文本引用（可选）")] public TMP_Text buttonText; // 拖拽 Button 下的 TMP_Text

    [Header("游戏管理器引用")] public GameManager gameManager; // 你的 GameManager 单例
    
}