using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Npc Contact", story: "[NpcStateMachine] [Npc] [Contact] to [Player]", category: "Action", id: "75e10447725ba7e3197cf9ca31f77e1e")]
public partial class NpcDanceAction : Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Npc;
    [SerializeReference] public BlackboardVariable<string> Contact;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    private float _duration;
    private float _counterTime = 0;
    private NavMeshAgent _navMeshAgent;
    protected override Status OnStart()
    {
        if (Player == null) return Status.Failure;
        _navMeshAgent = Npc.Value.GetComponent<NavMeshAgent>();
        if(_navMeshAgent != null){
            if(_navMeshAgent.isOnNavMesh) _navMeshAgent.isStopped = true;
        }

        _counterTime = 0;
        SwitchState(Contact.Value);
        _duration = Npc.Value.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;

        return Status.Running;
    }

    private void SwitchState(string value)
    {
        int randomDance = UnityEngine.Random.Range(1, 3);
        switch (value)
        {
            case "Dance":
                NpcStateMachine.Value.SwitchState(new NpcDanceState(NpcStateMachine.Value, randomDance));
                break;
            case "Kiss":
                NpcStateMachine.Value.SwitchState(new NpcKissState(NpcStateMachine.Value));
                break;
        }
    }

    protected override Status OnUpdate()
    {

        _counterTime += Time.deltaTime;

        return _counterTime >= _duration ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

