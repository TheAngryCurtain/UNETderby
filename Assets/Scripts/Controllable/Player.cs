using UnityEngine;
using System.Collections;
using GameInput;
using System;

public class Player : Character
{
    public Action<bool, InteractiveTrigger.InteractionInfo> OnIInfoAccepted;
    
    [SerializeField] ThirdPersonCamera _camera;

    private Vector3 _movementVector = Vector3.zero;
    private Quaternion _targetRotation;
    private Quaternion _newRotation;

    protected InteractiveTrigger.InteractionInfo _interactiveInfo;

    private void Start()
    {
        InputController.Instance.SetPlayerControllable(0, this);
    }

    protected override void Move(float h, float v)
    {
        if (_camera != null)
        {
            // movement
            _movementVector.x = h;
            _movementVector.z = v;
            _movementVector = _camera.transform.TransformDirection(_movementVector) * _currentSpeed * Time.deltaTime;
            _movementVector.y = 0f;

            _rigidbody.MovePosition(transform.position + _movementVector);

            // rotation
            if (_movementVector != Vector3.zero)
            {
                transform.LookAt(transform.position + _movementVector);
            }
        }
    }

    private void MoveCamera(float h, float v)
    {
        if (_camera != null)
        {
            _camera.HandleAxisInput(h, v);
        }
    }

    public void AcceptInteractiveInfo(bool valid, InteractiveTrigger.InteractionInfo info)
    {
        if (valid)
        {
            Debug.LogFormat("({0}) {1}", info.ActionButton, info.CalloutText);
        }
        else
        {
            Debug.Log("---");
        }

        _interactiveInfo = info;

        // TODO hook up to UI
        // need to register event with UI manager when that gets created
        if (OnIInfoAccepted != null)
        {
            OnIInfoAccepted(valid, info);
        }
    }

    public override void HandleAxis(Axis a, float h, float v)
    {
        base.HandleAxis(a, h, v);

        if (a == Axis.RStick)
        {
            MoveCamera(h, v);
        }
    }

    public override void HandleButtonPress(Button b)
    {
        // if the player is in an interactive zone, check if you've interacted with it
        if (_interactiveInfo != null && _interactiveInfo.Action != null)
        {
            if (b == _interactiveInfo.ActionButton)
            {
                // pack parameters
                object parameters = new object[]
                {
                    this,
                    _interactiveInfo.ActionButton
                };

                // update info in state
                _state.InteractiveInfo = _interactiveInfo;

                // TODO update UI
                if (OnIInfoAccepted != null)
                {
                    OnIInfoAccepted(false, null);
                }

                _interactiveInfo.Action.Invoke(parameters);
                return;
            }
        }

        if (b == Button.X)
        {
            _currentSpeed = _runSpeed;
        }
    }

    public override void HandleButtonRelease(Button b)
    {
        if (b == Button.X)
        {
            _currentSpeed = _walkSpeed;
        }
    }
}
