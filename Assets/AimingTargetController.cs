using UnityEngine;

public class AimingTargetController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float distanceFromCamera = 5f; 

    private void OnEnable() {
        if(mainCamera == null){
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector3 forwardPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

        transform.position = forwardPosition;
    }
}
