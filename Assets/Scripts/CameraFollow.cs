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
    private Vector3 m_jumpingCameraOffset = new Vector3(0, 1, -5);
    private Vector3 m_currentRigidBodyPos;
    private Vector3 m_initialPlayerSize;
    private float m_mouseX = 0f;
    private float m_mouseY = 0f;

    private void Awake()
    {
        m_thirdPersonCamera = GetComponent<Camera>();
        m_playerBlob = transform.parent.Find("PlayerBlob").gameObject;
        m_ballRigidbody = m_playerBlob.GetComponent<Rigidbody>();
        m_ballController = m_ballRigidbody.GetComponent<BallController>();

        m_blobAbsorb = m_playerBlob.GetComponentInChildren<BlobAbsorb>();
        m_initialPlayerSize = m_ballRigidbody.transform.localScale;
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

        if (playerSizeDiff.y > 0f && m_currentOffset.y < m_initialOffset.y * playerSizeDiff.y)
        {
            //Debug.Log("Player size different: " + playerSizeDifference);
            //Debug.Log("Camera current offset before: " + m_currentOffset);
            m_currentOffset.y = m_initialOffset.y * playerSizeDiff.y;
            m_currentOffset.z = m_initialOffset.z * playerSizeDiff.z;
            //Debug.Log("Camera current offset after: " + m_currentOffset);
            //playerSizeDiff = new Vector3(0, 0, 0);
        }

        // Get the desired position for the camera
        Vector3 desiredPosition = m_ballRigidbody.position + rotation * m_currentOffset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, m_smoothSpeed);
        transform.position = smoothPosition;

        // Set the camera's position and rotation
        transform.position = desiredPosition;
        transform.LookAt(m_ballRigidbody.position);

        Vector3 playerSizeDifference = m_blobAbsorb.GetPLayerSizeDifference();
        float currentJumpHeight;
        m_currentRigidBodyPos = m_ballRigidbody.transform.position;
        currentJumpHeight = m_currentRigidBodyPos.y - m_ballController.PositionBeforeJump.y;
        if (!m_ballController.IsGrounded && currentJumpHeight > 0f)
        {
            //Debug.Log("Player is in the air");
            Vector3 JumpingOffsetResized = Vector3.zero;
            JumpingOffsetResized.y = m_jumpingCameraOffset.y * (currentJumpHeight);
            JumpingOffsetResized.z = m_jumpingCameraOffset.z * (currentJumpHeight);
            Vector3 totalOffset = m_currentOffset + JumpingOffsetResized;
            //totalOffset.y = totalOffset.y * (playerSizeDifference.y * 10);
            //totalOffset.z = totalOffset.z * (playerSizeDifference.z * 10);
            m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, totalOffset, Time.deltaTime);
        }
        else if (m_ballController.IsGrounded)
        {
            //Debug.Log("Player hit the ground");
            m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, m_initialOffset, Time.deltaTime);
        }
    }


}