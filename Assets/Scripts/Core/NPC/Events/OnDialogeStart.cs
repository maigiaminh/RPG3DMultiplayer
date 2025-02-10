using System;
using Unity.Behavior.GraphFramework;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnDialogeStart")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnDialogeStart", message: "[Player] open [Dialoge]", category: "Events", id: "f6a1472b171b3ba8c3ef4b729b224f64")]
public partial class OnDialogeStart : EventChannelBase
{
    public delegate void OnDialogeStartEventHandler(GameObject Player, GameObject Dialoge);
    public event OnDialogeStartEventHandler Event; 

    public void SendEventMessage(GameObject Player, GameObject Dialoge)
    {
        Event?.Invoke(Player, Dialoge);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> PlayerBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Player = PlayerBlackboardVariable != null ? PlayerBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<GameObject> DialogeBlackboardVariable = messageData[1] as BlackboardVariable<GameObject>;
        var Dialoge = DialogeBlackboardVariable != null ? DialogeBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(Player, Dialoge);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        OnDialogeStartEventHandler del = (Player, Dialoge) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Player;

            BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
            if(var1 != null)
                var1.Value = Dialoge;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as OnDialogeStartEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as OnDialogeStartEventHandler;
    }
}

