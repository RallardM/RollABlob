// Source : https://youtu.be/Gin9nVJ2nYc
// Source : https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
// Source : https://youtu.be/XYJpDig5s6U
// Source : https://youtu.be/ORD7gsuLivE
// Source : Maxime


using UnityEngine;
using static JellyMesh;

public class BallController : MonoBehaviour
{
    public float m_speed = 10.0f;
    public float m_torque = 20.0f;
    public Rigidbody m_ballRigidbody;
    public Camera m_thirdPersonCamera;

    private JellyMesh m_jellyMesh;
    private float m_direction;
    private bool m_isGrounded = false;

    Vector2 m_input;

    private void Awake()
    {
        m_jellyMesh = GetComponent<JellyMesh>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Object is triggered");

        if (other.gameObject.CompareTag("Jumpable"))
        {
            m_isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Object is not triggered");

        if (other.gameObject.CompareTag("Jumpable"))
        {
            m_isGrounded = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (m_isGrounded)
        {
            //Debug.Log("Object is grounded");
        }

        if (m_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space is pressed");
        }

        if (m_isGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Space is released");
        }

        //if (m_isGrounded && Input.GetKeyDown(KeyCode.Space))
        //{
        //    m_jellyMesh.m_squashing += 0.05f;
        //}

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

        direction.Normalize();

        if (direction.magnitude <= 0)
        {
            return;
        }

        //Debug.Log("Shit hit equals : " + GetIsShiftPressed());
        m_ballRigidbody.AddTorque(direction * m_torque * m_speed * GetIsShiftPressed() * Time.fixedDeltaTime, ForceMode.Force);
    }

    private float GetIsShiftPressed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("Shit hit");
            return 1000f;
        }
        return 1f;
    }
}