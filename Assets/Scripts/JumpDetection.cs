
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    private Transform m_playerTransform;
    private JellyMesh m_jellyMesh;
    private BallController m_playerBallController;
    private float m_initialSquashing;

    private void Awake()
    {
        // Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
        m_playerTransform = transform.parent.transform;
        m_playerBallController = m_playerTransform.GetComponent<BallController>();
        m_jellyMesh = m_playerTransform.GetComponent<JellyMesh>();
        m_initialSquashing = m_jellyMesh.m_squashing;
    }

    private void OnTriggerEnter(Collider other)
    {
        // What follows should only be handled when the player hit the floor after a jump.
        // and not for each modular floor tile.
        // Early return if the touched object is not a jumpable object.
        if (other.gameObject.tag != "Jumpable")
        {
            return;
        }

        // Only applies if the player was in the air and is now hitting the ground.
        if (m_playerBallController.IsGrounded)
        {
            return;
        }

        // Update the player state as grounded (touching the ground from jumping).
        m_playerBallController.IsGrounded = true;

        // If (As) the player is still stretched from the jump-strech, we need to reset it.
        if (m_jellyMesh.m_squashing != m_initialSquashing)
        {
            m_jellyMesh.m_squashing = m_initialSquashing;
        }
    }
}
