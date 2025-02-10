using System;
using Unity.Behavior;
using UnityEngine;


[Serializable]
[NodeDescription(name: "Npc navigate to Player ", story: "[NpcStateMachine] [Agent] navigate to [Player] in [_range] Range", category: "Action", id: "9b243be78f515fadd7392f2bff88ed8c")]
public class NpcNavigateToPlayer : Unity.Behavior.Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Range;
    
    protected override Status OnStart()
    {
        NpcStateMachine.Value.SwitchState(new NpcWalkState(NpcStateMachine.Value));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Vector3.Distance(Agent.Value.transform.position, Player.Value.transform.position) < Range.Value)
        {
            Agent.Value.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
            return Status.Success;
        }
        else
        {
            Agent.Value.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(Player.Value.transform.position);
        }
        return Status.Running;
    }
}

