using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiActivator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IBrain>() && other.GetComponent<IBrain>().enabled == false)
        {
            other.GetComponent<IBrain>().enabled = true;
            other.GetComponent<Animator>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IBrain>() && other.GetComponent<IBrain>().enabled == true)
        {
            other.GetComponent<IBrain>().enabled = false;
            //other.GetComponent<Animator>().enabled = false;
        }
    }
}
