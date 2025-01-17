﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour {

    public string Name = "Juichi Lee";

    private float filteredForwardInput = 0f;
    private float filteredRightInput = 0f;

    public bool InputMapToCircular = true;

    public float forwardInputFilter = 10f;
    public float RightInputFilter = 10f;

    private float forwardSpeedLimit = 1f;
    private float rightSpeedLimit = 1f;

    private PlayerStatus status;

    public float Forward
    {
        get;
        private set;
    }

    public float Right
    {
        get;
        private set;
    }

    public bool Action
    {
        get;
        private set;
    }

    public bool Ability1
    {
        get;
        private set;
    }
    public bool Ability2
    {
        get;
        private set;
    }
    public bool Ability3
    {
        get;
        private set;
    }

    public bool Jump
    {
        get;
        private set;
    }

    public bool AimDown
    {
        get;
        private set;
    }

    public bool Attack
    {
        get;
        private set;
    }

    public bool HoldAttack
    {
        get;
        private set;
    }

    public bool Interact
    {
        get;
        private set;
    }

    public bool Drop
    {
        get;
        private set;
    }

    public bool Reload
    {
        get;
        private set;
    }


	void Update () {
        status = GetComponent<PlayerStatus>();

        //GetAxisRaw() so we can do filtering here instead of the InputManager
        float h = Input.GetAxisRaw("Horizontal");// setup h variable as our horizontal input axis
        float v = Input.GetAxisRaw("Vertical"); // setup v variables as our vertical input axis

        if (InputMapToCircular)
        {
            // make coordinates circular
            //based on http://mathproofs.blogspot.com/2005/07/mapping-square-to-circle.html
            h = h * Mathf.Sqrt(1f - 0.5f * v * v);
            v = v * Mathf.Sqrt(1f - 0.5f * h * h);

        }


        //BEGIN ANALOG ON KEYBOARD DEMO CODE
        // if (Input.GetKey(KeyCode.Q))
        //     h = -0.5f;
        // else if (Input.GetKey(KeyCode.E))
        //     h = 0.5f;

        // if (Input.GetKeyUp(KeyCode.Alpha1))
        //     forwardSpeedLimit = 0.1f;
        // else if (Input.GetKeyUp(KeyCode.Alpha2))
        //     forwardSpeedLimit = 0.2f;
        // else if (Input.GetKeyUp(KeyCode.Alpha3))
        //     forwardSpeedLimit = 0.3f;
        // else if (Input.GetKeyUp(KeyCode.Alpha4))
        //     forwardSpeedLimit = 0.4f;
        // else if (Input.GetKeyUp(KeyCode.Alpha5))
        //     forwardSpeedLimit = 0.5f;
        // else if (Input.GetKeyUp(KeyCode.Alpha6))
        //     forwardSpeedLimit = 0.6f;
        // else if (Input.GetKeyUp(KeyCode.Alpha7))
        //     forwardSpeedLimit = 0.7f;
        // else if (Input.GetKeyUp(KeyCode.Alpha8))
        //     forwardSpeedLimit = 0.8f;
        // else if (Input.GetKeyUp(KeyCode.Alpha9))
        //     forwardSpeedLimit = 0.9f;
        // else if (Input.GetKeyUp(KeyCode.Alpha0))
        //     forwardSpeedLimit = 1.0f;
        //END ANALOG ON KEYBOARD DEMO CODE  

        if (!PauseMenu.GetIsPaused())
        {
            //do some filtering of our input as well as clamp to a speed limit
            filteredForwardInput = Mathf.Clamp(Mathf.Lerp(filteredForwardInput, v,
                Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);

            filteredRightInput = Mathf.Clamp(Mathf.Lerp(filteredRightInput, h,
                Time.deltaTime * RightInputFilter), -rightSpeedLimit, rightSpeedLimit);

            Forward = filteredForwardInput;
            Right = filteredRightInput;

            //Capture "fire" button for action event
            Action = Input.GetButtonDown("Fire1");

            Ability1 = Input.GetButtonDown("Ability1") && status.sword;
            Ability2 = Input.GetButtonDown("Ability2") && status.slam;
            Ability3 = Input.GetButtonDown("Ability3") && status.scream;

            Jump = Input.GetButton("Jump");

            // Aimdown sights
            AimDown = Input.GetMouseButton(1);

            // Melee
            Attack = Input.GetMouseButtonDown(0);

            HoldAttack = Input.GetMouseButton(0);

            Interact = Input.GetKeyDown(KeyCode.E);

            Drop = Input.GetKeyDown(KeyCode.X);

            Reload = Input.GetKeyDown(KeyCode.R);
        }
        
	}
}
