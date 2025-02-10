using UnityEngine;

public class WorldMapPortal : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    private bool _isPlayerInContact;
    
    private void Start(){
        inputReader = InputReaderManager.Instance.InputReader;
    }

    private void OnEnable()
    {
        inputReader = InputReaderManager.Instance.InputReader;
        inputReader.OpenDialogeEvent += OpenWorldMap;
    }


    private void OnDisable()
    {
        inputReader.OpenDialogeEvent -= OpenWorldMap;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isPlayerInContact = true;    
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if(!_isPlayerInContact) return;
        _isPlayerInContact = false;
    }
    
    private void OpenWorldMap(){
        if (!_isPlayerInContact) return;
        if (WorldMapUIManager.Instance.worldMapGO.activeSelf) return;

        WorldMapUIManager.Instance.worldMapGO.SetActive(true);
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter();
    }

    public void CloseWorldMap(){
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
    }
}
