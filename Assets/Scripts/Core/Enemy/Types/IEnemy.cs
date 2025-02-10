
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public interface IEnemy
{

    public GameObject GameObject { get; }
    public NavMeshAgent Agent {get;}
    public void SetPool(ObjectPool<IEnemy> pool);
    public void Attack();
}
