using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Player By Tag", story: "[Npc] Find [Player]", category: "Action", id: "f878ceb3fa1859ca50f3c61370f5d68b")]
public partial class FindPlayerByTagAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Npc;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    protected override Status OnStart()
    {
        if(Player.Value == null){
            var player = GameObject.FindGameObjectWithTag("Player");
            Player.Value = player;
        }
        return Status.Success;
    }

}

