using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public Transform m_targetPalyer;
    public float m_mouseSensitivity = 100f;
    public float m_smoothSpeed = 1.0f;
    public float m_clampCamUp = 90f;
    public float m_clampCamDown = -90f;
    public Vector3 m_offset;

    private float m_mouseX = 0f;
    private float m_mouseY = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate mouse rotations
        m_mouseX += m_mouseSensitivity * Input.GetAxis("Mouse X");
        m_mouseY -= m_mouseSensitivity * Input.GetAxis("Mouse Y");

        // Clamp the pitch value to avoid flipping the camera
        m_mouseY = Mathf.Clamp(m_mouseY, m_clampCamDown, m_clampCamUp);

        // Calculate the rotation based on yaw and pitch
        Quaternion rotation = Quaternion.Euler(m_mouseY, m_mouseX, 0f);

        // Get the desired position for the camera
        Vector3 desiredPosition = m_targetPalyer.position + rotation * m_offset;

        // Set the camera's position and rotation
        transform.position = desiredPosition;
        transform.LookAt(m_targetPalyer.position);
    }
}

// Source : https://forum.unity.com/threads/how-to-get-pitch-and-yaw-of-goal-position-for-camera-orbit-solved.417644/