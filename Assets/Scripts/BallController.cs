// Source : https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
// Source : https://stackoverflow.com/questions/58377170/how-to-jump-in-unity-3d

// Source : https://stackoverflow.com/questions/5096926/what-is-the-get-set-syntax-in-c

using UnityEngine;
public class BallController : MonoBehaviour
{
    public float m_speed = 10.0f;
    public float m_torque = 20.0f;
    public float m_jumpForce = 2.0f;
    public float m_cameraJumpSpeed = 0.5f;

    private Camera m_thirdPersonCamera;
    private Rigidbody m_ballRigidbody;
    private JellyMesh m_jellyMesh;
    private Vector3 m_jumpDirection = Vector3.zero;
    private float m_heightBeforeJump = 0.0f;
    private float m_initialSquashing;
    private float m_prepareJumpSquashing;
    private float m_midAirJumpStretching;
    private const float m_lerpDuration = 3.0f;
    private float m_lerpElapsedTime;
    private bool m_isGrounded = false;

    public bool IsGrounded { get => m_isGrounded; set => m_isGrounded = value; }
    public float HeightBeforeJump { get => m_heightBeforeJump; set => m_heightBeforeJump = value; }

    private void Awake()
    {
        m_thirdPersonCamera = transform.parent.Find("ThirdPersonCamera").gameObject.GetComponent<Camera>();
        m_ballRigidbody = GetComponent<Rigidbody>();
        m_jellyMesh = GetComponent<JellyMesh>();
        m_initialSquashing = m_jellyMesh.m_squashing;
        m_prepareJumpSquashing = m_initialSquashing * 5.0f;
        m_midAirJumpStretching = m_initialSquashing * -4.0f;
        m_jumpDirection = new Vector3(0.0f, 1.0f, 0.0f);
    }

    // Should only be handled when the player hit the floor after a jump.
    // and not for each modular floor tile.
    private void OnTriggerEnter(Collider other)
    {
        // Early return if the toucing object is not a jumpable object.
        if (other.gameObject.layer == LayerMask.NameToLayer("Jumpable"))
        {
            return;
        }

        // Only applies if the player was in the air and is now hitting the ground.
        if (IsGrounded)
        {
            return;
        }

        // Update the player state as grounded (touching the ground from jumping).
        IsGrounded = true;

        // If (As) the player is still stretched from the jump-strech, we need to reset it.
        if (m_jellyMesh.m_squashing != m_initialSquashing)
        {
            m_jellyMesh.m_squashing = m_initialSquashing;
        }
    }

    private void Update()
    {
        m_lerpElapsedTime += Time.fixedDeltaTime;
        float percentageComplete = m_lerpElapsedTime / m_lerpDuration;

        // If the player touches the ground and presses the space bar, we need to prepare the jump by squaching its blob body.
        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            HeightBeforeJump = m_ballRigidbody.transform.position.y;
            m_jellyMesh.m_squashing = m_prepareJumpSquashing;
        }

        // If the player is releasing the space bar as he is on the ground, it proceed to jump.
        if (IsGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            HeightBeforeJump = m_ballRigidbody.transform.position.y;
            m_jellyMesh.m_squashing = m_initialSquashing;
            // Source : https://stackoverflow.com/questions/58377170/how-to-jump-in-unity-3d
            m_ballRigidbody.AddForce(m_jumpDirection * m_jumpForce, ForceMode.Impulse);
            m_jellyMesh.m_squashing = Mathf.Lerp(m_jellyMesh.m_squashing, m_midAirJumpStretching, Mathf.SmoothStep(0, 1, percentageComplete));
            IsGrounded = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Source : Maxime Flageole and Alexandre Pipon
        m_lerpElapsedTime += Time.fixedDeltaTime;

        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(0, 0, -1);
        }

        if (direction.magnitude <= 0)
        {
            return;
        }

        // Source: https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
        m_ballRigidbody.AddTorque(GetIsShiftPressed() * m_speed * m_torque * Time.fixedDeltaTime * direction, ForceMode.Force);
    }

    // Is called at the add torque, if the player is pressing the shift key, it will increase the speed of the ball.
    private float GetIsShiftPressed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return 10000f;
        }
        return 1f;
    }
}