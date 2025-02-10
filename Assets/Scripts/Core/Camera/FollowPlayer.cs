using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = Vector3.zero;
    private void Start() {
        player = FindAnyObjectByType<TankPlayer>().transform;
    }
    void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
