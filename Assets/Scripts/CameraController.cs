using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float panSpeed = 20f; // 相机的移动速度
    [SerializeField]
    private float scrollSpeed; // 滚轮移动速度
    [SerializeField]
    private float panBorderThickness = 10f; // 边缘检测的厚度

    private Vector3 moveInput;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        // 键盘输入
        moveInput.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // 鼠标边缘检测输入
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            moveInput.x = 1;
        }
        else if (Input.mousePosition.x <= panBorderThickness)
        {
            moveInput.x = -1;
        }

        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            moveInput.z = 1;
        }
        else if (Input.mousePosition.y <= panBorderThickness)
        {
            moveInput.z = -1;
        }

        // 计算新位置
        pos.x += moveInput.normalized.x * panSpeed * Time.deltaTime;
        pos.y += -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
        pos.z += moveInput.normalized.z * panSpeed * Time.deltaTime;

        // 限制相机的位置
        pos.x = Mathf.Clamp(pos.x, -30, 30);
        pos.y = Mathf.Clamp(pos.y, 5, 30);
        pos.z = Mathf.Clamp(pos.z, -30, 30);

        // 应用新位置
        transform.position = pos;
    }
}
