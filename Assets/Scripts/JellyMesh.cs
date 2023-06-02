using UnityEngine;
using UnityEngine.UIElements;

public class JellyMesh : MonoBehaviour
{
    public float m_intensity = 1f;
    public float m_mass = 1f;
    public float m_stiffness = 1f;
    public float m_damping = 0.75f;

    private Mesh m_originalMesh, m_meshClone;
    private MeshRenderer m_renderer;
    private JellyVertex[] m_jellyVertex;
    private Vector3[] m_vertexArray;

    // Start is called before the first frame update
    void Start()
    {
        m_originalMesh = GetComponent<MeshFilter>().sharedMesh;
        m_meshClone = Instantiate(m_originalMesh);
        GetComponent<MeshFilter>().sharedMesh = m_meshClone;
        m_renderer = GetComponent<MeshRenderer>();
        m_jellyVertex = new JellyVertex[m_meshClone.vertices.Length];
        for (int i = 0; i < m_meshClone.vertices.Length; i++)
        {
            m_jellyVertex[i] = new JellyVertex(i, transform.TransformPoint(m_meshClone.vertices[i]));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_vertexArray = m_originalMesh.vertices;
        for (int i = 0; i < m_jellyVertex.Length; i++) 
        {
            Vector3 m_target = transform.TransformPoint(m_vertexArray[m_jellyVertex[i].m_id]);
            float m_newIntensity = (1 - (m_renderer.bounds.max.y - m_target.y) / m_renderer.bounds.size.y) * m_intensity;
            m_jellyVertex[i].Shake(m_target, m_mass, m_stiffness, m_damping);
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
