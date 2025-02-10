using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public float range = 10f;
    public Transform player { get; private set; }

    private void Start()
    {
        player = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PlayerDetector: OnTriggerEnter");
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("PlayerDetector: OnTriggerExit");
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
