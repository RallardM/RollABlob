using UnityEngine;

public class BallController : MonoBehaviour
{
    public float m_speed = 0.0f;
    public float m_torque = 10.0f;
    public Rigidbody m_ballRigidbody;

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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
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
