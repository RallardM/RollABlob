// Source : https://answers.unity.com/questions/1574465/error-cannot-convert-quaternion-to-vector3.html
// Source : https://docs.unity3d.com/ScriptReference/Vector3.Distance.html
// Source : https://forum.unity.com/threads/need-to-change-a-transform-position-y-value-with-a-in-c-close-but-no-cigar.169050/
// Source : https://answers.unity.com/questions/1936597/how-do-you-rotate-an-object-relative-to-another-wi.html

using UnityEngine;

public class EatenAsset : MonoBehaviour
{
    private GameObject m_playerBlob;
    private Vector3 m_distanceToPlayer;
    private Quaternion m_AssetRotation;
    private const float m_lerpSpeed = 0.125f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp
    private bool m_isBeingEaten = false;
    private bool m_isEaten = false;

    public bool IsBeingEaten { get => m_isBeingEaten; set => m_isBeingEaten = value; }
    public bool IsEaten { get => m_isEaten; set => m_isEaten = value; }

    private void Awake()
    {
        m_playerBlob = GameObject.FindGameObjectWithTag("PlayerBlob");
        GameObject blobFullBodyTrigger = m_playerBlob.transform.Find("FullBodyTrigger").gameObject;

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
            sphereVolume = ((radius * radius * radius) * Mathf.PI) * (4 / 3);
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
        // BlobAsorb script sets if the object touching the player as IsBeingEaten
        if (IsBeingEaten && !IsEaten)
        {
            PrepareAssetToBeEaten();
        }

        // Early return allows only eaten assets
        if (!IsEaten)
        {
            return;
        }

        LerpAssetToPlayer();
    }

    private void PrepareAssetToBeEaten()
    {
        // Source : https://docs.unity3d.com/ScriptReference/Vector3.Distance.html
        // Source : https://forum.unity.com/threads/need-to-change-a-transform-position-y-value-with-a-in-c-close-but-no-cigar.169050/

        // Prepare the asset's position and rotation before being synced with the player's
        Vector3 assetPreviousPosition = transform.position;
        //Quaternion assetPreviousRotation = transform.rotation;
        m_distanceToPlayer = assetPreviousPosition - m_playerBlob.transform.position;
        m_AssetRotation = transform.rotation;
        IsBeingEaten = false;
        IsEaten = true;
    }

    private void LerpAssetToPlayer()
    {
        m_distanceToPlayer = Vector3.Lerp(m_distanceToPlayer, Vector3.zero, Time.fixedDeltaTime * m_lerpSpeed);
        m_AssetRotation = Quaternion.Lerp(m_AssetRotation, m_playerBlob.transform.rotation, Time.fixedDeltaTime * m_lerpSpeed);
        // Source : https://answers.unity.com/questions/1936597/how-do-you-rotate-an-object-relative-to-another-wi.html
        transform.SetPositionAndRotation(m_playerBlob.transform.position + m_distanceToPlayer, m_AssetRotation * Quaternion.Euler(m_distanceToPlayer));
    }
}