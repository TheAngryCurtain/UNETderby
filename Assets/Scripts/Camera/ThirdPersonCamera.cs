using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    private const float MIN_Y_ANGLE = 0f;
    private const float MAX_Y_ANGLE = 50f;

    [SerializeField] private Transform _target;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private Camera _camera;

    [SerializeField] private float _xSensitivity;
    [SerializeField] private float _ySensitivity;

    public Transform CameraTransform { get { return _camTransform; } }

    private bool _freeCam = false;
    private bool _shouldFollow = false;
    private float _currentX = 0f;
    private float _currentY = 0f;

    private Vector3 _defaultOffset;
    private float _defaultDistance;

    private void Awake()
    {
        _defaultOffset = _camTransform.position - _target.position;
        _defaultDistance = _defaultOffset.magnitude;

        // intial position
        _camTransform.position = _target.position + _defaultOffset;
    }

    public void HandleAxisInput(float h, float v)
    {
        if (h != 0f || v != 0f)
        {
            _currentX += h * _xSensitivity;
            _currentY += v * _ySensitivity;

            _currentY = Mathf.Clamp(_currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);

            _freeCam = true;
        }
    }

    public void UpdateFollowState(bool follow)
    {
        _shouldFollow = follow;
    }

    public void Recenter()
    {
        _currentX = 0f;
        _currentY = 0f;

        UpdateFollowOffset();
        
        _freeCam = false;
    }

    private void UpdateFollowOffset()
    {
        Vector3 adjustedCamForward = Vector3.ProjectOnPlane(_camTransform.forward, _target.up);
        float angle = Utils.GetSignedAngle(adjustedCamForward, _target.forward);

        _defaultOffset = Quaternion.Euler(0f, angle, 0f) * _defaultOffset;
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            if (_freeCam)
            {
                Quaternion rotation = Quaternion.Euler(_currentY, _currentX, _defaultDistance);
                _camTransform.position = _target.position + rotation * _defaultOffset;
                _camTransform.LookAt(_target.position);
            }
            else
            {
                if (_shouldFollow)
                {
                    UpdateFollowOffset();
                }

                _camTransform.position = _target.position + _defaultOffset;
                _camTransform.LookAt(_target);
            }
        }
    }
}
