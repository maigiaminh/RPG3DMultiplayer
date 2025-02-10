using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Player In Range", story: "[PlayerDetector]", category: "Action", id: "30b61b04fda273681fc6cc200e27a03b")]
public partial class CheckPlayerInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<PlayerDetector> PlayerDetector;

    protected override Status OnStart()
    {   
        return PlayerDetector.Value.player != null ? Status.Success : Status.Failure;
    }

}

