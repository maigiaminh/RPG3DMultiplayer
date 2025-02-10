using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Idle when player nearby", story: "[NpcStateMachine] [Npc] Idle when [PlayerDetector]", category: "Action", id: "fad1c6c3923443bf89034aa313e2f216")]
public partial class IdleWhenPlayerNearbyAction : Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Npc;
    [SerializeReference] public BlackboardVariable<PlayerDetector> PlayerDetector;
    private NavMeshAgent _navMeshAgent;

    protected override Status OnStart()
    {
        _navMeshAgent = Npc.Value.GetComponent<NavMeshAgent>();
        if (_navMeshAgent != null) {
            if(_navMeshAgent.isOnNavMesh) _navMeshAgent.isStopped = true;
        }

        NpcStateMachine.Value.SwitchState(new NpcIdleState(NpcStateMachine.Value));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (PlayerDetector.Value.player == null) return Status.Success;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

