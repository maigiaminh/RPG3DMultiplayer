using System;
using UnityEngine;

public class DungeonDescriptItem : MonoBehaviour
{
    public DungeonDescriptScriptableObject dungeonDescript;
    [SerializeField] InputReader _inputReader;
    private bool _isPlayerInContact;

    private void Start(){
        _inputReader = InputReaderManager.Instance.InputReader;
    }
    private void OnEnable()
    {
        _inputReader = InputReaderManager.Instance.InputReader;
        _inputReader.OpenDungeonDescriptPanelEvent += OpenDungeonDescriptPanel;
    }


    private void OnDisable()
    {
        _inputReader.OpenDungeonDescriptPanelEvent -= OpenDungeonDescriptPanel;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isPlayerInContact = true;
        DungeonDescriptManager.Instance.DungeonContactName.PlayDungeonContactName(dungeonDescript.DungeonPortrait, dungeonDescript.DungeonName);
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        DungeonDescriptManager.Instance.DungeonContactName.CloseDungeonContactName();
        if(!_isPlayerInContact) return;
        _isPlayerInContact = false;
        GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseDungeonDescriptPanel(this);
    }

    private void OpenDungeonDescriptPanel()
    {
        if (!_isPlayerInContact) return;
        GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenDungeonDescriptPanel(this);
    }


}
