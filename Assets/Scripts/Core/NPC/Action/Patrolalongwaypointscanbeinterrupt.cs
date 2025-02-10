using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol along waypoints can be interrupt", story: "[NpcStateMachine] [Agent] patrols along [Waypoints] with [PlayerDetector]", category: "Action", id: "84595958488916bccca2b596918b28d5")]
public partial class PatrolAlongWaypointsAndStopWhenPlayerInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Waypoints;
    [SerializeReference] public BlackboardVariable<PlayerDetector> PlayerDetector;
    private int _currentWaypointIndex = 0;
    private int _amountWaypoints = 0;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    protected override Status OnStart()
    {
        NpcStateMachine.Value.SwitchState(new NpcWalkState(NpcStateMachine.Value));

        if (_navMeshAgent == null)
            _navMeshAgent = Agent.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if(_navMeshAgent.isStopped)
            _navMeshAgent.isStopped = false;
            
        if (Waypoints.Value.Count > 0)
            _amountWaypoints = Waypoints.Value.Count;
        _currentWaypointIndex = 0;


        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(PlayerDetector.Value.player != null) return Status.Failure;


        Debug.Log("AgentPatrolsAlongWaypointsAndStopWhenPlayerInRange OnUpdate");
        if (Vector3.Distance(Agent.Value.transform.position, Waypoints.Value[_currentWaypointIndex]) < 2f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % Waypoints.Value.Count;
        }
        if (_currentWaypointIndex == _amountWaypoints - 1)
        {
            Debug.Log("AgentPatrolsAlongWaypointsAndStopWhenPlayerInRange OnUpdate _currentWaypointIndex: " + _currentWaypointIndex);
            return Status.Success;
        }
        _navMeshAgent.SetDestination(Waypoints.Value[_currentWaypointIndex]);

        Debug.Log("AgentPatrolsAlongWaypointsAndStopWhenPlayerInRange OnUpdate _currentWaypointIndex: " + _currentWaypointIndex);
        return Status.Running;
    }

}

