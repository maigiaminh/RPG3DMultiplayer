using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Destination : MonoBehaviour
{
    public Vector3 Position;
    public GameObject MarkerPrefab;
    [HideInInspector] public string questId;
    [HideInInspector] public int destinationId;
    public static event Action<string, int> onDestinationReached;
    private void OnEnable()
    {
        Position = transform.position;
    }
    private void OnDisable() {
        transform.position = Position;
    }

    private void Start() {
        transform.position = Position;
    }
    public void DestinationReached(string questId, int id)
    {
        onDestinationReached?.Invoke(questId, id);

    }

    public void SetDestination(string qId, int destinationId)
    {
        Debug.Log("QuestId: " + qId);
        Debug.Log("DestinationId: " + destinationId);
        this.questId = qId;
        this.destinationId = destinationId;
        Debug.Log("Assign DestinationId: " + this.questId);

        RaycastHit hit;
        Vector3 potentialPoint = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000, LayerMask.GetMask("Default")))
        {
            potentialPoint = hit.point;
        }
        transform.position = potentialPoint;
        MarkerPrefab.SetActive(true);
        MarkerPrefab.transform.localPosition = Vector3.zero;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DestinationReached(questId, destinationId);
            gameObject.SetActive(false);
            if (MarkerPrefab != null)
            {
                Destroy(MarkerPrefab);
            }
        }
    }
}
