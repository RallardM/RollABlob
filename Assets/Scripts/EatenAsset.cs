// Source : https://answers.unity.com/questions/1574465/error-cannot-convert-quaternion-to-vector3.html

using UnityEngine;

public class EatenAsset : MonoBehaviour
{
    GameObject m_playerNode;
    private bool m_isBeingEaten = false;
    private bool m_isEaten = false;

    public bool IsBeingEaten { get => m_isBeingEaten; set => m_isBeingEaten = value; }

    private void Awake()
    {
        m_playerNode = GameObject.FindGameObjectWithTag("PlayerBlob");
    }

    void FixedUpdate()
    {
        if (m_isBeingEaten)
        {
            //transform.position = Vector3.Lerp(transform.position, m_playerNode.transform.position, Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, m_playerNode.transform.position) < 0.4f)
            {
                m_isBeingEaten = false;
                m_isEaten = true;
            }
            else 
            {
                transform.position = Vector3.Lerp(transform.position, m_playerNode.transform.position, Time.fixedDeltaTime * 2f);

                // Source : https://answers.unity.com/questions/1574465/error-cannot-convert-quaternion-to-vector3.html
                transform.rotation = Quaternion.Lerp(transform.rotation, m_playerNode.transform.rotation, Time.fixedDeltaTime * 2f);
            }
        }

        if (m_isEaten)
        {
            transform.position = m_playerNode.transform.position;
            transform.rotation = m_playerNode.transform.rotation;
        }
    }
}
