using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DynamicTrainingDummy : TrainingDummy
{
    private NavMeshAgent _agent;
    private Vector3 _currentDestination;
    private const float IDLE_TIME = 5;
    private const float PATROL_TIME = 5;
    private float _counterIdle = 0;

    private float _counterPatrol = 0;
    private bool _isPatrolling = false;




    protected override void Awake()
    {
        base.Awake();
        _counterIdle = 0;
        _agent = GetComponent<NavMeshAgent>();

        _currentDestination = FindRandomlyPos();
    }

    protected override void Update()
    {
        base.Update();
        
        if(_isInCC) {
            _agent.ResetPath();
            _agent.isStopped = true;
            return;
        }
        _agent.isStopped = false;
        if (_isTakingDamage) return;
        if (_currentDestination == Vector3.zero)
        {
            FindRandomlyPos();
            return;
        }
        if (_counterIdle > 0)
        {
            _counterIdle -= Time.deltaTime;
            return;
        }
        if (_isPatrolling)
        {
            _counterPatrol -= Time.deltaTime;
            if (_counterPatrol <= 0)
            {
                _currentDestination = FindRandomlyPos();
                ChangeAnimState(TrainingDummyState.Idle);
                _isPatrolling = false;
                _counterIdle = IDLE_TIME;
            }
        }
        if (Vector3.Distance(transform.position, _currentDestination) < 0.1f)
        {
            _counterIdle = IDLE_TIME;
            _agent.ResetPath();
            ChangeAnimState(TrainingDummyState.Idle);
            _currentDestination = FindRandomlyPos();
            return;
        }

        ChangeAnimState(TrainingDummyState.Patrol);
        _agent.SetDestination(_currentDestination);
        _isPatrolling = true;
    }

    public override void ApplyCC(CCType ccType, float ccDuration)
    {
        base.ApplyCC(ccType, ccDuration);

    }



    public Vector3 FindRandomlyPos()
    {
        NavMeshHit hit;
        Vector3 randomPos = transform.position + Random.insideUnitSphere * 5;
        // Ray ray = new Ray(transform.position, targetPosition - transform.position);
        // RaycastHit
        // if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        // {
        //     Debug.Log($"Ray hit {hit.collider.name}");
        // }
        // else
        // {
        //     Debug.Log("No obstacle detected.");
        // }
        Debug.Log($"Generated Random Position: {randomPos}");
        if (NavMesh.SamplePosition(randomPos, out hit, 10, NavMesh.AllAreas))
        {
            Debug.Log($"Found Position on NavMesh: {hit.position}");
        }
        else
        {
            Debug.LogError("Failed to find position on NavMesh.");
            return Vector3.zero;
        }
        return hit.position;
    }

}
