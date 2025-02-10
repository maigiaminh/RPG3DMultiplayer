using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private KeyCode switchKey = KeyCode.Space; 
    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;
    private List<ICameraView> cameraViews = new List<ICameraView>();
    public ICameraView cameraView;
    private int currentViewIndex = 0;


    public override void OnNetworkSpawn()
    {
        if(IsServer){
            DeactivateCameraOnAllClientsBeforeClientRpc(NetworkObjectId);
        }
        if (IsOwner){
            cameraViews = GetComponentsInChildren<ICameraView>().ToList();

            DeactiveAllCameraAppear();

            SetCameraPriority();

            SwitchCamera();

        }
    }


    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(switchKey))
        {
            SwitchCamera();
        }
    }


    private void SwitchCamera()
    {
        // deactive current camera view
        cameraView?.DeactiveView();

        // get index of next camera view and activate it
        var nextIndex = (currentViewIndex++) % cameraViews.Count;
        cameraView = cameraViews[nextIndex];
        cameraView.ActivateView();
    }

    private void SetCameraPriority()
    {
        foreach (var view in cameraViews)
        {
            if (view is ThirdPersonCamera thirdPersonCamera)
            {
                thirdPersonCamera.gameObject.GetComponent<CinemachineFreeLook>().Priority = cameraPriority;
            }
        }
    }
    private void DeactiveAllCameraAppear()
    {
        foreach(NetworkObject player in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values){
            if(player == NetworkObject) continue;

            var views = player.GetComponentsInChildren<ICameraView>();

            foreach(var view in views){
                if(view == null) continue;

                view.DeactiveView();
            }
        }
    }


    [ClientRpc]
    private void DeactivateCameraOnAllClientsBeforeClientRpc(ulong networkObjectId)
    {
        if(IsOwner) return;

        if(!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject player)){
            Debug.Log("Player not found In CameraController");
            return;
        }
        var views = player.GetComponentsInChildren<ICameraView>();
        foreach(var view in views){
            if(view == null) continue;


            view.DeactiveView();
        }

    }
}
