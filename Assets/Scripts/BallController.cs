using UnityEngine;

public class BallController : MonoBehaviour
{
    public float m_speed = 0.0f;
    public float m_torque = 10.0f;
    public Rigidbody m_ballRigidbody;
    public Transform m_thirdPersonCamera;

    private float m_direction;

    Vector2 m_input;

    // Start is called before the first frame update
    void Start()
    {
        m_ballRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteMovement();
    }

    private void ExecuteMovement()
    {
        // Get the direction of where the camera is facing and the direction of where the right of the camera is facing
        Vector3 camForward = m_thirdPersonCamera.forward;
        Vector3 camRight = m_thirdPersonCamera.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 movement = (camForward * input.y + camRight * input.x).normalized;
        m_ballRigidbody.AddForce(movement * m_speed);

        if (Input.GetKey(KeyCode.A))
        {
            m_ballRigidbody.AddTorque(transform.up * -m_torque);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_ballRigidbody.AddTorque(transform.up * m_torque);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            m_ballRigidbody.AddTorque(transform.right * -m_torque);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_ballRigidbody.AddTorque(transform.right * m_torque);
        }
    }

    private float GetIsShiftPressed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return 2f;
        }
        return 1f;
    }

}

// Source : https://youtu.be/Gin9nVJ2nYc
// Source : https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
// Source : https://youtu.be/XYJpDig5s6U
// Source : https://youtu.be/ORD7gsuLivE