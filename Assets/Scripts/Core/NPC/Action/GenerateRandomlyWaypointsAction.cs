using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEditor.Analytics;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Generate randomly Waypoints", story: "Generate [Waypoints] on [Surface] surface of [Agent]", category: "Action", id: "3b37f5637e56c55c631721eec3ecea90")]
public partial class GenerateRandomlyWaypointsAction : Action
{
    [SerializeReference] public BlackboardVariable<List<Vector3>> Waypoints;
    [SerializeReference] public BlackboardVariable<NavMeshSurface> Surface;
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;

    private NavMeshSurface _navMeshSurface;
    private NavMeshAgent _navMeshAgent;

    private float _sampleRadius = 100; // Bán kính cho vị trí ngẫu nhiên trong NavMesh

    protected override Status OnStart()
    {
        if (Waypoints.Value == null)
            Waypoints.Value = new List<Vector3>();
        else
            Waypoints.Value.Clear();
        if (_navMeshSurface == null)
            _navMeshSurface = Surface.Value.GetComponent<NavMeshSurface>();

        _navMeshAgent = Agent.Value;

        if (_navMeshAgent)
        {
            Debug.Log("GenerateRandomlyWaypointList OnStart _navMeshAgent is not null");
        }
        if (_navMeshSurface)
        {
            Debug.Log("GenerateRandomlyWaypointList OnStart _navMeshSurface is not null");
        }

        // Tìm một điểm thích hợp ngẫu nhiên và thêm vào danh sách Waypoints
        Vector3 randomPoint;
        int randomAmountWaypoints = UnityEngine.Random.Range(5, 10);
        while (FindRandomPoint(_navMeshAgent.transform.position, _sampleRadius, out randomPoint) && Waypoints.Value.Count < randomAmountWaypoints)
        {
            Waypoints.Value.Add(randomPoint);
        }

        return Status.Success;
    }

    private bool FindRandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++) // Thử tối đa 30 lần để tìm vị trí hợp lệ
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += center;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}

