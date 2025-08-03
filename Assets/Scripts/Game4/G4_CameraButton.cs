using UnityEngine;
using UnityEngine.UI;

public class G4_CameraButton : MonoBehaviour
{
    public int id; // 按钮的唯一标识符
    public G4_CameraController controller; // 目标控制器
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            Debug.Log("Button " + id + " pressed.");
            OnClick();
        }
    }

    private void OnClick()
    {
        if (controller != null)
        {
            controller.ReceiveInput(id); // 将ID传递给控制器
        }
        else
        {
            Debug.LogError("Controller reference is missing in ButtonID.", this);
        }
    }
}