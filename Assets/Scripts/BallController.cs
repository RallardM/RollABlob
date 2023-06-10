// Source : https://youtu.be/Gin9nVJ2nYc
// Source : https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
// Source : https://youtu.be/XYJpDig5s6U
// Source : https://youtu.be/ORD7gsuLivE
// Source : https://stackoverflow.com/questions/58377170/how-to-jump-in-unity-3d
// Source : https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
// Source : Maxime


using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static JellyMesh;

public class BallController : MonoBehaviour
{
    public float m_speed = 10.0f;
    public float m_torque = 20.0f;
    public float m_jumpForce = 2.0f;
    public float m_cameraJumpSmoothness = 0.1f;
    public float m_jumpSquashSmoothness = 0.1f;

    private Camera m_thirdPersonCamera;
    private Rigidbody m_ballRigidbody;
    private JellyMesh m_jellyMesh;
    private CameraFollow m_cameraFollow;
    private Vector3 m_jumpDirection;
    private Vector3 m_initialCameraOffset;
    private Vector3 m_jumpingCameraOffset = new Vector3(0, 3, -9);
    private float m_initialSquashing;
    private float m_prepareJumpSquashing;
    private float m_midAirJumpStretching;
    private bool m_isGrounded = false;

    private void Awake()
    {
        m_thirdPersonCamera = transform.parent.Find("ThirdPersonCamera").gameObject.GetComponent<Camera>();
        m_cameraFollow = m_thirdPersonCamera.GetComponent<CameraFollow>();
        m_initialCameraOffset = m_cameraFollow.m_offset;
        m_ballRigidbody = GetComponent<Rigidbody>();
        m_jellyMesh = GetComponent<JellyMesh>();
        m_initialSquashing = m_jellyMesh.m_squashing;
        m_prepareJumpSquashing = m_initialSquashing * 10f;
        m_midAirJumpStretching = m_initialSquashing * -5f;
        m_jumpDirection = new Vector3(0.0f, 1.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.transform.parent.parent.name;
        //Debug.Log("Object entered : " + other.transform.parent.parent.name);

        //if (other.transform.parent.parent.name == "Floor")
        if (other.transform.name == "Desert")
        {
            //Debug.Log("Object entered Jumpable : " + other.name);
            m_isGrounded = true;
        }

        if (m_jellyMesh.m_squashing != m_initialSquashing)
        {
            m_jellyMesh.m_squashing = m_initialSquashing;
        }

        if (m_thirdPersonCamera.GetComponent<CameraFollow>().m_offset != m_initialCameraOffset)
        {
            m_thirdPersonCamera.GetComponent<CameraFollow>().m_offset = Vector3.Lerp(m_cameraFollow.m_offset, m_initialCameraOffset, m_cameraJumpSmoothness);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Object is not triggered");

        //if (other.gameObject.CompareTag("Jumpable"))
        if (other.transform.name == "Desert")
        {
            m_isGrounded = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space is pressed");
            m_jellyMesh.m_squashing = m_prepareJumpSquashing;
        }

        if (m_isGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Space is released");
            m_jellyMesh.m_squashing = m_initialSquashing;
            m_ballRigidbody.AddForce(m_jumpDirection * m_jumpForce, ForceMode.Impulse);
            m_thirdPersonCamera.GetComponent<CameraFollow>().m_offset = Vector3.Lerp(m_cameraFollow.m_offset, m_jumpingCameraOffset, m_cameraJumpSmoothness);
            m_jellyMesh.m_squashing = Mathf.Lerp(m_jellyMesh.m_squashing, m_midAirJumpStretching, m_jumpSquashSmoothness);
            m_isGrounded = false;
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