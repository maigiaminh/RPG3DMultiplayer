using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraController  : MonoBehaviour
{
    public List<CinemachineVirtualCameraBase> CameraBases;
    public float zoomSpeed = 100f;
    public float minFOV = 20f;
    public float maxFOV = 50f;
    private PlayerStateMachine stateMachine;

    void Start(){
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    void Update()
    {
        float scrollValue = stateMachine.InputReader.ZoomValue.y;
        if (scrollValue != 0)
        {
            foreach (CinemachineVirtualCameraBase c in CameraBases){
                if (c is CinemachineFreeLook && c.Name == "FreeLook Camera") {
                    float currentFOV = (c as CinemachineFreeLook).m_Lens.FieldOfView;
                    float targetFOV = Mathf.Clamp(currentFOV - scrollValue * zoomSpeed, minFOV, maxFOV);

                    (c as CinemachineFreeLook).m_Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * 5f);
                }
            }
        }
    }
    public void SwitchToFreeLook(){
        foreach (CinemachineVirtualCameraBase c in CameraBases){
            if (c is CinemachineFreeLook && c.Name == "FreeLook Camera") {
                c.Priority = 11;
            }
            else{
                c.Priority = 9;
            }
        }
    }

    public void SwitchToTarget(){
        foreach (CinemachineVirtualCameraBase c in CameraBases){
            if (c is CinemachineVirtualCamera && c.Name == "Targeting Camera") {
                c.Priority = 11;
            }
            else{
                c.Priority = 9;
            }
        }
    }

    public void SwitchToAiming(){
        foreach (CinemachineVirtualCameraBase c in CameraBases){
            if (c is CinemachineFreeLook && c.Name == "Aiming Camera") {
                c.Priority = 11;
            }
            else{
                c.Priority = 9;
            }
        }
    }

    public void SwitchToInteract(){
        foreach (CinemachineVirtualCameraBase c in CameraBases){
            if (c is CinemachineFreeLook && c.Name == "Interact Camera") {
                c.Priority = 11;
            }
            else{
                c.Priority = 9;
            }
        }
    }
}
