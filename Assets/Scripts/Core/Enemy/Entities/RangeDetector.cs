using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Collider))]
public class RangeDetector : MonoBehaviour
{
    public bool TargetInRange => targetsInRange.Count > 0;

    public List<IDamageable> targetsInRange = new List<IDamageable>();

    private List<Enemy.TargetType> targetTypes = new List<Enemy.TargetType>(){
        Enemy.TargetType.Player
    };

    public delegate void TargetDetectedEvent(IDamageable damageable);
    public TargetDetectedEvent OnTargetChanged;



    public Transform detectTarget;

    private float _updateTargetInterval = 2f;
    Coroutine _updateTargetCoroutine;


    private void OnEnable()
    {
        targetsInRange.Clear();
        detectTarget = null;

        if (_updateTargetCoroutine == null)
        {
            _updateTargetCoroutine = StartCoroutine(UpdateTarget());
        }
    }

    private void OnDisable()
    {
        targetsInRange.Clear();
        detectTarget = null;

        if (_updateTargetCoroutine != null)
        {
            StopCoroutine(_updateTargetCoroutine);
            _updateTargetCoroutine = null;
        }
    }


    public void Initialize(List<Enemy.TargetType> targetTypes)
    {
        this.targetTypes = targetTypes;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ChaseTargetDetector: OnTriggerEnter");
        // bool isTarget = false;
        // foreach (var type in targetTypes)
        // {
        //     if (!other.CompareTag(type.ToString())) continue;
        //     isTarget = true;
        // }
        // if (!isTarget) return;
        if (!other.CompareTag("Player")) return;
        other.TryGetComponent<IDamageable>(out IDamageable target);
        if (!targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);
        }
        detectTarget = other.transform;
        detectTarget.TryGetComponent<IDamageable>(out IDamageable detectTargetDamageable);
        OnTargetChanged?.Invoke(detectTargetDamageable);

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("ChaseTargetDetector: OnTriggerExit");
        // bool isTarget = false;
        // foreach (var type in targetTypes)
        // {
        //     if (!other.CompareTag(type.ToString())) continue;

        //     isTarget = true;
        // }
        // if (!isTarget) return;
        other.TryGetComponent<IDamageable>(out IDamageable target);
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
        }

        if (other.transform == detectTarget)
        {
            detectTarget = targetsInRange.Count > 0 ? targetsInRange[0].GetTransform() : null;
            if (detectTarget != null)
            {
                detectTarget.TryGetComponent<IDamageable>(out IDamageable detectTargetDamageable);
                OnTargetChanged?.Invoke(detectTargetDamageable);

            }
        }
    }
    IEnumerator UpdateTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(_updateTargetInterval);

            if (targetsInRange.Count == 0)
            {
                detectTarget = null;
                continue;
            }

            foreach (var target in targetsInRange)
            {
                if (this.detectTarget == null) { this.detectTarget = target.GetTransform(); continue; }
                if (Vector3.Distance(transform.position, target.GetTransform().position) < Vector3.Distance(transform.position, this.detectTarget.position))
                {
                    this.detectTarget = target.GetTransform();
                }
            }
        }
    }
}
