using System.Collections;
using UnityEngine;

public class MapViewportManager : Singleton<MapViewportManager>
{
    public InputReader inputReader; // Input Reader
    public GameObject map; // Map Panel
    public Camera mapCamera; // Camera hiển thị Render Texture
    public GameObject player; // Vị trí của người chơi
    public float zoomSpeed = 2f; // Tốc độ zoom
    public float minZoom = 15f; // Zoom tối thiểu
    public float dragSpeed = 100f; // Tốc độ kéo map
    public float maxZoom = 45f; // Zoom tối đa
    private Vector3 dragOrigin;
    private bool openMap = false;
    private Animator animator;

    private void Start(){
        if(!map){
            Debug.LogError("MapViewportController - Can not find map panel");
        }
        else{
            animator = map.GetComponent<Animator>();
        }

        if(!mapCamera){
            Debug.LogError("MapViewportController - Can not find map camera");
        }

        if(!player){
            Debug.LogError("MapViewportController - Can not find player");
        }

        if(!inputReader){
            Debug.LogError("MapViewportController - Can not find input reader");
        }

        
    }

    private void OnEnable(){
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenMap += OpenMap;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseMap += CloseMap;
    }

    private void OnDisable(){
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenMap -= OpenMap;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseMap -= CloseMap;

    }
    void Update()
    {   
        if (openMap)
        {
            HandleDrag();
            HandleZoom();
        }
    }

    private void OpenMap(){
        if(!map || !player || !mapCamera) return;

        map.SetActive(true);
        SetAnimation("Appear");
        openMap = true;
        mapCamera.orthographicSize = 30f;
        Vector3 playerPosition = player.transform.position;
        mapCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y + 30, playerPosition.z);

    }

    private void CloseMap(){
        if(!map || !player || !mapCamera) return;
        
        SetAnimation("Disappear");
        openMap = false;
    }   

    void HandleDrag()
    {
        if(!map || !player || !mapCamera) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            CursorManager.Instance.SetDragCursor();
            Vector3 difference = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            dragOrigin = Input.mousePosition;

            mapCamera.transform.Translate(-difference.x * dragSpeed, -difference.y * dragSpeed, 0);
        }

        if (Input.GetMouseButtonUp(0)){
            CursorManager.Instance.SetDefaultCursor();
        }
    }

    void HandleZoom()
    {
        if(!openMap) return;

        float scroll = inputReader.ZoomValue.y; // Cuộn chuột để zoom
        if (scroll != 0)
        {
            mapCamera.orthographicSize -= scroll * zoomSpeed;
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize, minZoom, maxZoom);
        }
    }

    private void SetAnimation(string trigger)
    {
        if (trigger == "Appear") animator.ResetTrigger("Disappear");
        else animator.ResetTrigger("Appear");
        
        animator.SetTrigger(trigger);
    }

    public void DisableMap(){
        map.SetActive(false);
    }
}
