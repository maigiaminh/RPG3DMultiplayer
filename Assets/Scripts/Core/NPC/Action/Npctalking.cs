using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Npc Talking", story: "[NpcStateMachine] [Npc] talk to [Player] have and [PlayerDetector]", category: "Action", id: "96d08e82d2dcffe92c3b23d6d56731e2")]
public partial class NpcTalkingAction : Action
{
    [SerializeReference] public BlackboardVariable<NpcStateMachine> NpcStateMachine;
    [SerializeReference] public BlackboardVariable<GameObject> Npc;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<PlayerDetector> PlayerDetector;
    private bool _isDialogeEnded = false;
    protected override Status OnStart()
    {
        _isDialogeEnded = false;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeQuest += OnDialogeStarted;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeQuest += OnDialogeEnded;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeFeature += OnDialogeStarted;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeFeature += OnDialogeEnded;

        return Status.Running;
    }



    protected override Status OnUpdate()
    {
        if (_isDialogeEnded)
        {
            return Status.Success;
        }

        return Status.Running;
    }


    protected override void OnEnd()
    {
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeQuest -= OnDialogeStarted;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeQuest -= OnDialogeEnded;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeFeature -= OnDialogeStarted;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeFeature -= OnDialogeStarted;
    }

    private void OnDialogeStarted(DialogeItem item)
    {
        if (!PlayerDetector.Value.player) return;

        NpcStateMachine.Value.SwitchState(new NpcTalkState(NpcStateMachine.Value));
    }

    private void OnDialogeStarted(FeatureItem item)
    {
        if (!PlayerDetector.Value.player) return;

        NpcStateMachine.Value.SwitchState(new NpcTalkState(NpcStateMachine.Value));
    }

    private void OnDialogeEnded(DialogeItem item)
    {
        if (!PlayerDetector.Value.player) return;

        _isDialogeEnded = true;
        NpcStateMachine.Value.SwitchState(new NpcIdleState(NpcStateMachine.Value));
    }

    private void OnDialogeEnded(FeatureItem item)
    {
        if (!PlayerDetector.Value.player) return;

        _isDialogeEnded = true;
        NpcStateMachine.Value.SwitchState(new NpcIdleState(NpcStateMachine.Value));
    }


}
