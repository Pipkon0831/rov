using UnityEngine;

[RequireComponent(typeof(Collider2D))] // 必需2D碰撞器组件
public class Quit : MonoBehaviour
{
    private void OnMouseDown()
    {
        // 检测鼠标左键点击（0-左键，1-右键，2-中键）
        if (Input.GetMouseButtonDown(0))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 编辑器模式下停止运行
#else
            Application.Quit(); // 正式构建版本退出程序
#endif
            
            Debug.Log("正在退出游戏..."); // 添加反馈信息
        }
    }
}