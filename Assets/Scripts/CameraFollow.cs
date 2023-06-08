// Source : https://forum.unity.com/threads/how-to-get-pitch-and-yaw-of-goal-position-for-camera-orbit-solved.417644/
// Source : https://youtu.be/sNmeK3qK7oA

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform m_targetPalyer;
    public float m_mouseSensitivity = 100f;
    public float m_smoothSpeed = 0.05f;
    public float m_clampCamUp = 90f;
    public float m_clampCamDown = -90f;

    private Vector3 m_offset = new Vector3(0, 2, -4);

    private float m_mouseX = 0f;
    private float m_mouseY = 0f;

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate mouse rotations
        m_mouseX += m_mouseSensitivity * Input.GetAxis("Mouse X");
        m_mouseY -= m_mouseSensitivity * Input.GetAxis("Mouse Y");

        // Clamp the pitch value to avoid flipping the camera
        m_mouseY = Mathf.Clamp(m_mouseY, m_clampCamDown, m_clampCamUp);

        // Calculate the rotation based on the X and Y
        Quaternion rotation = Quaternion.Euler(m_mouseY, m_mouseX, 0f);

        // Get the desired position for the camera
        Vector3 desiredPosition = m_targetPalyer.position + rotation * m_offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, m_smoothSpeed);
        transform.position = smoothPosition;

        // Set the camera's position and rotation
        transform.position = desiredPosition;
        transform.LookAt(m_targetPalyer.position);
    }
}