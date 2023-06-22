using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobStomac : MonoBehaviour
{
    private Transform m_playerTransform;
    private GameObject m_fullBodyTrigger;
    private BlobAbsorb m_blobAbsorb;

    private void Awake()
    {
        m_playerTransform = transform.parent.transform;
        m_fullBodyTrigger = m_playerTransform.transform.Find("FullBodyTrigger").gameObject;
        m_blobAbsorb = m_fullBodyTrigger.GetComponentInChildren<BlobAbsorb>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EatenAsset>().IsBeingEaten == false)
        {
            m_blobAbsorb.PrepareNPC(other);
        }

        // If is not digested,
        // set it to is digested
        // this will force its position syncing
        // to the player's position in EatenAsset.cs
        Debug.Log(other.name + " Is digested");
        other.gameObject.GetComponent<EatenAsset>().IsDigested = true;
    }
}
