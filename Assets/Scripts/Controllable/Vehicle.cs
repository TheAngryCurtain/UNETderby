using UnityEngine;
using System.Collections;
using System;
using GameInput;

public class Vehicle : MonoBehaviour, IControllable, IInteractable
{
    [SerializeField] protected float _maxTorque;
    [SerializeField] protected float _maxSteerAngle;
    [SerializeField] protected Axle[] _axles;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected Transform[] _seats;
    [SerializeField] protected Transform _driverDoorTransform;
    [SerializeField] protected Transform _centerOfMass;

    [SerializeField] protected float _driftWheelFriction;

    private Player _passenger;
    private Button _actionButton;
    private Axle _currentAxle;
    
    private float _steerValue;
    private float _throttle;
    private float _defaultWheelFriction;

    private Vector3 _activeWheelPos;
    private Quaternion _activeWheelRot;
    private float _currentSpeed;

    public float CurrentSpeed { get { return _currentSpeed; } }
    public Transform DriverSeat { get { return _seats[0]; } }

    [System.Serializable]
    public class Axle
    {
        public WheelCollider LeftWheelCollider;
        public WheelCollider RightWheelCollider;

        public Transform LeftWheelTransform;
        public Transform RightWheelTransform;

        public bool Motor;
        public bool Steer;
    }

    protected virtual void Start()
    {
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
        _defaultWheelFriction = _axles[0].LeftWheelCollider.sidewaysFriction.stiffness;
    }

    protected virtual void Steer(float h, float v)
    {
        _steerValue = h;
    }

    protected virtual void Move(float h, float v)
    {
        _throttle = h;
    }

    protected virtual void Brake(float h, float v)
    {
        _throttle = -h;
    }

    protected virtual void FixedUpdate()
    {
        float motor = _maxTorque * _throttle;
        float steering = _maxSteerAngle * _steerValue;

        // apply forces to wheels
        for (int i = 0; i < _axles.Length; ++i)
        {
            _currentAxle = _axles[i];
            if (_currentAxle.Steer)
            {
                _currentAxle.LeftWheelCollider.steerAngle = steering;
                _currentAxle.RightWheelCollider.steerAngle = steering;
            }

            if (_currentAxle.Motor)
            {
                _currentAxle.LeftWheelCollider.motorTorque = motor;
                _currentAxle.RightWheelCollider.motorTorque = motor;
            }

            // reflect wheel collider movement onto visual wheels
            ApplyPositionToVisuals(_currentAxle);
        }

        _currentSpeed = _rigidbody.velocity.magnitude;
    }

    private void ApplyPositionToVisuals(Axle current)
    {
        current.LeftWheelCollider.GetWorldPose(out _activeWheelPos, out _activeWheelRot);
        current.LeftWheelTransform.position = _activeWheelPos;
        current.LeftWheelTransform.rotation = _activeWheelRot;

        current.RightWheelCollider.GetWorldPose(out _activeWheelPos, out _activeWheelRot);
        current.RightWheelTransform.position = _activeWheelPos;
        current.RightWheelTransform.rotation = _activeWheelRot;
    }

    private void ChangeSidewaysWheelFriction(float value)
    {
        WheelFrictionCurve curve = _axles[0].LeftWheelCollider.sidewaysFriction;
        curve.stiffness = value;
        for (int i = 0; i < _axles.Length; ++i)
        {
            _currentAxle = _axles[1];
            _currentAxle.LeftWheelCollider.sidewaysFriction = curve;
            _currentAxle.RightWheelCollider.sidewaysFriction = curve;
        }
    }

    #region IInteractable
    public void InteractAction(object parameters)
    {
        object[] param = (object[])parameters;
        Player p = (Player)param[0];
        _actionButton = (Button)param[1];

        bool canChangeState = p.TryChangeState(new CharacterState(CharacterState.eState.Driving));
        if (canChangeState)
        {
            _passenger = p;
            
            // align for driving
            _passenger.transform.SetParent(this.transform);
            _passenger.transform.position = DriverSeat.position;
            _passenger.transform.rotation = DriverSeat.rotation;

            InputController.Instance.SetPlayerControllable(0, this);
        }
    }
    #endregion

    #region IControllable
    public virtual void HandleAxis(Axis a, float h, float v)
    {
        if (a == Axis.LStick)
        {
            // steering
            Steer(h, v);
        }
        else if (a == Axis.RStick)
        {
            // pass on camera
            if (_passenger != null)
            {
                _passenger.HandleAxis(a, h, v);
            }
        }
        else if (a == Axis.RTrigger)
        {
            // power
            Move(h, v);
        }
        else if (a == Axis.LTrigger)
        {
            // brake
            Brake(h, v);
        }
    }

    public virtual void HandleButtonHeld(Button b, float duration)
    {
        //throw new NotImplementedException();
    }

    public virtual void HandleButtonPress(Button b)
    {
        if (b == _actionButton)
        {
            // exit the vehicle
            bool canChangeState = _passenger.TryChangeState(new CharacterState(CharacterState.eState.Default));
            if (canChangeState)
            {
                _passenger.transform.position = _driverDoorTransform.position + (_driverDoorTransform.up * 0.5f);
                _passenger.transform.rotation = _driverDoorTransform.rotation;
                _passenger.transform.SetParent(null);

                InputController.Instance.SetPlayerControllable(0, _passenger);
                _passenger = null;

                return;
            }
        }

        if (b == Button.X)
        {
            // drift
            ChangeSidewaysWheelFriction(_driftWheelFriction);
        }
    }

    public virtual void HandleButtonRelease(Button b)
    {
        if (b == Button.X)
        {
            ChangeSidewaysWheelFriction(_defaultWheelFriction);
        }
    }
    #endregion
}
