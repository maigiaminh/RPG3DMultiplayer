using System;
using BrainFailProductions.PolyFew;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisibleControl : MonoBehaviour
{
    private bool _enable = true;

    private void Start()
    {
        DisableLod();
        EnableLogic(false);
    }

    private void DisableLod()
    {
        GetComponent<LODGroup>().enabled = false;
        GetComponent<PolyFew>().enabled = false;
    }

    #region Optimalize
    public void EnableLogic(bool isEnabled)
    {
        if (_enable == isEnabled) return;
        _enable = isEnabled;
        // Lấy tất cả các Component trên GameObject hiện tại
        Component[] components = GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == this) continue;
            if (component is MonoBehaviour monoBehaviour)
            {
                if (monoBehaviour is PolyFew polyFew) continue;
                if (monoBehaviour is TestApplyDamage testApplyDamage)
                {
                    testApplyDamage.enabled = false;
                    continue;
                }
                if (monoBehaviour is Enemy && isEnabled)
                {
                    Enemy enemy = monoBehaviour as Enemy;
                    enemy.SetState(new Idle(enemy, enemy.Agent, enemy.animator, enemy.IdleTime));
                }
                monoBehaviour.enabled = isEnabled;
            }
            else if (component is NavMeshAgent navMeshAgent) navMeshAgent.enabled = isEnabled;
            else if (component is Animator animator) animator.enabled = isEnabled;
            else if (component is Collider collider) continue;
            else if (component is Rigidbody rigidbody) rigidbody.isKinematic = !isEnabled;
        }
    }
    #endregion

}
