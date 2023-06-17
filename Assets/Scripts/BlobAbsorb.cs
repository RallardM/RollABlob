// Source : https://answers.unity.com/questions/37286/how-turn-off-the-mesh-collider.html
// Source: https://forum.unity.com/threads/cuberoot.33513/
// Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
// Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
// Source : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/

using UnityEngine;
using UnityEngine.AI;
public class BlobAbsorb : MonoBehaviour
{
    private Transform m_playerTransform;
    private MeshCollider m_playerMeshCollider;
    private SphereCollider m_playerSphereCollider;
    private Vector3 m_playerInitialScale = Vector3.zero;
    //private float m_playerInitialVolume = 0.0f;
    private float m_assetMassToAdd = 0.0f;
    private float m_npcMassMultiplier   = 10000000;
    private float m_propsMassMultiplier = 1000000;
    //private float m_playerInitialMass = 0.0f;
    private float m_lerpSpeed = 8f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp

    private void Awake()
    {
        // Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
        m_playerTransform = transform.parent.transform;
        m_playerMeshCollider = m_playerTransform.GetComponent<MeshCollider>();
        m_playerInitialScale = GetPlayerLocalScale();
        m_playerSphereCollider = m_playerTransform.GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            other.gameObject.GetComponent<IBrain>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            m_assetMassToAdd += (other.gameObject.GetComponent<Rigidbody>().mass * m_npcMassMultiplier);
            other.gameObject.layer = LayerMask.NameToLayer("EatenAssets");
            other.gameObject.tag = "Eaten";
            other.gameObject.GetComponent<EatenAsset>().IsBeingEaten = true;
        }

        if (other.gameObject.tag == "Movable")
        {
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            m_assetMassToAdd += (other.gameObject.GetComponent<Rigidbody>().mass * m_propsMassMultiplier);
            other.gameObject.layer = LayerMask.NameToLayer("EatenAssets");
            other.gameObject.tag = "Eaten";
            if (other.gameObject.GetComponent<EatenAsset>() != null)
            {
                other.gameObject.GetComponent<EatenAsset>().IsBeingEaten = true;
            }
        }
    }

    void FixedUpdate()
    {
        //Debug.Log("Object mass to add : " + m_assetMassToAdd);
        if (m_assetMassToAdd > 0.0f)
        {
            Vector3 previousPlayerSize = Vector3.zero;
            Vector3 newPlayerSize = Vector3.zero;
            Vector3 playerSizeVolumeDifference = Vector3.zero;
            float newVolumeToAddToPLayer = 0.0f;
            float massAddedToSubstract = 0.0f;

            // Source: https://forum.unity.com/threads/cuberoot.33513/
            // Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
            // Get the cube root of the added mass to get one of the lenght of the mass as a cube and add this length to the player's cubic lenghts
            newVolumeToAddToPLayer = Mathf.Pow(m_assetMassToAdd, 1f / 3f);
            //Debug.Log("New volume to add to player : " + newVolumeToAddToPLayer);
            previousPlayerSize = GetPlayerLocalScale();

            // Source : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
            float xLerp = Mathf.Lerp(previousPlayerSize.x, previousPlayerSize.x + newVolumeToAddToPLayer, Time.fixedDeltaTime / m_lerpSpeed);
            float yLerp = Mathf.Lerp(previousPlayerSize.y, previousPlayerSize.y + newVolumeToAddToPLayer, Time.fixedDeltaTime / m_lerpSpeed);
            float zLerp = Mathf.Lerp(previousPlayerSize.z, previousPlayerSize.z + newVolumeToAddToPLayer, Time.fixedDeltaTime / m_lerpSpeed);

            // Resize the player scale to the lerping new scale
            m_playerTransform.localScale = new Vector3(xLerp, yLerp, zLerp);

            // Get the difference between the new player size and the initial player size
            newPlayerSize = GetPlayerLocalScale();
            //Debug.Log("New player size : " + newPlayerSize);
            playerSizeVolumeDifference = newPlayerSize - m_playerInitialScale;
            //Debug.Log("Player size volume difference : " + playerSizeVolumeDifference);
            massAddedToSubstract = Mathf.Pow(playerSizeVolumeDifference.x, 3f);
            //Debug.Log("Mass added to substract : " + massAddedToSubstract);

            //Debug.Log("Object mass to add before substraction : " + m_assetMassToAdd);
            if (m_assetMassToAdd > 0)
            {
                // Substract that difference to the mass to add
                m_assetMassToAdd -= massAddedToSubstract;
                //Debug.Log("Object mass to add after substraction : " + m_assetMassToAdd);
                if (m_assetMassToAdd < 0)
                {
                    m_assetMassToAdd = 0;
                }   
            }

            if (massAddedToSubstract == 0.0f)
            {
                // Update the player's mesh collider to the new scale
                m_playerMeshCollider.GetComponent<MeshCollider>().sharedMesh = null;
                m_playerMeshCollider.GetComponent<MeshCollider>().sharedMesh = m_playerTransform.GetComponent<MeshFilter>().mesh;
            }
        }
    }

    public Vector3 GetPlayerNewSize()
    {
        // Source : https://docs.unity3d.com/ScriptReference/Collider-bounds.html
        float colliderHeight = m_playerMeshCollider.bounds.size.y;
        float colliderWidth = m_playerMeshCollider.bounds.size.x;
        float colliderDepth = m_playerMeshCollider.bounds.size.z;
        return new Vector3(colliderWidth, colliderHeight, colliderDepth);
    }

    public Vector3 GetPLayerSizeDifference()
    {
        Vector3 playerSizeDifference = new Vector3(0, 0, 0);

        // Calculate the offset based on the difference between the player's initial and new size
        playerSizeDifference.x = 0.0f;
        playerSizeDifference.y = m_playerTransform.localScale.y / m_playerInitialScale.y;
        playerSizeDifference.z = m_playerTransform.localScale.z / m_playerInitialScale.z;

        return playerSizeDifference;
    }

    public float GetPlayerCurrentRadius()
    {
        return m_playerSphereCollider.radius * GetPLayerSizeDifference().y;
    }

    private Vector3 GetPlayerLocalScale()
    {
        return m_playerTransform.localScale;
    }
}
