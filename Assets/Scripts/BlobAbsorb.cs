// Source : https://answers.unity.com/questions/37286/how-turn-off-the-mesh-collider.html
// Source : https://forum.unity.com/threads/cuberoot.33513/
// Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
// Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
// Source : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/

using UnityEngine;
using UnityEngine.AI;

public class BlobAbsorb : MonoBehaviour
{
    private Transform m_playerTransform;
    private Vector3 m_playerInitialScale = Vector3.zero;
    private float m_assetMassToAdd = 0.0f;
    private const float m_massMultiplier = 10000000;
    private const float m_lerpSpeed = 0.125f; // Divide by 2 or multiply by 0.5, higher divider or smaller multiplier, faster lerp
    private bool m_eatableObjects = false;

    private void Awake()
    {
        // Source : https://forum.unity.com/threads/getting-the-position-of-a-parent-gameobject.1138150/
        m_playerTransform = transform.parent.transform;
        m_playerInitialScale = GetPlayerLocalScale();

        // If the possibility to eat objects is activated, we need to enable the player's full body trigger
        // so that the player can interract with objects without passthrough them.
        if (!m_eatableObjects)
        {
            GameObject blobFullBodyTrigger = m_playerTransform.transform.Find("FullBodyCollider").gameObject;
            blobFullBodyTrigger.GetComponent<SphereCollider>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.GetComponent<EatenAsset>() == null)
        {
            return;
        }

        bool isBeingEaten = other.gameObject.GetComponent<EatenAsset>().IsBeingEaten;
        bool isEaten = other.gameObject.GetComponent<EatenAsset>().IsEaten;
        bool isDigested = other.gameObject.GetComponent<EatenAsset>().IsDigested;
        bool isUntouched = (isBeingEaten == false && isEaten == false && isDigested == false);

        // If the asset in contact with the player is a NPC,
        // we need to disable its the attributes that make it interact physically with the world.
        if (other.gameObject.tag == "NPC" && isUntouched)
        {
            PrepareNPC(other);
            return;
        }

        // If the asset in contact with the player is a movable object,
        // we need to disable its the attributes that make it interact physically with the world.
        if (other.gameObject.tag == "Movable" && isUntouched)
        {
            // Activate/deactivate the possibility to eat objects for the whole game
            if (!m_eatableObjects)
            {
                return;
            }

            DeactivateObjectPhysic(other);
            CollectAssetMassToAddToPlayer(other);
            ChangeLayerTag(other);
            SetIsBeingEaten(other);
            return;
        }
    }

    void FixedUpdate()
    {
        // If there is mass to add to the player
        if (m_assetMassToAdd > 0.0f)
        {
            // Resize the player scale to the lerping new scale
            // Takes the mass from the eaten asset that was collected into m_assetMassToAdd
            // and adds it to the player's scale
            // Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
            m_playerTransform.localScale = GetAssetToPlayerAdditiveResize();

            // Then substract the mass that was added to the player from the asset's mass collected in m_assetMassToAdd
            // and return the mass that was substracted to see how much mass is left to add to the player
            float massAddedToSubstract = SubstractNewlyAddedMass();
        }
    }

    public float GetPLayerSizeDifference()
    {
        float playerSizeDifference = 0.0f;

        // Calculate the offset based on the difference between the player's initial and new size
        playerSizeDifference = m_playerTransform.localScale.y / m_playerInitialScale.y;

        if (playerSizeDifference == 1.0f)
        {
            return 0.0f;
        }

        return playerSizeDifference;
    }

    public void PrepareNPC(Collider other)
    {
        DeactivateNPCPhysic(other);
        CollectAssetMassToAddToPlayer(other);
        ChangeLayerTag(other);
        SetIsBeingEaten(other);
    }

    private Vector3 GetPlayerLocalScale()
    {
        return m_playerTransform.localScale;
    }

