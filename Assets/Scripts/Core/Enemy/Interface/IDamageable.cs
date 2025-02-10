
using UnityEngine;

public interface IDamageable 
{
    void TakeDamage(Transform trans ,int damage);
    Transform GetTransform();
    void ApplyCC(CCType ccType,float ccDuration);
}
