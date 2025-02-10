using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : IState
{
    private readonly DissolvingController _dissolvingController;

    public Dissolve(DissolvingController dissolvingController)
    {
        _dissolvingController = dissolvingController;
    }
    public void Tick()
    {
    }

    public void OnEnter()
    {
        _dissolvingController.Dissolve();
    }

    public void OnExit()
    {
    }



}
