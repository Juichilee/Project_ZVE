using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallCollisionReporter : MonoBehaviour
{
    void OnCollisionEnter(Collision c)
    {

        if (c.impulse.magnitude > 0.25f)
        {
            //we'll just use the first contact point for simplicity
            EventManager.TriggerEvent<BombBounceEvent, Vector3>(c.contacts[0].point);

        }

    }
}