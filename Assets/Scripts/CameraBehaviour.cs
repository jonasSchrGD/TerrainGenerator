using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private float m_ScrollSpeed = 500.0f;
    [SerializeField]
    private float m_PanSpeed = 25;
    [SerializeField]
    private float m_RotateSpeed = 25;

    //private float m_XBorder = 0.1f * Screen.width;
    //private float m_YBorder = 0.1f * Screen.height;


    // Update is called once per frame
    void Update()
    {
        float xSpeed = 0.0f;
        float ySpeed = 0.0f;

        if (/*Input.mousePosition.x < m_XBorder ||*/ Input.GetKey(KeyCode.A))
            xSpeed = -1;
        else if (/*Input.mousePosition.x > Screen.width - m_XBorder ||*/ Input.GetKey(KeyCode.D))
            xSpeed = 1;

        if (/*Input.mousePosition.y < m_YBorder ||*/ Input.GetKey(KeyCode.S))
            ySpeed = -1;
        else if (/*Input.mousePosition.y > Screen.height - m_YBorder ||*/ Input.GetKey(KeyCode.W))
            ySpeed = 1;

        transform.Translate(
            Time.deltaTime * m_PanSpeed * xSpeed,
            Time.deltaTime * m_ScrollSpeed * - Input.GetAxis("Mouse ScrollWheel"),
            Time.deltaTime * m_PanSpeed * ySpeed,
            Space.Self
            );

        int rotateDirection = 0;
        if (Input.GetKey(KeyCode.Q))
            rotateDirection = -1;
        else if (Input.GetKey(KeyCode.E))
            rotateDirection = 1;

        transform.Rotate(0, rotateDirection * m_RotateSpeed * Time.deltaTime, 0, Space.World);
    }
}
