using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float panSpeed = 20f; // ������ƶ��ٶ�
    [SerializeField]
    private float scrollSpeed; // �����ƶ��ٶ�
    [SerializeField]
    private float panBorderThickness = 10f; // ��Ե���ĺ��

    private Vector3 moveInput;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        // ��������
        moveInput.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // ����Ե�������
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

        // ������λ��
        pos.x += moveInput.normalized.x * panSpeed * Time.deltaTime;
        pos.y += -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
        pos.z += moveInput.normalized.z * panSpeed * Time.deltaTime;

        // ���������λ��
        pos.x = Mathf.Clamp(pos.x, -30, 30);
        pos.y = Mathf.Clamp(pos.y, 5, 30);
        pos.z = Mathf.Clamp(pos.z, -30, 30);

        // Ӧ����λ��
        transform.position = pos;
    }
}
