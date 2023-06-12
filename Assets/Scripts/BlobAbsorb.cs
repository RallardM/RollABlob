// Source : https://answers.unity.com/questions/37286/how-turn-off-the-mesh-collider.html
// Source: https://forum.unity.com/threads/cuberoot.33513/
// Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
// Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
// Source : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/

using System;
using UnityEngine;
using UnityEngine.AI;

public class BlobAbsorb : MonoBehaviour
{
    private Transform m_playerTransform;
    private MeshCollider m_playerMeshCollider;
    private Vector3 m_playerInitialSize = Vector3.zero;
    private float m_playerInitialVolume = 0.0f;
    private float m_assetMassToAdd = 0.0f;
    //private float m_playerInitialMass = 0.0f;
    private float m_lerpSpeed = 8f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp

    private void Awake()
    {
        float colliderHeight = 0.0f;
        float colliderWidth = 0.0f;
        float colliderDepth = 0.0f;

        // Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
        m_playerTransform = transform.parent.transform;
        m_playerMeshCollider = m_playerTransform.GetComponent<MeshCollider>();

        // Source : https://docs.unity3d.com/ScriptReference/Collider-bounds.html
        colliderHeight = m_playerMeshCollider.bounds.size.y;
        colliderWidth = m_playerMeshCollider.bounds.size.x;
        colliderDepth = m_playerMeshCollider.bounds.size.z;
        m_playerInitialVolume = colliderWidth * colliderHeight * colliderDepth;
        m_playerInitialSize = GetPlayerNewSize();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "NPC")
        {
            Debug.Log("Object entered : " + other.name);
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            other.gameObject.GetComponent<IBrain>().enabled = false;
            other.gameObject.GetComponent<EatenAsset>().IsBeingEaten = true;
            //other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            m_assetMassToAdd += other.gameObject.GetComponent<Rigidbody>().mass;
            other.gameObject.tag = "Eaten";
        }

        if (other.gameObject.tag == "Movable")
        {

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
            previousPlayerSize = GetPlayerNewSize();

            // Source : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
            float xLerp = Mathf.Lerp(previousPlayerSize.x, previousPlayerSize.x + newVolumeToAddToPLayer, Time.fixedDeltaTime / m_lerpSpeed);
            float yLerp = Mathf.Lerp(previousPlayerSize.y, previousPlayerSize.y + newVolumeToAddToPLayer, Time.fixedDeltaTime / m_lerpSpeed);
            float zLerp = Mathf.Lerp(previousPlayerSize.z, previousPlayerSize.z + newVolumeToAddToPLayer, Time.fixedDeltaTime / m_lerpSpeed);

            // Resize the player scale to the lerping new scale
            m_playerTransform.localScale = new Vector3(xLerp, yLerp, zLerp);

            // Update the player's mesh collider to the new scale
            m_playerMeshCollider.GetComponent<MeshCollider>().sharedMesh = null;
            m_playerMeshCollider.GetComponent<MeshCollider>().sharedMesh = m_playerTransform.GetComponent<MeshFilter>().mesh;

            // Get the difference between the new player size and the initial player size
            newPlayerSize = GetPlayerNewSize();
            playerSizeVolumeDifference = newPlayerSize - m_playerInitialSize;
            massAddedToSubstract = Mathf.Pow(playerSizeVolumeDifference.x, 3f);

            //Debug.Log("Object mass to add before substraction : " + m_assetMassToAdd);
            // Substract that difference to the mass to add
            m_assetMassToAdd -= massAddedToSubstract;
            //Debug.Log("Object mass to add after substraction : " + m_assetMassToAdd);
        }
    }

    private Vector3 GetPlayerNewSize()
    {
        float colliderHeight = 0.0f;
        float colliderWidth = 0.0f;
        float colliderDepth = 0.0f;
        colliderHeight = m_playerMeshCollider.bounds.size.y;
        colliderWidth = m_playerMeshCollider.bounds.size.x;
        colliderDepth = m_playerMeshCollider.bounds.size.z;
        return new Vector3(colliderWidth, colliderHeight, colliderDepth);
    }
}
