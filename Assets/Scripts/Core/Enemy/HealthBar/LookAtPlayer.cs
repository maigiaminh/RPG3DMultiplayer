using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] 
    private Transform mainCamera;

    private void Awake() {
        mainCamera = FindObjectOfType<Camera>().transform;
    }

    private void LateUpdate() {
        transform.LookAt(mainCamera);
    }
}
