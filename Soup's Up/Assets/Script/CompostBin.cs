using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompostBin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Destroys Ingredient on Entry
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            Destroy(other.gameObject);

            Debug.Log(other.gameObject.name + "Trashed");
        }
    }
}
