using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody projRigidbody;
    [SerializeField] private float speed = 10f; // Should be set to const or scriptable in the future to preserve memory
    [SerializeField] private int despawnTime = 10; // in seconds
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
}
