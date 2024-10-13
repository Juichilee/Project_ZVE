using UnityEngine;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterInputController))]
public class ThirdPersonCamera : MonoBehaviour
{
    public float maxYAxisSpeed = 2f;
    public float maxXAxisSpeed = 300f;
	public float sensitivity = 0.0f;
    public float aimSensitivity = 0.8f;
    public CharacterInputController characterInputController;
	public GameObject thirdPersonCam;
    private CinemachineFreeLook thirdPersonFreeLook;
    public GameObject combatCam;
    private CinemachineFreeLook combatFreeLook;
    public CameraStyle currStyle;

    protected float angle;
    public enum CameraStyle
    {
        Basic,
        Combat
    }

    void Awake()
    {
        thirdPersonCam.SetActive(true);
        combatCam.SetActive(false);
        thirdPersonFreeLook = thirdPersonCam.GetComponent<CinemachineFreeLook>();
        combatFreeLook = combatCam.GetComponent<CinemachineFreeLook>();
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
        if (characterInputController.AimDown)
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
