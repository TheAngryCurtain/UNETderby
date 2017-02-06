using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    private const float MIN_Y_ANGLE = 0f;
    private const float MAX_Y_ANGLE = 50f;

    [SerializeField] private Transform _target;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private Camera _camera;

    [SerializeField] private float _distance;
    [SerializeField] private float _xSensitivity;
    [SerializeField] private float _ySensitivity;

    public Transform CameraTransform { get { return _camTransform; } }

    private float currentX = 0f;
    private float currentY = 0f;

    public void HandleAxisInput(float h, float v)
    {
        currentX += h * _xSensitivity;
        currentY += v * _ySensitivity;

        currentY = Mathf.Clamp(currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            Vector3 dir = new Vector3(0f, 0f, -_distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
            _camTransform.position = _target.position + rotation * dir;
            _camTransform.LookAt(_target.position);
        }
    }
}
