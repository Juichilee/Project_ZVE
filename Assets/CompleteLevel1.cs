using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteLevel1 : MonoBehaviour
{
    //private string nextSceneName = "Level1";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider onTriggerEnterCollider)
    {
        if (onTriggerEnterCollider.gameObject.tag == "playercharacter")
        {
            Debug.Log("Load the next scene");
        }
    }
}
