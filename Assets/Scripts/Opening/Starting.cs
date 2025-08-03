using UnityEngine;

[RequireComponent(typeof(Collider2D))] // 确保物体有碰撞器用于点击检测
public class Starting : MonoBehaviour
{
    [Tooltip("需要过渡新场景的名称")]
    public string newSceneName;

    private void OnMouseDown()
    {
        // 检测左键点击（0表示左键，1右键，2中键）
        if (Input.GetMouseButtonDown(0))
        {
            // 确保场景加载器实例存在
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.TransitionToScene(newSceneName);
            }
            else
            {
                Debug.LogError("SceneLoader实例未找到！请确保场景加载器已正确设置。");
            }
        }
    }
}