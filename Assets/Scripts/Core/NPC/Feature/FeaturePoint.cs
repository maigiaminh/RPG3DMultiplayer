using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FeaturePoint : MonoBehaviour
{
    public FeatureItem featureItem;
    public NpcFeatureType featureType;
    public DialogeInfo dialogeItem;
    private bool _playerInRange = false;
    private AudioSource _audioSource;
    [SerializeField] InputReader _inputReader;

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _inputReader = InputReaderManager.Instance.InputReader;
    }

    protected virtual void OnEnable()
    {
        _inputReader = InputReaderManager.Instance.InputReader;
        
        _inputReader.OpenDialogeEvent += HandlePlayerOpenDialoge;
    }

    protected virtual void OnDisable()
    {
        _inputReader.OpenDialogeEvent -= HandlePlayerOpenDialoge;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = true;
        DialogeUIManager.Instance.ActivateContactPressEBoard(dialogeItem.actorName, dialogeItem.actorIcon, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
        DialogeUIManager.Instance.ActivateContactPressEBoard(dialogeItem.actorName, dialogeItem.actorIcon, false);
        if (!DialogeUIManager.Instance.IsDialogOpen) return;
        DialogeUIManager.Instance.CloseDialoge(featureItem);
    }


    protected virtual void HandlePlayerOpenDialoge()
    {
        if (!_playerInRange) return;
        GameEventManager.Instance.DialogeEvents.DialogeFeatureStart(featureType, dialogeItem, featureItem, _audioSource);
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter(transform);
    }
}

public enum NpcFeatureType
{
    Shop
}