    private void DeactivateNPCPhysic(Collider other)
    {
        // Early return, if the asset has no NavMeshAgent, IBrain or Rigidbody component to avoid errors
        if (other.gameObject.GetComponent<NavMeshAgent>() == null)
        {
            return;
        }

        if (other.gameObject.GetComponent<IBrain>() == null)
        {
            return;
        }

        if (other.gameObject.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        // Source: https://answers.unity.com/questions/37286/how-turn-off-the-mesh-collider.html
        other.gameObject.GetComponent<NavMeshAgent>().enabled = false;

        other.gameObject.GetComponent<IBrain>().enabled = false;

        other.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    private void DeactivateObjectPhysic(Collider other)
    {
        // Early return, if the asset has no Rigidbody component to avoid errors
        if (other.gameObject.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        other.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    private void CollectAssetMassToAddToPlayer(Collider other)
    {
        // Take the mass from that asset and add it to the mass to add to the player
        m_assetMassToAdd += (other.gameObject.GetComponent<Rigidbody>().mass * m_massMultiplier);
    }

    private void ChangeLayerTag(Collider other)
    {
        // Change the layer of the asset to EatenAssets so it doesn't interact with the world anymore
        other.gameObject.layer = LayerMask.NameToLayer("EatenAssets");
        other.gameObject.tag = "Eaten";
    }

    private void SetIsBeingEaten(Collider other)
    {
        // Early return, if the asset has an EatenAsset component to avoid errors
        if (other.gameObject.GetComponent<EatenAsset>() == null)
        {
            return;
        }

        other.gameObject.GetComponent<EatenAsset>().IsBeingEaten = true;
    }

    private float SubstractNewlyAddedMass()
    {
        // Get the difference between the new player size and the initial player size
        Vector3 playerSizeVolumeDifference = GetPlayerLocalScale() - m_playerInitialScale;

        // Since the player is the same size in both directions, we only need to take the X value of the difference
        float adjustOverSubstraction = VerifyOverSubstraction(playerSizeVolumeDifference.x);

        // Source : https://forum.unity.com/threads/cuberoot.33513/
        // Cube that difference to get the volume to substract from the gathered mass from the eaten asset.
        float massAddedToSubstract = Mathf.Pow(adjustOverSubstraction, 3f);

        // Substract that difference to the mass to add
        m_assetMassToAdd -= massAddedToSubstract;

        // Readjust m_assetMassToAdd to 0.0f if the substaction goes under zero to avoid negative values
        m_assetMassToAdd = VerifyOverSubstraction(m_assetMassToAdd);

        return massAddedToSubstract;
    }

    private float VerifyOverSubstraction(float valueToCheck)
    {
        // If the value to check is under zero, return zero
        // so the result does not goes under zero
        if (valueToCheck < 0)
        {
            return valueToCheck = 0;
        }

        return valueToCheck;
    }

    private float GetAddedMassCubeRoot()
    {
        // Source: https://forum.unity.com/threads/cuberoot.33513/
        // Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
        // Get the cube root of the added mass to get one of the lenght of the mass as a cube and add this length to the player's cubic lenghts
        return Mathf.Pow(m_assetMassToAdd, 1f / 3f);
    }

    private Vector3 GetAssetToPlayerAdditiveResize()
    {
        // Source : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        // Lerps the current player's scale to the new scale resulting from the added mass from the eaten asset.
        float xLerp = Mathf.Lerp(GetPlayerLocalScale().x, GetPlayerLocalScale().x + GetAddedMassCubeRoot(), Time.fixedDeltaTime * m_lerpSpeed);
        float yLerp = Mathf.Lerp(GetPlayerLocalScale().y, GetPlayerLocalScale().y + GetAddedMassCubeRoot(), Time.fixedDeltaTime * m_lerpSpeed);
        float zLerp = Mathf.Lerp(GetPlayerLocalScale().z, GetPlayerLocalScale().z + GetAddedMassCubeRoot(), Time.fixedDeltaTime * m_lerpSpeed);

        // Resize the player scale to the lerping new scale
        // Source : https://docs.unity3d.com/ScriptReference/Transform-localScale.html
        return new Vector3(xLerp, yLerp, zLerp);
    }
}
