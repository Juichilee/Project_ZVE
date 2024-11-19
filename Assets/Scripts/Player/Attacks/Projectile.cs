using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody projRigidbody;
    [SerializeField] private float speed = 10f; // Should be set to const or scriptable in the future to preserve memory
    [SerializeField] private int despawnTime = 10; // in seconds
    [SerializeField] private LayerMask triggerLayer;
    // Start is called before the first frame update
    void Start()
    {
        projRigidbody = GetComponent<Rigidbody>();
        projRigidbody.AddForce(this.transform.forward * speed, ForceMode.VelocityChange);
        StartCoroutine(DespawnCountdown());
    }

    IEnumerator DespawnCountdown()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        int objLayerMask = 1 << collision.gameObject.layer;
        // Perform bitwise AND between the LayerMask and the object's layer mask
        bool destroyProj = (triggerLayer.value & objLayerMask) != 0;

        if (destroyProj)
        {
            Destroy(gameObject);
        }
    }
}
