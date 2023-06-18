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
    private Vector3 m_initialCameraOffset = new Vector3(0, 2, -4);
    private Vector3 m_groundedCameraOffset = new Vector3(0, 2, -4);
    private Vector3 m_cameraOffsetToAdd = new Vector3(0, 0, 0);
    private Vector3 m_currentOffset = new Vector3(0, 2, -4);
    //private float m_jumpCameraOffsetMultiplier = 3.0f;
    private float m_jumpOffsetMaxMultiplier = 1.5f;
    
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
        // Set the camera's position and rotation
        //Debug.Log("transform.position : " + transform.position.magnitude);
        //Debug.Log("GetMouseDesiredPos() : " + GetMouseDesiredPos().magnitude);
        //Debug.Log("Set cam pos rot");
        transform.position = Vector3.Lerp(transform.position, GetMouseDesiredPos(), m_smoothSpeed);
        transform.LookAt(m_ballRigidbody.position);

        if (m_blobAbsorb.PlayerIsResizing)
        {
            // Update the camera position to the new player size
            // so the camera does not stay too close to the player as the player grows
            SetNewCamPosFromPlayerResize();
        }

        //Debug.Log("is resizing? : " + m_blobAbsorb.PlayerIsResizing + " and player diff : " + m_blobAbsorb.GetInitialPLayerSizeDifference() + " >0");
        if (!m_blobAbsorb.PlayerIsResizing && m_blobAbsorb.GetInitialPLayerSizeDifference() > 0.0f)
        {
            //Debug.Log("Update camera pos to accumulated player size");
            UpdateNewCamPosFromPlayerResize();
        }

        // If the player is jumping
        if (m_ballController.IsJumping)
        {
            // Update the camera jumping offset
            // so the player can see the surroundings while jumping
            UpdateCameraJumpingOffset();
            return;
        }

        // Otherwise, reset the camera poistion to initial offset
        ResetCameraPos();
    }

    private void UpdateNewCamPosFromPlayerResize()
    {
        Vector3 previousOffset = m_groundedCameraOffset;
        Vector3 newOffset = Vector3.Lerp(m_groundedCameraOffset, m_cameraOffsetToAdd, Time.deltaTime * m_smoothSpeed);
        m_groundedCameraOffset = newOffset;
        m_cameraOffsetToAdd -= (newOffset - previousOffset);
        if (m_cameraOffsetToAdd.magnitude < 0.0f)
        {
            m_cameraOffsetToAdd = new Vector3(0, 0, 0);
        }
    }

    private void SetNewCamPosFromPlayerResize()
    {
        // Get the player size difference since the beginning of the game
        // so as we use it to offset the camera, the camera does not
        // stay too close to the player as the player grows
        float playerSizeDiff = m_blobAbsorb.GetPLayerSizeDifference();
        //Debug.Log("Update camera pos to new player size");
        // If the player size difference is higher than 0 (the player has absorbed something)
        if (playerSizeDiff > 0.0f)
        {
            //Debug.Log("Player size is diff : " + playerSizeDiff);
            //Debug.Log("Player size diff : " + playerSizeDiff);
            m_cameraOffsetToAdd.y *= playerSizeDiff;
            m_cameraOffsetToAdd.z *= playerSizeDiff;
        }
        //Debug.Log("m_cameraOffsetToAdd : " + m_cameraOffsetToAdd);
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
        Vector3 desiredPosition = m_ballRigidbody.position + rotation * m_currentOffset;
        //Debug.Log("Return desired mouse pos");
        return desiredPosition;
    }

    private void UpdateCameraJumpingOffset()
    {
        // Early return to prevents recursive assignation to m_currentOffset so it does not go too high
        if (m_currentOffset.magnitude > (m_initialCameraOffset.magnitude * m_jumpOffsetMaxMultiplier))
        {
            //Debug.Log("m_currentOffset.magnitude is higher");
            return;
        }
        //Debug.Log("JumpingOffsets");
        Vector3 jumpingCameraOffset = m_currentOffset * m_jumpOffsetMaxMultiplier;
        m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, jumpingCameraOffset, Time.deltaTime * m_lerpSpeed);
    }

    private void ResetCameraPos()
    {
        // Early return to prevents recursive assignation to m_currentOffset so it does not go too high
        //if (m_currentOffset.magnitude < m_initialCameraOffset.magnitude)
        //{
        //    Debug.Log("No reset, currOff : " + m_currentOffset.magnitude + " < " + (m_initialCameraOffset.magnitude * m_jumpOffsetMaxMultiplier));
        //    return;
        //}
        //Debug.Log("ResetOffsets");
        m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, m_groundedCameraOffset, Time.deltaTime * m_lerpSpeed);
    }
}