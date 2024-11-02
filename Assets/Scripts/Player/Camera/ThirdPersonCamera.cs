using UnityEngine;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float maxYAxisSpeed = 2f;
    public float maxXAxisSpeed = 300f;
	public float sensitivity = 1.0f;
    public float aimSensitivity = 0.5f;
    public float aimDownFov = 15f;
    public float regularFov = 50f;
	public GameObject basicCam;
    private CinemachineFreeLook basicFreeLook;
    public GameObject rangedCam;
    private CinemachineFreeLook rangedFreeLook;
    public CameraStyle currCamStyle;
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
    }

    void Start()
    {
        SetDefaultFreeLook();
    }

    public void SetDefaultFreeLook()
    {
        basicFreeLook.m_YAxis.m_MaxSpeed = maxYAxisSpeed * sensitivity;
        basicFreeLook.m_XAxis.m_MaxSpeed = maxXAxisSpeed * sensitivity;

        rangedFreeLook.m_YAxis.m_MaxSpeed = maxYAxisSpeed * aimSensitivity;
        rangedFreeLook.m_XAxis.m_MaxSpeed = maxXAxisSpeed * aimSensitivity;

        basicFreeLook.m_Lens.FieldOfView = regularFov;
        rangedFreeLook.m_Lens.FieldOfView = regularFov;
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
                break;
            case CameraStyle.Ranged:
                rangedCam.SetActive(true);
                basicCam.SetActive(false);
                break;
        }
    }

    public void UpdateRangedCameraFOV(bool aimDown)
    {
        if (aimDown)
        {
            rangedFreeLook.m_Lens.FieldOfView = aimDownFov;
            return;
        }
        rangedFreeLook.m_Lens.FieldOfView = regularFov;
    }
}
