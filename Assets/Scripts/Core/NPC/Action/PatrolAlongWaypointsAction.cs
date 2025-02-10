using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "patrol along waypoints ", story: "[Agent] patrols along [waypoints]", category: "Action", id: "ffe0f73e330622569f97958f55a2ede9")]
public partial class PatrolAlongWaypointsAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;

    private NavMeshAgent _navMeshAgent;
    private int _currentWaypointIndex;
    private List<GameObject> _currentWaypointsList;

    protected override Status OnStart()
    {
        if(Agent.Value == null) {
            Debug.LogError("Agent does not exist");
            return Status.Failure;
        }
        _navMeshAgent = Agent.Value;
        _currentWaypointIndex = 0;
        _currentWaypointsList = Waypoints.Value;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 targetPos = _currentWaypointsList[_currentWaypointIndex].transform.position;

        if(_currentWaypointIndex >= _currentWaypointsList.Count - 1) return Status.Success;

        if (Vector3.Distance(agentPos, targetPos) < .1f)
        {
            _currentWaypointIndex++;
        }

        _navMeshAgent.SetDestination(targetPos);
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

