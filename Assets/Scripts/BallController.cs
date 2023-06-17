// Source : https://youtu.be/Gin9nVJ2nYc
// Source : https://docs.unity3d.com/ScriptReference/Rigidbody.AddTorque.html
// Source : https://youtu.be/XYJpDig5s6U
// Source : https://youtu.be/ORD7gsuLivE
// Source : https://stackoverflow.com/questions/58377170/how-to-jump-in-unity-3d
// Source : https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
// Source : https://youtu.be/MyVY-y_jK1I
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
    private BlobAbsorb m_blobAbsorb;
    private Vector3 m_jumpDirection = Vector3.zero;
    private Vector3 m_previousDirection = Vector3.zero;
    private float m_heightBeforeJump = 0.0f;
    private float m_currentPlayerHeight = 0.0f;
    private float m_initialSquashing;
    private float m_prepareJumpSquashing;
    private float m_midAirJumpStretching;
    private float m_lerpDuration = 3f;
    private float m_lerpElapsedTime;
    private const int JUMPABLE = 10;
    private bool m_isGrounded = false;
    private bool m_isSquashing = false;

    public bool IsGrounded { get => m_isGrounded; set => m_isGrounded = value; }
    public bool IsSquashing { get => m_isSquashing; set => m_isSquashing = value; }
    public float HeightBeforeJump { get => m_heightBeforeJump; set => m_heightBeforeJump = value; }
    public float CurrentPlayerHeight { get => m_currentPlayerHeight; set => m_currentPlayerHeight = value; }

    private void Awake()
    {
        m_thirdPersonCamera = transform.parent.Find("ThirdPersonCamera").gameObject.GetComponent<Camera>();
        m_ballRigidbody = GetComponent<Rigidbody>();
        m_jellyMesh = GetComponent<JellyMesh>();
        m_blobAbsorb = m_ballRigidbody.GetComponentInChildren<BlobAbsorb>();
        m_initialSquashing = m_jellyMesh.m_squashing;
        m_prepareJumpSquashing = m_initialSquashing * 10f;
        m_midAirJumpStretching = m_initialSquashing * -5f;
        m_jumpDirection = new Vector3(0.0f, 1.0f, 0.0f);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    m_lerpElapsedTime += Time.fixedDeltaTime;
    //    float percentageComplete = m_lerpElapsedTime / m_lerpDuration;

    //    if (other.gameObject.layer == JUMPABLE)
    //    {
    //        //Debug.Log("Floor is touched");
    //        m_isGrounded = true;
    //    }

    //    if (m_jellyMesh.m_squashing != m_initialSquashing)
    //    {
    //        m_jellyMesh.m_squashing = m_initialSquashing;
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("OnTriggerStay");

        // If the object is not jumpable, return.
        if (other.gameObject.layer != JUMPABLE)
        {
            //Debug.Log("OnTriggerStay");
            return;
        }

        // If the player is touching the current floor tile, return.
        if (IsGrounded)
        {
            return;
        }

        CurrentPlayerHeight = m_ballRigidbody.transform.position.y;
        Debug.Log("CurrentPlayerHeight: " + CurrentPlayerHeight);
        float playerBallRadius = m_blobAbsorb.GetPlayerCurrentRadius();
        //Debug.Log("playerBallRadius: " + playerBallRadius);
        CurrentPlayerHeight -= playerBallRadius;
        Debug.Log("CurrentHeight: " + CurrentPlayerHeight);
        float colliderHeight = other.transform.position.y;
        
        //Debug.Log("colliderHeight: " + colliderHeight);

        // If the player is in the threshold height of touching the current floor tile.
        if (CurrentPlayerHeight >= colliderHeight && CurrentPlayerHeight + 0.03f <= colliderHeight)
        {
            //Debug.Log("Is on the floor");
            IsGrounded = true;
            Debug.Log("OnTriggerStay IsGrounded: " + IsGrounded);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_jellyMesh.m_squashing != m_initialSquashing)
        {
            m_jellyMesh.m_squashing = m_initialSquashing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object is not jumpable, return.
        if (other.gameObject.layer != JUMPABLE)
        {
            return;
        }

        // If the player realesed jump (IsGrounded = false;) return.
        //if (!IsGrounded) 
        //{
        //    return;
        //}

        CurrentPlayerHeight = m_ballRigidbody.transform.position.y;
        //Debug.Log("CurrentPlayerHeight: " + CurrentPlayerHeight);
        float playerBallRadius = GetComponent<MeshCollider>().bounds.size.y / 2;
        CurrentPlayerHeight -= playerBallRadius;
        float colliderHeight = other.transform.position.y;

        if (CurrentPlayerHeight + 0.04f >= colliderHeight)
        {
            //Debug.Log("Is in the air");
            IsGrounded = false;
            //Debug.Log("OnTriggerExit IsGrounded: " + IsGrounded);
            return;
        }

        //Debug.Log("Left the floor");
        //IsGrounded = true;
    }

    //private void Update()
    //{
    //    m_lerpElapsedTime += Time.deltaTime;
    //    float percentageComplete = m_lerpElapsedTime / m_lerpDuration;
    //    if (IsGrounded && Input.GetKey(KeyCode.Space))
    //    {
    //        //Debug.Log("Space is pressed");
    //        IsSquashing = true;
            
    //    }

    //    if (IsGrounded && Input.GetKeyUp(KeyCode.Space))
    //    {
    //        IsSquashing = false;
    //        m_jellyMesh.m_squashing = m_initialSquashing;
    //        HeightBeforeJump = m_ballRigidbody.transform.position.y;
    //        m_ballRigidbody.AddForce(m_jumpDirection * m_jumpForce, ForceMode.Impulse);
    //        float lerpedSquashing = Mathf.Lerp(m_jellyMesh.m_squashing, m_midAirJumpStretching, Mathf.SmoothStep(0, 1, percentageComplete));
    //        m_jellyMesh.m_squashing = lerpedSquashing;
    //        m_isGrounded = false;
    //    }

    //    if(IsSquashing)
    //    {
    //        m_jellyMesh.m_squashing = m_prepareJumpSquashing;
    //    }
    //    if (!IsSquashing || !(IsGrounded && (Input.GetKey(KeyCode.Space))))
    //    {
    //        m_jellyMesh.m_squashing = m_initialSquashing;
    //    }
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        m_lerpElapsedTime += Time.fixedDeltaTime;
        float percentageComplete = m_lerpElapsedTime / m_lerpDuration;

        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Space is pressed");
            HeightBeforeJump = m_ballRigidbody.transform.position.y;
            m_jellyMesh.m_squashing = m_prepareJumpSquashing;
        }

        if (IsGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Space is released");
            HeightBeforeJump = m_ballRigidbody.transform.position.y;
            //Debug.Log("Space is released");
            m_jellyMesh.m_squashing = m_initialSquashing;
            
            m_ballRigidbody.AddForce(m_jumpDirection * m_jumpForce, ForceMode.Impulse);
            m_jellyMesh.m_squashing = Mathf.Lerp(m_jellyMesh.m_squashing, m_midAirJumpStretching, Mathf.SmoothStep(0, 1, percentageComplete));
            IsGrounded = false;
            //Debug.Log("FixedUpdate IsGrounded: " + IsGrounded);
        }

        Vector3 direction = new Vector3();
        Vector3 lerpDirection = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(1, 0, 0);
            lerpDirection = Vector3.Lerp(m_previousDirection, direction, m_lerpElapsedTime);
            m_previousDirection = new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(0, 0, 1);
            lerpDirection = Vector3.Lerp(m_previousDirection, direction, m_lerpElapsedTime);
            m_previousDirection = new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(-1, 0, 0);
            lerpDirection = Vector3.Lerp(m_previousDirection, direction, m_lerpElapsedTime);
            m_previousDirection = new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += m_thirdPersonCamera.transform.TransformDirection(0, 0, -1);
            lerpDirection = Vector3.Lerp(m_previousDirection, direction, m_lerpElapsedTime);
            m_previousDirection = new Vector3(0, 0, -1);
        }

        direction = Vector3.Lerp(lerpDirection, direction, m_lerpElapsedTime);
        direction.Normalize();

        if (direction.magnitude <= 0)
        {
            return;
        }
        
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