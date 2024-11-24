using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationExchange : MonoBehaviour
{
    [SerializeField] bool inTrigger = false;
    [SerializeField] GameObject interactGuide;


    void Awake()
    {
        interactGuide = GameObject.Find("InteractPanel");
        if (interactGuide == null)
        {
            Debug.LogError("MutationExchange needs an interact panel UI");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = false;
        }
    }
}
