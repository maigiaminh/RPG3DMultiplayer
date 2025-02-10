using UnityEngine;

public class FirstPersonCamera : MonoBehaviour, ICameraView
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float mouseSpeed = 100;

    public bool isActive;
    private float x;
    private float y;


    private void OnEnable()
    {
        // inputReader.MouseLockEvent += HandleMouseLock;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // inputReader.MouseLockEvent -= HandleMouseLock;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {


    }

    private void FixedUpdate()
    {
        if (!isActive) return;
        playerTransform.Rotate(Vector3.up * x);
        transform.localRotation *= Quaternion.Euler(-y, 0, 0);
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




    private void HandleMouseLock(Vector2 vector)
    {
        x = vector.x * mouseSpeed * Time.deltaTime;
        y = Mathf.Clamp(vector.y * mouseSpeed * Time.deltaTime, -30, 60);
    }


}
