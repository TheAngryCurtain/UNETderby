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
    [SerializeField] protected Transform _centerOfMass;

    [SerializeField] protected float _driftWheelFriction;

    protected Player _driver;
    //protected Player[] _passengers;
    //protected int _maxPassengers;
    //protected int _passengerCount = 0;

    private Button _actionButton;
    private Transform _actionLocation;
    private Axle _currentAxle;

    private float _steerValue;
    private float _fowardThrottle;
    private float _reverseThrottle;
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
        _fowardThrottle = h;
    }

    protected virtual void Brake(float h, float v)
    {
        _reverseThrottle = h;
    }

    protected virtual void FixedUpdate()
    {
        float forwardMotor = _maxTorque * _fowardThrottle;
        float reverseMotor = _maxTorque * _reverseThrottle;
        float deltaMotor = forwardMotor - reverseMotor;

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
                _currentAxle.LeftWheelCollider.motorTorque = deltaMotor;
                _currentAxle.RightWheelCollider.motorTorque = deltaMotor;
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
            _currentAxle = _axles[1]; // just back wheels for now
            _currentAxle.LeftWheelCollider.sidewaysFriction = curve;
            _currentAxle.RightWheelCollider.sidewaysFriction = curve;
        }
    }

    #region IInteractable
    public void InteractAction(object parameters)
    {
        object[] param = (object[])parameters;
        Player p = (Player)param[0];
        InteractiveVehicleTrigger.InteractionVehicleInfo info = (InteractiveVehicleTrigger.InteractionVehicleInfo)param[1];
        _actionButton = info.ActionButton;
        _actionLocation = info.ActionLocation;

        bool canChangeState = p.TryChangeState(new CharacterState(CharacterState.eState.Driving));
        if (canChangeState)
        {
            //if (info.IsDriverDoor)
            //{
                _driver = p;
            //}
            //else
            //{
            //    _passengers[_passengerCount] = p;
            //    _passengerCount += 1;
            //}

            // align to seat
            _driver.transform.SetParent(this.transform);
            _driver.transform.position = info.ActiveSeat.position;
            _driver.transform.rotation = info.ActiveSeat.rotation;

            InputController.Instance.SetPlayerControllable(p.ControllerIndex, this);
        }
    }
    #endregion

    #region IControllable
    public virtual void HandleAxis(Axis a, float h, float v)
    {
        if (a == Axis.LStick)
        {
            // TODO need to check if you're the driving player

            // steering
            Steer(h, v);
        }
        else if (a == Axis.RStick)
        {
            // pass on camera
            if (_driver != null)
            {
                _driver.HandleAxis(a, h, v);
            }
        }
        else if (a == Axis.RTrigger)
        {
            // TODO need to check if you're the driving player

            // power
            Move(h, v);
        }
        else if (a == Axis.LTrigger)
        {
            // TODO need to check if you're the driving player

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
            bool canChangeState = _driver.TryChangeState(new CharacterState(CharacterState.eState.Default));
            if (canChangeState)
            {
                _driver.transform.position = _actionLocation.position + (_actionLocation.up * 0.5f);
                _driver.transform.rotation = _actionLocation.rotation;
                _driver.transform.SetParent(null);

                InputController.Instance.SetPlayerControllable(0, _driver);
                _driver = null;

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
