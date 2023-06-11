using UnityEngine;

public class BlobAbsorb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered : " + other.name);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
