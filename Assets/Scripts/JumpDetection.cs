using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    //private bool m_isGrounded;
    private GameObject m_playerBlob;

    private void Awake()
    {
        m_playerBlob = GameObject.FindGameObjectWithTag("PlayerBlob");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided mesh" + other.name);
       // m_isGrounded = true;
        //if (other.gameObject.layer == JUMPABLE)
        //{
        //    //Debug.Log("Floor is touched");
        //    m_isGrounded = true;
        //}

        //if (m_jellyMesh.m_squashing != m_initialSquashing)
        //{
        //    m_jellyMesh.m_squashing = m_initialSquashing;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.layer == JUMPABLE)
        //{
        //    //Debug.Log("Left the floor");
        //    m_isGrounded = false;
        //}
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

 
     
        //if (m_isGrounded)
        //{
        //    Debug.Log("Is grounded");
        //}
        //else
        //{
        //    Debug.Log("Is not grounded");
        //}
    }
}
