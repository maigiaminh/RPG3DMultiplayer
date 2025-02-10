using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Dialoge Opened", story: "Check Dialoge Opened with [QuestPoint]", category: "Action", id: "49958555a8752fe299b8571ae0683747")]
public partial class NpcOpenDialogeAction : Action
{
    [SerializeReference] public BlackboardVariable<QuestPoint> QuestPoint;
    protected override Status OnStart()
    {
        return DialogeUIManager.Instance.IsDialogOpen && QuestPoint.Value.GetQuestInfo().dialogeInfo.actorName == DialogeUIManager.Instance.dialogeItem.actorName
        ? Status.Success
        : Status.Failure;
    }


}

