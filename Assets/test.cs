using UnityEngine;

public class test : MonoBehaviour
{

    void Start()
    {
    }

    public void OnDropdownChanged(int index)  // 这个函数会被事件调用
    {
        Debug.Log("选中了第 " + index + " 个选项！");  // 示例：打印索引
        // 这里加逻辑，比如切换颜色：GetComponent<Renderer>().material.color = Color.red;
    }
}