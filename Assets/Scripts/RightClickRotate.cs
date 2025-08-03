using UnityEngine;

public class RightClickRotate : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                RotateObject();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 调用旋转函数
            RotateObject();
        }
    }

    public void RotateObject()
    {
        transform.Rotate(0, 0, -90f, Space.World);
    }
}