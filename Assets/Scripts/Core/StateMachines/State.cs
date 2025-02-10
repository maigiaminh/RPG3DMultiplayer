using UnityEngine;

public abstract class State
{
    public abstract void Enter();
    public abstract void Tick(float deltaTime);
    public abstract void Exit();

    protected float GetNormalizedTime(Animator animator, string tag, int layer)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(layer);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(layer);

        if (animator.IsInTransition(layer) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(layer) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }

    public virtual bool CanTakeDamage() => true;

}
