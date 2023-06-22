// Source : https://forum.unity.com/threads/how-to-get-pitch-and-yaw-of-goal-position-for-camera-orbit-solved.417644/
// Source : https://youtu.be/sNmeK3qK7oA

using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public float m_mouseSensitivity = 100f;
    public float m_smoothSpeed = 0.5f;

    private const float m_clampCamUp = 65.0f;
    private const float m_clampCamDown = -15.0f;
    private Camera m_thirdPersonCamera;
    private Rigidbody m_ballRigidbody;
    private GameObject m_playerBlob;
    private BallController m_ballController;
    private BlobAbsorb m_blobAbsorb;
    private Vector3 m_initialOffset = new Vector3(0, 3, -5);
    private Vector3 m_currentOffset = new Vector3(0, 3, -5);

    private const float m_lerpSpeed = 1f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp
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
        // Update the camera position to the new player size
        // so the camera does not stay too close to the player as the player grows
        UpdateCamPosToNewPlayerSize();

        // Set the camera's position and rotation
        transform.position = Vector3.Lerp(transform.position, GetMouseDesiredPos(), m_smoothSpeed);

        transform.LookAt(m_ballRigidbody.position);

        // If the player jumped (is not grounded)
        if (!m_ballController.IsGrounded)
        {
            // Update the camera jumping offset
            // so the player can see the surroundings while jumping
            UpdateCamJumpingOffset();
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
        float playerSizeDiff = m_blobAbsorb.GetPLayerSizeDifference();

        // Offsets the camera as the size of the player changes
        // If the player size difference is higher than 0 (the player has absorbed something)
        // and the current camera offset height is lower than
        // the initial camera offset height in ratio to the player size difference
        if (playerSizeDiff > 0f && m_currentOffset.y < m_initialOffset.y * playerSizeDiff)
        {
            m_currentOffset.y = m_initialOffset.y * playerSizeDiff;
            m_currentOffset.z = m_initialOffset.z * playerSizeDiff;
        }
    }

    private Vector3 GetMouseDesiredPos()
    {
        // Calculate mouse rotations
        m_mouseX += m_mouseSensitivity * Input.GetAxis("Mouse X");
        m_mouseY -= m_mouseSensitivity * Input.GetAxis("Mouse Y");

        // Clamp the pitch value to avoid flipping the camera
        m_mouseY = Mathf.Clamp(m_mouseY, m_clampCamDown,  m_clampCamUp);

        // Calculate the rotation based on the X and Y
        Quaternion rotation = Quaternion.Euler(m_mouseY, m_mouseX, 0f);

        // Get the desired position for the camera
        return m_ballRigidbody.position + rotation * m_currentOffset;
    }

    private float GetJumpHeight()
    {
        return m_ballRigidbody.transform.position.y - m_ballController.HeightBeforeJump;
    }

    private void UpdateCamJumpingOffset()
    {
        // Update the camera jumping offset to the possibly new current offset
        Vector3 jumpingCameraOffset = m_currentOffset;

        // Use the jumping offset to offset the camera
        float camOffsetFromJump = Mathf.Abs(GetJumpHeight() * 0.375f);

        jumpingCameraOffset.y *= camOffsetFromJump;
        jumpingCameraOffset.z *= camOffsetFromJump;

        if (jumpingCameraOffset.magnitude > m_initialOffset.magnitude * 5)
        {
            return;
        }

        // Update the current offset to the new jumping offset
        m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, jumpingCameraOffset, Time.deltaTime * m_lerpSpeed);
    }

    private void ResetCameraPos()
    {
        // taking in account the player size difference
        float intialPlayerSizeDiff = m_blobAbsorb.GetPLayerSizeDifference();
        
        if (intialPlayerSizeDiff == 0)
        {
            intialPlayerSizeDiff = 1.0f;
        }

        Vector3 newPlayerSizeOffset = new Vector3(0, 0, 0);

        newPlayerSizeOffset.y = intialPlayerSizeDiff * m_initialOffset.y;
        newPlayerSizeOffset.z = intialPlayerSizeDiff * m_initialOffset.z;
        m_thirdPersonCamera.GetComponent<CameraFollow>().m_currentOffset = Vector3.Lerp(m_currentOffset, newPlayerSizeOffset, Time.deltaTime * m_lerpSpeed);
    }
}