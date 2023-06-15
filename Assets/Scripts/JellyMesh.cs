// Source : https://www.youtube.com/watch?v=Kwh4TkQqqf8
// Source : https://youtu.be/mPgODeBDyIw
// Source : https://answers.unity.com/questions/523289/change-size-of-mesh-at-runtime.html
// Source : https://catlikecoding.com/unity/tutorials/mesh-deformation/

using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    public float m_intensity = 1f;
    public float m_stiffness = 1f;
    public float m_damping = 0.75f;
    public float m_squashing = 0f;
    public float m_flattenY = 0f;
    [HideInInspector] public float m_initialSquashing;
    [HideInInspector] public float m_midAirJumpStretching;

    private Rigidbody m_rigidbody;
    private Mesh m_originalMesh, m_meshClone;
    private MeshRenderer m_renderer;
    private JellyVertex[] m_jellyVertex;
    private BallController m_ballController;
    private Vector3[] m_vertexArray;
 
    private float m_prepareJumpSquashing;
    private float m_lerpDuration = 3f;
    private float m_lerpElapsedTime;
    [SerializeField] private bool m_isSquashing = false;

    public bool IsSquashing { get => m_isSquashing; set => m_isSquashing = value; }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_ballController = GetComponent<BallController>();
        m_originalMesh = GetComponent<MeshFilter>().sharedMesh;
        m_meshClone = Instantiate(m_originalMesh);
        GetComponent<MeshFilter>().sharedMesh = m_meshClone;
        m_renderer = GetComponent<MeshRenderer>();
        m_jellyVertex = new JellyVertex[m_meshClone.vertices.Length];
        m_initialSquashing = m_squashing;
        m_prepareJumpSquashing = m_initialSquashing * 10f;
        m_midAirJumpStretching = m_initialSquashing * -10f;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_meshClone.vertices.Length; i++)
        {
            Vector3 vertex = m_meshClone.vertices[i];
            vertex.y = m_flattenY;
            m_meshClone.vertices[i] = vertex;
            m_jellyVertex[i] = new JellyVertex(i, transform.TransformPoint(m_meshClone.vertices[i]));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_vertexArray = m_originalMesh.vertices;

        if (IsSquashing)
        {
            m_squashing = m_prepareJumpSquashing;
        }
        else
        {
            m_lerpElapsedTime += Time.deltaTime;
            float percentageComplete = m_lerpElapsedTime / m_lerpDuration;
            //Debug.Log("Space is released");
            m_squashing = m_initialSquashing;
            m_ballController.HeightBeforeJump = m_rigidbody.transform.position.y;
            //Debug.Log("Height before jump : " + HeightBeforeJump);
            
            m_squashing = Mathf.Lerp(m_squashing, m_midAirJumpStretching, Mathf.SmoothStep(0, 1, percentageComplete));
            Debug.Log("Squashing : " + m_squashing);
            m_ballController.IsGrounded = false;
        }
        
        for (int i = 0; i < m_jellyVertex.Length; i++) 
        {
            Vector3 m_target = transform.TransformPoint(m_vertexArray[m_jellyVertex[i].m_id]);
            float m_newIntensity = (1 - (m_renderer.bounds.max.y - m_target.y) / m_renderer.bounds.size.y) * m_intensity;

            // Calculate the squashing amount based on m_squashing and m_intensity linear interpolation between the two
            // and applies that squashing amount on the player's mesh and collider in their y coordinates
            float squashingAmount = Mathf.Lerp(0f, m_squashing, m_newIntensity);
            m_target.y -= m_target.y * squashingAmount;

            // Flatten the vertices below the mesh
            if (m_target.y < m_flattenY)
            {
                m_target.y = m_flattenY;
            }

            m_jellyVertex[i].Shake(m_target, m_rigidbody.mass, m_stiffness, m_damping);
            m_target = transform.InverseTransformPoint(m_jellyVertex[i].m_position);
            m_vertexArray[m_jellyVertex[i].m_id] = Vector3.Lerp(m_vertexArray[m_jellyVertex[i].m_id], m_target, m_intensity);
        }
        m_meshClone.vertices = m_vertexArray;
    }

    public class JellyVertex
    {
        public int m_id;
        public Vector3 m_position;
        public Vector3 m_velocity, m_force;

        public JellyVertex(int _id, Vector3 _pos)
        {
            m_id = _id;
            m_position = _pos;
        }

        public void Shake(Vector3 _target, float _mass, float _stiffness, float _damping)
        {
            m_force = (_target - m_position) * _stiffness;
            m_velocity = (m_velocity + m_force / _mass) * _damping;
            m_position += m_velocity;
            if ((m_velocity + m_force + m_force / _mass).magnitude < 0.001f)
            {
                m_position = _target;
            }
        }
    }
}