using UnityEngine;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;

// [RequireComponent(typeof(CharacterInputController))]
public class ThirdPersonCamera : MonoBehaviour
{
    public float maxYAxisSpeed = 2f;
    public float maxXAxisSpeed = 300f;
	public float sensitivity = 1.0f;
    public float aimSensitivity = 0.8f;
	public GameObject thirdPersonCam;
    private CinemachineFreeLook thirdPersonFreeLook;
    public GameObject combatCam;
    private CinemachineFreeLook combatFreeLook;
    public CinemachineFreeLook currFreeLook;
    public CameraStyle currCamStyle;
    public CameraStyle prevCamStyle;
    public CharacterInputController cinput;
    public bool _inputAimDown;
    public enum CameraStyle
    {
        Basic,
        Combat
    }

    // Store the axis values from the current camera
    float currentXAxisValue;
    float currentYAxisValue;

    void Awake()
    {
        thirdPersonCam.SetActive(true);
        combatCam.SetActive(false);
        thirdPersonFreeLook = thirdPersonCam.GetComponent<CinemachineFreeLook>();
        combatFreeLook = combatCam.GetComponent<CinemachineFreeLook>();
        cinput = GetComponent<CharacterInputController>();
    }

    void Start()
    {
        SetSensitivity();
    }

    public void SetSensitivity()
    {
        thirdPersonFreeLook.m_YAxis.m_MaxSpeed = maxYAxisSpeed * sensitivity;
        thirdPersonFreeLook.m_XAxis.m_MaxSpeed = maxXAxisSpeed * sensitivity;

        combatFreeLook.m_YAxis.m_MaxSpeed = maxYAxisSpeed * aimSensitivity;
        combatFreeLook.m_XAxis.m_MaxSpeed = maxXAxisSpeed * aimSensitivity;
    }
    
    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        currCamStyle = newStyle;
        // Activate the new camera and deactivate the current camera
        switch (newStyle)
        {
            case CameraStyle.Basic:
                thirdPersonCam.SetActive(true);
                combatCam.SetActive(false);
                currFreeLook = thirdPersonFreeLook;
                break;
            case CameraStyle.Combat:
                combatCam.SetActive(true);
                thirdPersonCam.SetActive(false);
                currFreeLook = combatFreeLook;
                break;
            default:
                thirdPersonCam.SetActive(true);
                combatCam.SetActive(false);
                currFreeLook = thirdPersonFreeLook;
                break;
        }

        // Store the axis values from the current camera before switching
        if (currCamStyle != prevCamStyle)
        {
            Debug.Log("Assigning old Axis Values:");
            // Assign the stored axis values to the new camera after it's been activated
            currFreeLook.m_XAxis.Value = currentXAxisValue;
            currFreeLook.m_YAxis.Value = currentYAxisValue;
        }

        currentXAxisValue = currFreeLook.m_XAxis.Value;
        currentYAxisValue = currFreeLook.m_YAxis.Value;

        prevCamStyle = currCamStyle;
        // Debug.Log("Curr X Axis Val: " + currFreeLook.m_XAxis.Value);
        // Debug.Log("Curr Y Axis Val: " + currFreeLook.m_YAxis.Value);
        // Ensure the new camera recognizes the previous state as valid
        // currFreeLook.PreviousStateIsValid = true;
    }

    void Update()
    {

        if (cinput.enabled)
        {
            _inputAimDown = cinput.AimDown;
        }

        // Check if the right mouse button is pressed down
        if (_inputAimDown)
        {
            SwitchCameraStyle(CameraStyle.Combat);
        }
        // Check if the right mouse button is released
        else
        {
            SwitchCameraStyle(CameraStyle.Basic);
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
