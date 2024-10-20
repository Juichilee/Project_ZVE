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
    public float aimSensitivity = 0.5f;
	public GameObject thirdPersonCam;
    private CinemachineFreeLook thirdPersonFreeLook;
    public GameObject combatCam;
    private CinemachineFreeLook combatFreeLook;
    public CinemachineFreeLook currFreeLook;
    public CameraStyle currCamStyle;
    private CameraStyle prevCamStyle;
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
            // Assign the stored previous axis values to the new camera after it's been activated
            currFreeLook.m_XAxis.Value = currentXAxisValue;
            currFreeLook.m_YAxis.Value = currentYAxisValue;
        }

        currentXAxisValue = currFreeLook.m_XAxis.Value;
        currentYAxisValue = currFreeLook.m_YAxis.Value;

        prevCamStyle = currCamStyle;
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
}
