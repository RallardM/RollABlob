// Source : https://answers.unity.com/questions/1574465/error-cannot-convert-quaternion-to-vector3.html

using UnityEngine;

public class EatenAsset : MonoBehaviour
{
    GameObject m_playerNode;
    private float m_lerpSpeed = 2f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp
    private float m_safeDistanceBeforeBeingEaten = 1f;
    private bool m_isBeingEaten = false;
    private bool m_isEaten = false;

    public bool IsBeingEaten { get => m_isBeingEaten; set => m_isBeingEaten = value; }

    private void Awake()
    {
        m_playerNode = GameObject.FindGameObjectWithTag("PlayerBlob");
        float radius = 0.0f;
        float radiusSurface = 0.0f;
        float sphereVolume = 0.0f;
        float colliderHeight = 0.0f;
        float colliderWidth = 0.0f;
        float colliderDepth = 0.0f;
        float colliderVolume = 0.0f;
        
        // Checks what is the current object collider to assign it's mass based on its collider volume
        if (GetComponent<CapsuleCollider>())
        {
            radius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;
            radiusSurface = (radius * radius) * Mathf.PI;
            sphereVolume = ((radius * radius * radius) * Mathf.PI) * (4/3);
            colliderVolume = (radiusSurface * colliderHeight) + sphereVolume;
            GetComponent<Rigidbody>().mass = colliderVolume;
        }
        else if (GetComponent<SphereCollider>())
        {
            radius = GetComponent<SphereCollider>().radius;
            sphereVolume = ((radius * radius * radius) * Mathf.PI) * (4 / 3);
            GetComponent<Rigidbody>().mass = sphereVolume;
        }
        else if (GetComponent<BoxCollider>())
        {
            colliderHeight = GetComponent<BoxCollider>().size.y;
            colliderWidth = GetComponent<BoxCollider>().size.x;
            colliderDepth = GetComponent<BoxCollider>().size.z;
            colliderVolume = colliderHeight * colliderWidth * colliderDepth;
            GetComponent<Rigidbody>().mass = colliderVolume;
        }
        // Source : https://docs.unity3d.com/ScriptReference/Collider-bounds.html
        else if (GetComponent<MeshCollider>())
        {
            colliderHeight = GetComponent<MeshCollider>().bounds.size.y;
            colliderWidth = GetComponent<MeshCollider>().bounds.size.x;
            colliderDepth = GetComponent<MeshCollider>().bounds.size.z;
            colliderVolume = colliderWidth * colliderHeight * colliderDepth;
            GetComponent<Rigidbody>().mass = colliderVolume;
        }
    }

    void FixedUpdate()
    {
        if (m_isBeingEaten)
        {
            //transform.position = Vector3.Lerp(transform.position, m_playerNode.transform.position, Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, m_playerNode.transform.position) < m_safeDistanceBeforeBeingEaten)
            {
                m_isBeingEaten = false;
                m_isEaten = true;
            }
            else 
            {
                transform.position = Vector3.Lerp(transform.position, m_playerNode.transform.position, Time.fixedDeltaTime / m_lerpSpeed);

                // Source : https://answers.unity.com/questions/1574465/error-cannot-convert-quaternion-to-vector3.html
                transform.rotation = Quaternion.Lerp(transform.rotation, m_playerNode.transform.rotation, Time.fixedDeltaTime / m_lerpSpeed);
            }
        }

        if (m_isEaten)
        {
            transform.position = m_playerNode.transform.position;
            transform.rotation = m_playerNode.transform.rotation;
        }
    }
}
