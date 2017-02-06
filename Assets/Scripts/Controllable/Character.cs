using UnityEngine;
using System.Collections;
using System;
using GameInput;

public class CharacterState
{
    public enum eState { Default, Driving };

    public eState State;
    public InteractiveTrigger.InteractionInfo InteractiveInfo;

    public CharacterState(eState state)
    {
        State = state;
        InteractiveInfo = null;
    }
}

public abstract class Character : MonoBehaviour, IControllable
{
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _runSpeed;

    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected Collider _bodyCollider;

    protected float _currentSpeed;
    protected CharacterState _state;

    public CharacterState State { get { return _state; } }

    protected abstract void Move(float h, float v);

    private void Awake()
    {
        ChangeState(new CharacterState(CharacterState.eState.Default));
    }

    public virtual bool TryChangeState(CharacterState state)
    {
        if (_state == state)
        {
            return false;
        }

        ChangeState(state);
        return true;
    }

    protected void ChangeState(CharacterState state)
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        switch (state.State)
        {
            case CharacterState.eState.Default:
                _rigidbody.isKinematic = false;
                _bodyCollider.enabled = true;
                break;

            case CharacterState.eState.Driving:
                _rigidbody.isKinematic = true;
                _bodyCollider.enabled = false;
                break;
        }

        _state = state;
    }

    public virtual void HandleAxis(Axis a, float h, float v)
    {
        if (a == Axis.LStick)
        {
            // move character
            Move(h, v);
        }
    }

    public virtual void HandleButtonHeld(Button b, float duration)
    {
        //throw new NotImplementedException();
    }

    public virtual void HandleButtonPress(Button b)
    {
        //throw new NotImplementedException();
    }

    public virtual void HandleButtonRelease(Button b)
    {
        //throw new NotImplementedException();
    }
}
