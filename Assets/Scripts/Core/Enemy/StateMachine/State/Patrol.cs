using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : IState
{
    private readonly IEnemy _enemy;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private static NavMeshTriangulation triangulation;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private string _patrolArea;
    private float _patrolSpeed;
    private float _initialSpeed;
    public float patrolTime = 5f;
    public float timer = 0f;
    private float _deltaTime = 2f;
    private Vector3 _targetPosition;

    public Patrol(IEnemy enemy, NavMeshAgent navMeshAgent, Animator animator, float patrolSpeed, float patrolTime, Enemy.SpawnArea spawnArea)
    {
        _enemy = enemy;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _patrolSpeed = patrolSpeed;
        _patrolArea = spawnArea.ToString();
        this.patrolTime = Mathf.Clamp(UnityEngine.Random.Range(1, patrolTime), 1, patrolTime + _deltaTime);

        if (triangulation.vertices == null) triangulation = NavMesh.CalculateTriangulation();
    }

    public void Tick()
    {
        timer += Time.deltaTime;
        if (Vector3.Distance(_navMeshAgent.transform.position, _targetPosition) < .1f)
        {
            PatrolToTarget();
        }

    }

    public void OnEnter()
    {
        timer = 0f;


        _navMeshAgent.speed = _patrolSpeed;

        _initialSpeed = _navMeshAgent.speed;
        _animator.SetInteger(Speed, 1);

        PatrolToTarget();
        if (!_navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a valid NavMesh.");
            return;
        }
    }



    public void OnExit()
    {
        _animator.SetInteger(Speed, 0);
        _navMeshAgent.speed = _initialSpeed;

        _navMeshAgent.ResetPath();
    }

    private void PatrolToTarget()
    {
        NavMeshHit hit;

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 30f; // Bán kính 30
        randomDirection += _navMeshAgent.transform.position; // Offset từ vị trí hiện tại

        // Tìm vị trí hợp lệ trên NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, 30, NavMesh.AllAreas))
        {
            Debug.Log($"Found valid NavMesh position at {hit.position}");
            _targetPosition = hit.position;
            _navMeshAgent.SetDestination(_targetPosition);
        }
        else
        {
            Debug.LogWarning("Failed to find valid position on NavMesh.");
        }
        _targetPosition = hit.position;

        // Log the final target position
        Debug.Log($"Target position set to: {_targetPosition}");

        _navMeshAgent.SetDestination(_targetPosition);
    }



}
