using UnityEngine;
using UnityEngine.AI;

public class Freeze : IState
{
    private Enemy _enemy;
    private readonly Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private GameObject _icePrefab;
    public float counterFreeze;
    public Freeze(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator, GameObject icePrefab)
    {
        _enemy = enemy;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
        _icePrefab = icePrefab;
    }
    public void OnEnter()
    {
        Debug.Log("Enter Freeze");
        counterFreeze = 0;
        _animator.SetTrigger("Freeze");        
        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _icePrefab.SetActive(true);
        _navMeshAgent.ResetPath();
    }

    public void OnExit()
    {
        Debug.Log("Exit Freeze");
        _animator.ResetTrigger("Freeze");
        _icePrefab.SetActive(false);
        _navMeshAgent.isStopped = false;
    }

    public void Tick()
    {
        counterFreeze += Time.deltaTime;
    }

}
