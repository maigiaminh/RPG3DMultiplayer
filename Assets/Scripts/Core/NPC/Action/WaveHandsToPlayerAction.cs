using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wave hands to player", story: "[NpcStateMachine] [Npc] waves hand to [player]", category: "Action", id: "71940db4a6f2b7454cfd79cb7482e265")]
public partial class WaveHandsToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Npc;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private float _waveHandDuration;

    private float _counter = 0;
    private NavMeshAgent _navMeshAgent;

    protected override Status OnStart()
    {
        if(Player == null) return Status.Failure;
        _navMeshAgent = Npc.Value.GetComponent<NavMeshAgent>();
        if(_navMeshAgent != null) {
            if(_navMeshAgent.isOnNavMesh) _navMeshAgent.isStopped = true;
        }
        _counter = 0;
        NpcStateMachine.Value.SwitchState(new NpcWaveHandState(NpcStateMachine.Value, Npc, Player));
        _waveHandDuration = Npc.Value.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _counter += Time.deltaTime;
        return _counter >= _waveHandDuration ? Status.Success : Status.Running;
    }

}

