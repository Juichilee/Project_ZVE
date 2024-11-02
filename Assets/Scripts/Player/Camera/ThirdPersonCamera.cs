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
	public GameObject basicCam;
    private CinemachineFreeLook basicFreeLook;
    public GameObject rangedCam;
    private CinemachineFreeLook rangedFreeLook;
    public CinemachineFreeLook currFreeLook;
    public CameraStyle currCamStyle;
    public CharacterInputController cinput;
    public WeaponHandler weaponHandler;
    public bool _inputAimDown;
    public enum CameraStyle
    {
        Basic,
        Ranged
    }

    void Awake()
    {
        basicCam.SetActive(true);
        rangedCam.SetActive(false);
        basicFreeLook = basicCam.GetComponent<CinemachineFreeLook>();
        rangedFreeLook = rangedCam.GetComponent<CinemachineFreeLook>();
        cinput = GetComponent<CharacterInputController>();
    }

    void Start()
    {
        SetSensitivity();
    }

    public void SetSensitivity()
    {
        basicFreeLook.m_YAxis.m_MaxSpeed = maxYAxisSpeed * sensitivity;
        basicFreeLook.m_XAxis.m_MaxSpeed = maxXAxisSpeed * sensitivity;

        rangedFreeLook.m_YAxis.m_MaxSpeed = maxYAxisSpeed * aimSensitivity;
        rangedFreeLook.m_XAxis.m_MaxSpeed = maxXAxisSpeed * aimSensitivity;
    }
    
    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        currCamStyle = newStyle;
        // Activate the new camera and deactivate the current camera
        switch (newStyle)
        {
            case CameraStyle.Basic:
                basicCam.SetActive(true);
                rangedCam.SetActive(false);
                currFreeLook = basicFreeLook;
                break;
            case CameraStyle.Ranged:
                rangedCam.SetActive(true);
                basicCam.SetActive(false);
                currFreeLook = rangedFreeLook;
                break;
        }
    }

    public void SetRangedCameraFOV(int fov)
    {
        rangedFreeLook.m_Lens.FieldOfView = fov;
    }
}
