using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationExchange : MonoBehaviour
{
    [SerializeField] bool inTrigger = false;
    [SerializeField] GameObject interactGuide;


    void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (interactGuide == null)
        {
            interactGuide = GameObject.Find("InteractPanel");
        }
        ShopMenu shopMenu = GameObject.Find("UICanvas").GetComponent<ShopMenu>();
        if (interactGuide == null)
        {
            Debug.LogError("MutationExchange needs an interact panel UI");
        }
        else interactGuide.SetActive(inTrigger && !(shopMenu.shopping || shopMenu.exShopping));
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

    public bool getInTrigger()
    {
        return inTrigger;
    }
}
