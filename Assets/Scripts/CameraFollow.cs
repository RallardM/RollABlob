// Source : https://forum.unity.com/threads/how-to-get-pitch-and-yaw-of-goal-position-for-camera-orbit-solved.417644/
// Source : https://youtu.be/sNmeK3qK7oA

using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float m_mouseSensitivity = 100f;
    public float m_smoothSpeed = 0.5f;
    public float m_clampCamUp = 90f;
    public float m_clampCamDown = -70f;

    private Camera m_thirdPersonCamera;
    private Rigidbody m_ballRigidbody;
    private GameObject m_playerBlob;
    private BallController m_ballController;
    private BlobAbsorb m_blobAbsorb;
    private Vector3 m_groundedCameraOffset = new Vector3(0, 2, -4);
    private Vector3 m_currentOffset = new Vector3(0, 2, -4);
    
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
        if (m_blobAbsorb.PlayerIsResizing)
        {
            // Update the camera position to the new player size
            // so the camera does not stay too close to the player as the player grows
            UpdateCamPosToNewPlayerSize();
        }

        // Set the camera's position and rotation
        transform.position = Vector3.Lerp(transform.position, GetMouseDesiredPos(), m_smoothSpeed);
        transform.LookAt(m_ballRigidbody.position);

        // If the player is jumping
        if (m_ballController.IsJumping && m_ballController.GetJumpCurrentHeight() > 2.0f)
        {
            // Update the camera jumping offset
            // so the player can see the surroundings while jumping
            UpdateCameraJumpingOffset();
            return;
        }

        // Otherwise, reset the camera poistion to initial offset
        ResetCameraPos();
    }

    private void UpdateCamPosToNewPlayerSize()
    {
        // Get the player size difference since the beginning of the game
        // so as we use it to offset the camera, the camera does not
        // stay too close to the player as the player grows
        Vector3 playerSizeDiff = m_blobAbsorb.GetPLayerSizeDifference();

        // If the player size difference is higher than 0 (the player has absorbed something)
        if (playerSizeDiff.y > 1.0f)
        {
            //Debug.Log("Player size diff : " + playerSizeDiff);
            m_groundedCameraOffset.y *= playerSizeDiff.y;
            m_groundedCameraOffset.z *= playerSizeDiff.z;
        }
    }

    private Vector3 GetMouseDesiredPos()
    {
        // Calculate mouse rotations
        m_mouseX += m_mouseSensitivity * Input.GetAxis("Mouse X");
        m_mouseY -= m_mouseSensitivity * Input.GetAxis("Mouse Y");

        // Clamp the pitch value to avoid flipping the camera
        m_mouseY = Mathf.Clamp(m_mouseY, m_clampCamDown, m_clampCamUp);

        // Calculate the rotation based on the X and Y
        Quaternion rotation = Quaternion.Euler(m_mouseY, m_mouseX, 0f);

        // Get the desired position for the camera
        return m_ballRigidbody.position + rotation * m_currentOffset;
    }

    private void UpdateCameraJumpingOffset()
    {
        // Early return to prevents recursive assignation to m_currentOffset to go too high
        //Debug.Log("currOff : " + m_currentOffset.magnitude + " > m_initOff*2 " + (m_groundedCameraOffset.magnitude * 2.0f));
        if (m_currentOffset.magnitude > (m_groundedCameraOffset.magnitude * 2.0f))
        {
            //Debug.Log("m_currentOffset.magnitude is higher");
            return;
        }
        //Debug.Log("m_currentOffset.magnitude is lower");
        //Debug.Log("currOff : " + m_currentOffset.magnitude + " > m_initOff*2 " + (m_groundedCameraOffset.magnitude * 2.0f));
        Vector3 jumpingCameraOffset = m_currentOffset * 2.0f;
        m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, jumpingCameraOffset, Time.deltaTime * m_lerpSpeed);
        //Debug.Log("JumpingOffset : " + m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset);
    }

    private void ResetCameraPos()
    {
        // taking in account the player size difference
        m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, m_groundedCameraOffset, Time.deltaTime * m_lerpSpeed);
        //Debug.Log("FloorOffset : " + m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset);
    }
}