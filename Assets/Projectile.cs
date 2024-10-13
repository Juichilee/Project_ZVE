using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody projRigidbody;
    const float speed = 10f;
    const int despawnTime = 10; // in seconds
    public GameObject shooter;

    public void Shooter(GameObject setShooter)
    {
        shooter = setShooter;
    }
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

    void OnTriggerEnter(Collider other)
    {
        // Ignore colliding with projectile layer and the shooter's layer
        if(other.gameObject.layer != gameObject.layer && other.gameObject.layer != shooter.layer){
            Destroy(gameObject);
        }
    }
}
