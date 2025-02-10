using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TrainingDummy : MonoBehaviour, IDamageable
{
    public GameObject frozenEffect;
    protected Animator _animator;
    private GameEventManager _gameEventManager;

    protected enum TrainingDummyState
    {
        Idle,
        Patrol,
        TakeDamage,
        Freeze
    }

    private TrainingDummyState _currentState = TrainingDummyState.Idle;
    protected bool _isTakingDamage = false;
    private float _takeDamageTime = 0;
    private float _counterTakeAnim = 0;

    protected bool _isInCC = false;

    public Transform GetTransform() => transform;
    protected virtual void Awake()
    {
        _gameEventManager = GameEventManager.Instance;
        _animator = GetComponent<Animator>();

        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Equals("TakeDamage"))
            {
                _takeDamageTime = clip.length;
            }
        }
    }
    public virtual void ApplyCC(CCType ccType, float ccDuration)
    {
        if (ccType == CCType.Freeze) GetFreeze();

        _isInCC = true;

        StartCoroutine(DisableFrozenEffect(ccDuration));
    }


    protected virtual void Update()
    {
        if (_isInCC) return;
        if (_isTakingDamage)
        {
            _counterTakeAnim -= Time.deltaTime;
            if (_counterTakeAnim <= 0)
            {
                _isTakingDamage = false;
                ChangeAnimState(TrainingDummyState.Idle);
            }
        }
    }


    IEnumerator DisableFrozenEffect(float ccDuration)
    {
        yield return new WaitForSeconds(ccDuration);
        frozenEffect.SetActive(false);
        _isInCC = false;
    }


    public void TakeDamage(Transform trans, int damage)
    {
        _gameEventManager.DamageEvent.NotifyDamage(transform, damage);
        ChangeAnimState(TrainingDummyState.TakeDamage);
    }


    private void GetFreeze()
    {
        frozenEffect.SetActive(true);
        ChangeAnimState(TrainingDummyState.Freeze);
    }


    protected void ChangeAnimState(TrainingDummyState state)
    {
        if(_isInCC) return;
        if (_currentState == state) return;
        if (state == TrainingDummyState.TakeDamage)
        {
            _counterTakeAnim = _takeDamageTime;
            _isTakingDamage = true;
        }
        _currentState = state;
        _animator.SetInteger("AnimState", (int)_currentState);

    }

}
