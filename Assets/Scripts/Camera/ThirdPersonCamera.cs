using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class ThirdPersonCamera : MonoBehaviour
{
	// public float positionSmoothTime = 1f;		// a public variable to adjust smoothing of camera motion
    // public float rotationSmoothTime = 1f;
    // public float positionMaxSpeed = 50f;        //max speed camera can move
    // public float rotationMaxSpeed = 50f;
	public GameObject thirdPersonCam;
    public GameObject combatCam;
    // public Transform combatLookAt;
    // public Transform target;
    public CameraStyle currStyle;

    // protected Vector3 currentPositionCorrectionVelocity;
    //protected Vector3 currentFacingCorrectionVelocity;
    //protected float currentFacingAngleCorrVel;
    // protected Quaternion quaternionDeriv;

    protected float angle;
    public enum CameraStyle
    {
        Basic,
        Combat
    }
    
    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        
        switch(newStyle)
        {
            case CameraStyle.Basic:
                thirdPersonCam.SetActive(true);
            break;
            case CameraStyle.Combat:
                combatCam.SetActive(true);
            break;
        }
        currStyle = newStyle;

    }

    void Update()
    {
        // Check if the right mouse button is pressed down
        if (Input.GetMouseButtonDown(1))
        {
            SwitchCameraStyle(CameraStyle.Combat);
            Debug.Log("Combat Style");
        }
        // Check if the right mouse button is released
        else if (Input.GetMouseButtonUp(1))
        {
            SwitchCameraStyle(CameraStyle.Basic);
            Debug.Log("Basic Style");
        }
    }

	// void LateUpdate ()
	// {

    //     if (basicLookAt != null && currStyle == CameraStyle.Basic)
    //     {
            // transform.position = Vector3.SmoothDamp(transform.position, basicLookAt.position, ref currentPositionCorrectionVelocity, positionSmoothTime, positionMaxSpeed, Time.deltaTime);

            // var targForward = basicLookAt.forward;
            // //var targForward = (target.position - this.transform.position).normalized;

            // transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation,
            //     Quaternion.LookRotation(targForward, Vector3.up), ref quaternionDeriv, rotationSmoothTime);


        // } else if (combatLookAt != null && currStyle == CameraStyle.Combat){
            
        // }
    // }
}
