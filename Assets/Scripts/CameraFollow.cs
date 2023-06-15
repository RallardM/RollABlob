// Source : https://forum.unity.com/threads/how-to-get-pitch-and-yaw-of-goal-position-for-camera-orbit-solved.417644/
// Source : https://youtu.be/sNmeK3qK7oA

using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float m_mouseSensitivity = 100f;
    public float m_smoothSpeed = 0.05f;
    public float m_clampCamUp = 90f;
    public float m_clampCamDown = -80f;

    private Camera m_thirdPersonCamera;
    private Rigidbody m_ballRigidbody;
    private GameObject m_playerBlob;
    private BallController m_ballController;
    private BlobAbsorb m_blobAbsorb;
    private Vector3 m_initialOffset = new Vector3(0, 2, -4);
    private Vector3 m_currentOffset = new Vector3(0, 2, -4);
    private Vector3 m_jumpingCameraOffset = new Vector3(0, 0, 0);
    private Vector3 m_currentRigidBodyPos;
    private float m_lerpSpeed = 1f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp
    private float m_mouseX = 0f;
    private float m_mouseY = 0f;

    private void Awake()
    {
        m_thirdPersonCamera = GetComponent<Camera>();
        m_playerBlob = transform.parent.Find("PlayerBlob").gameObject;
        m_ballRigidbody = m_playerBlob.GetComponent<Rigidbody>();
        m_ballController = m_ballRigidbody.GetComponent<BallController>();
        m_blobAbsorb = m_playerBlob.GetComponentInChildren<BlobAbsorb>();
    }

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

        Vector3 playerSizeDiff = m_blobAbsorb.GetPLayerSizeDifference();

        // Offsets the camera as the size of the player changes
        if (playerSizeDiff.y > 0f && m_currentOffset.y < m_initialOffset.y * playerSizeDiff.y)
        {
            m_currentOffset.y = m_initialOffset.y * playerSizeDiff.y;
            m_currentOffset.z = m_initialOffset.z * playerSizeDiff.z;
        }

        // Get the desired position for the camera
        Vector3 desiredPosition = m_ballRigidbody.position + rotation * m_currentOffset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, m_smoothSpeed);
        transform.position = smoothPosition;

        // Set the camera's position and rotation
        transform.position = desiredPosition;
        transform.LookAt(m_ballRigidbody.position);

        float jumpHeightDistance;
        m_currentRigidBodyPos = m_ballRigidbody.transform.position;
        jumpHeightDistance = m_currentRigidBodyPos.y - m_ballController.HeightBeforeJump;
        if (!m_ballController.IsGrounded && jumpHeightDistance > 0f)
        {
            float reducedHeightDistance = jumpHeightDistance * 0.375f;
            m_jumpingCameraOffset = m_currentOffset;
            m_jumpingCameraOffset.y *= Mathf.Clamp(Mathf.Abs(reducedHeightDistance), 0.0f, Mathf.Abs(reducedHeightDistance) * 2.0f);
            m_jumpingCameraOffset.z *= Mathf.Clamp(Mathf.Abs(reducedHeightDistance), 0.0f, Mathf.Abs(reducedHeightDistance) * 2.0f);
            m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, m_jumpingCameraOffset, Time.deltaTime * m_lerpSpeed);
        }
        else if (m_ballController.IsGrounded)
        {
            m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, m_initialOffset, Time.deltaTime * m_lerpSpeed);
        }
    }
}