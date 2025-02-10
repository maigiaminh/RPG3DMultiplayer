using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider Collider;
    public float FieldOfView = 90f;
    public LayerMask LineOfSightLayers;

    public delegate void GainSightEvent(IDamageable player);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(IDamageable player);
    public LoseSightEvent OnLoseSight;

    private Coroutine CheckForLineOfSightCoroutine;

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable player;
        if (!other.TryGetComponent<IDamageable>(out player)) return;
        if (!other.CompareTag("Player")) return;
        if (!CheckLineOfSight(player))
        {
            CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable player;
        if (!other.TryGetComponent<IDamageable>(out player)) return;
        if (!other.CompareTag("Player")) return;
        OnLoseSight?.Invoke(player);
        if (CheckForLineOfSightCoroutine != null)
        {
            StopCoroutine(CheckForLineOfSightCoroutine);
        }
    }

    private bool CheckLineOfSight(IDamageable player)
    {

        Vector3 Direction = (player.GetTransform().position - transform.position).normalized;
        float DotProduct = Vector3.Dot(transform.forward, Direction);
        if (DotProduct >= Mathf.Cos(FieldOfView))
        {
            RaycastHit Hit;

            if (Physics.Raycast(transform.position, Direction, out Hit, Collider.radius, LineOfSightLayers))
            {
                if (Hit.transform.GetComponent<IDamageable>() != null)
                {
                    OnGainSight?.Invoke(player);
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(IDamageable player)
    {
        WaitForSeconds Wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return Wait;
        }
    }
}