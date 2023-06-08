// Source : https://youtu.be/Gin9nVJ2nYc
// Source : https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
// Source : https://youtu.be/XYJpDig5s6U
// Source : https://youtu.be/ORD7gsuLivE


using UnityEngine;

public class BallController : MonoBehaviour
{
    public float m_speed = 10.0f;
    public float m_torque = 20.0f;
    public Rigidbody m_ballRigidbody;
    public Camera m_thirdPersonCamera;

    private float m_direction;

    Vector2 m_input;

    // Start is called before the first frame update
    //void Start()
    //{
    //    //m_ballRigidbody = GetComponent<Rigidbody>();
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        //ExecuteMovement();
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(1, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(0, 0, 1);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(-1, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(0, 0, -1);
        }

        direction.Normalize();
        if (direction.magnitude <= 0)
        {
            return;
        }
        m_ballRigidbody.AddTorque(direction * m_torque * m_speed * GetIsShiftPressed() * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void ExecuteMovement()
    {
        // Get the direction of where the camera is facing and the direction of where the right of the camera is facing
        //Vector3 camForward = m_thirdPersonCamera.forward;
        //Vector3 camRight = m_thirdPersonCamera.right;

        //amForward.y = 0;
        //camRight.y = 0;
        //camForward = camForward.normalized;
        //camRight = camRight.normalized;

        //Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //Vector3 movement = (camForward * input.y + camRight * input.x).normalized;
        //m_ballRigidbody.AddForce(movement * m_speed);

        //Vector3 vector3 = new Vector3();
        //if (Input.GetKey(KeyCode.W))
        //{
        //    vector3 += new Vector3(1, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    vector3 += new Vector3(0, 0, 1);
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    vector3 += new Vector3(-1, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    vector3 += new Vector3(0, 0, -1);
        //}

        //vector3.Normalize();
        //if (vector3.magnitude <= 0)
        //{
        //    return;
        //}
        //m_ballRigidbody.AddTorque(vector3 * m_torque * Time.fixedDeltaTime, ForceMode.Force);
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

