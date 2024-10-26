using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialDevice : MonoBehaviour
{
    public GameObject[] AerialDeviceFlightPoints;
    public GameObject focusedFlightPoint;
    public GameObject DroppableObjectOne;
    public GameObject DroppableObjectTwo;

    private int flightPointNumber;
    private float dropPause = 30f;
    private Vector3 initialPlacement;
    private float timeLength;
    private float maximumFlightLength = 15f;
    private Vector3 shiningDownwardVector;
    private bool raycastContact;
    private RaycastHit lightContact;
    private bool playerSeen = false;

    public enum AerialDeviceState
    {
        MoveState,
        DropState
    };

    public AerialDeviceState aerialDeviceState;

    // Start is called before the first frame update
    void Start()
    {
        aerialDeviceState = AerialDeviceState.MoveState;
        flightPointNumber = 0;
        focusedFlightPoint = AerialDeviceFlightPoints[flightPointNumber];
        initialPlacement = this.transform.position;
        timeLength = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Confirm whether the player is underneath the aerial device
        shiningDownwardVector = this.transform.TransformDirection(Vector3.down);
        raycastContact = Physics.Raycast(this.transform.position, shiningDownwardVector, out lightContact, 21.5f);
        if (raycastContact == true)
        {
            if (lightContact.transform.gameObject.tag == "Player")
            {
                playerSeen = true;
            } else {
                playerSeen = false;
            }
        }

        if (dropPause <= 0f && playerSeen == true)
        {
            aerialDeviceState = AerialDeviceState.DropState;
        } else {
            aerialDeviceState = AerialDeviceState.MoveState;
        }

        switch (aerialDeviceState)
        {
            case AerialDeviceState.MoveState:
                // The aerial device ought to travel from flight point to flight point
                this.transform.position = Vector3.Lerp(initialPlacement, focusedFlightPoint.transform.position, timeLength / maximumFlightLength);
                if (timeLength / maximumFlightLength >= 1f)
                {
                    changeFlightPoint();
                } else {
                    timeLength = timeLength + Time.deltaTime;
                }
                break;

            case AerialDeviceState.DropState:
                if (Random.Range(0f, 1f) >= 0.5f)
                {
                    if (DroppableObjectOne != null) {
                        Instantiate(DroppableObjectOne, this.transform.position - new Vector3(0, 5, 0), this.transform.rotation);
                    }
                } else {
                    if (DroppableObjectTwo != null) {
                        Instantiate(DroppableObjectTwo, this.transform.position - new Vector3(0, 5, 0), this.transform.rotation);
                    }
                }
                break;
        }

        // Decrease the dropPause
        if (dropPause > 0f)
        {
            dropPause = dropPause - Time.deltaTime;
        } else {
            dropPause = 0f;
        }
    }

    private void changeFlightPoint()
    {
        if (AerialDeviceFlightPoints.GetLength(0) <= 0f)
        {
            Debug.Log("The array does not have any objects");
        } else {
            if (flightPointNumber != 3)
            {
                flightPointNumber = flightPointNumber + 1;
            } else {
                flightPointNumber = 0;
            }
            focusedFlightPoint = AerialDeviceFlightPoints[flightPointNumber];
            timeLength = 0f;
            initialPlacement = this.transform.position;
        }
    }
}
