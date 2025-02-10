using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Switch To Idle", story: "[NpcStateMachine] [Self] Idle", category: "Action", id: "6466f8076f9b1006a12e16b24effb6f8")]
public partial class SwitchToIdleAction : Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    private NavMeshAgent _navMeshAgent;

    protected override Status OnStart()
    {
        if (_navMeshAgent == null) _navMeshAgent = Self.Value.GetComponent<NavMeshAgent>();
        _navMeshAgent.isStopped = true;

        NpcStateMachine.Value.SwitchState(new NpcIdleState(NpcStateMachine.Value));
        return Status.Success;
    }

}

