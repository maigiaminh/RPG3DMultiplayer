using UnityEngine;
using UnityEngine.AI;

public class Fasten : IState
{
    private Enemy _enemy;
    private readonly Animator _animator;
    private NavMeshAgent _navMeshAgent;

    public float counterFasten;
    public Fasten(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator)
    {
        _enemy = enemy;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
    }
    public void OnEnter()
    {
        Debug.Log("Fasten");
        counterFasten = 0;
        _animator.speed = 0;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.ResetPath();
    }

    public void OnExit()
    {
        Debug.Log("Exit Fasten");
        _animator.speed = 1;
        _navMeshAgent.isStopped = false;
    }

    public void Tick()
    {
        counterFasten += Time.deltaTime;
        Debug.Log("Fasten: " + counterFasten);
    }
}
