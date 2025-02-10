using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class ThirdPersonCamera : MonoBehaviour, ICameraView
{
    [SerializeField] private InputReader inputReader;
    public bool isActive;
    private void OnEnable()
    {
        // inputReader.MouseLockEvent += HandleMouseLock;

    }
    private void OnDisable()
    {
        // inputReader.MouseLockEvent -= HandleMouseLock;
    }

    void Update()
    {
        // Kiểm tra xem chuột phải có được nhấn hay không
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleMouseLock(Vector2 vector)
    {
        if (Input.GetMouseButton(1))
        {
            Debug.Log("Mouse Lock");
            var cameraFreeLook = gameObject.GetComponent<CinemachineFreeLook>();
            cameraFreeLook.m_XAxis.Value += vector.x * Time.deltaTime * 100f;
            cameraFreeLook.m_YAxis.Value += vector.y * Time.deltaTime * 0.1f;
        }
    }

    public void ActivateView()
    {
        isActive = true;
        gameObject.SetActive(true);
    }

    public void DeactiveView()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}