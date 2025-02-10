using System;
using Unity.Behavior.GraphFramework;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Npc Open Dialoge Alert")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Npc Open Dialoge Alert", message: "[Npc] open Dialoge with [QuestPoint]", category: "Events", id: "b0034e4104d3ca911ea49a0132ced520")]
public partial class NpcOpenDialogeAlert : EventChannelBase
{
    public delegate void NpcOpenDialogeAlertEventHandler(GameObject Npc, QuestPoint QuestPoint);
    public event NpcOpenDialogeAlertEventHandler Event; 

    public void SendEventMessage(GameObject Npc, QuestPoint QuestPoint)
    {
        Event?.Invoke(Npc, QuestPoint);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> NpcBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Npc = NpcBlackboardVariable != null ? NpcBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<QuestPoint> QuestPointBlackboardVariable = messageData[1] as BlackboardVariable<QuestPoint>;
        var QuestPoint = QuestPointBlackboardVariable != null ? QuestPointBlackboardVariable.Value : default(QuestPoint);

        Event?.Invoke(Npc, QuestPoint);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        NpcOpenDialogeAlertEventHandler del = (Npc, QuestPoint) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Npc;

            BlackboardVariable<QuestPoint> var1 = vars[1] as BlackboardVariable<QuestPoint>;
            if(var1 != null)
                var1.Value = QuestPoint;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as NpcOpenDialogeAlertEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as NpcOpenDialogeAlertEventHandler;
    }
}

