using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeCamera : MonoBehaviour
{

    float ShakeForce;
    float StartShakeTime;
    float ShakeTime;
    bool Smooth = false;

    private player PlayerController;
    private GameObject CameraObject;
    private CinemachineBrain Brain;

    void Start()
    {
        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        CameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        Brain = CameraObject.GetComponent<CinemachineBrain>();

        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }


    void OnCameraUpdated(CinemachineBrain brain)
    {
        if (ShakeTime > 0)
        {
            if (Smooth)
                brain.OutputCamera.transform.position += ShakeTime / StartShakeTime * (Random.insideUnitSphere * ShakeForce);
            else
                brain.OutputCamera.transform.position += Random.insideUnitSphere * ShakeForce;
            ShakeTime -= Time.deltaTime;
        }
    }
    
    public void ShakingCamera(float ShakeForce, float ShakeTime, bool Smooth)
    {
        this.ShakeForce = ShakeForce;
        this.StartShakeTime = this.ShakeTime = ShakeTime;
        this.Smooth = Smooth;
    }

    public void AddShakingCamera(float ShakeForce, float ShakeTime)
    {
        this.ShakeForce += ShakeForce;
        this.ShakeTime += ShakeTime;
    }

}
