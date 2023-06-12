// Source : https://docs.unity3d.com/ScriptReference/Transform.SetParent.html
// Source : https://forum.unity.com/threads/rotation-is-not-being-inherited-from-parent.822780/
// Source : https://answers.unity.com/questions/37286/how-turn-off-the-mesh-collider.html

using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AI;

public class BlobAbsorb : MonoBehaviour
{
    private Transform m_playerNode;

    private void Awake()
    {
        m_playerNode = transform.parent.GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Object entered : " + other.name);
        if (other.gameObject.tag == "NPC")
        {
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            other.gameObject.GetComponent<IBrain>().enabled = false;
            other.gameObject.GetComponent<EatenAsset>().IsBeingEaten = true;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
            
        if (other.gameObject.tag == "Movable")
        {
            //Vector3 objectScale = other.transform.localScale;
            //RemoveCollider(other.gameObject);
            //DetachFromParent(other.gameObject);
            //SetPlayerNodeParent(other.gameObject);
            //other.transform.localScale = objectScale;
        }
    }

    // Source : https://answers.unity.com/questions/37286/how-turn-off-the-mesh-collider.html
    private void RemoveCollider(GameObject child)
    {
        //Removes the collider component from the GameObject "child".
        Destroy(child.GetComponent<Collider>());
    }

    // Source : https://docs.unity3d.com/ScriptReference/Transform.SetParent.html
    private void DetachFromParent(GameObject child)
    {
        // Detaches the transform from its parent.
        child.transform.SetParent(null);
    }
    private void SetPlayerNodeParent(GameObject child)
    {
        //Vector3 objectScale = child.transform.localScale;
        //Debug.Log("Object scale before : " + objectScale);
        //Makes the GameObject "player" the parent of the GameObject "child".
        child.transform.SetParent(m_playerNode);
        //child.transform.localScale = objectScale;
        //Debug.Log("Object scale after : " + child.transform.localScale);
    }
}
