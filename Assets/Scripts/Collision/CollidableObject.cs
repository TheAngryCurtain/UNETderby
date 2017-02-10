using UnityEngine;
using System.Collections;
using System;

public class CollidableObject : MonoBehaviour
{
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] private Collider _preCollisionTrigger;

    protected float _preColVel;
    public float PreCollisionVelocity { get { return _preColVel; } }

    protected Vector3 _preColContactPos;
    public Vector3 PreCollisionContactPoint { get { return _preColContactPos; } }

    // each object controls the amount of force it exerts based on speed and mass
    // create a script for collidable Objects and, when they collide, calculate their force and pass it to the object you collided with
    // that way, objects are in charge of their own forces and can react based on the force exerted by another
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor")) return;

        CollidableObject cObj = other.gameObject.GetComponent<CollidableObject>();
        if (cObj != null)
        {
            Vector3 contactNormal = other.contacts[0].normal;
            Vector3 directionOfHit = (_preColContactPos - transform.position).normalized;
            float impactForce = cObj.GetCollisionForce(_rigidbody.mass);

            PreCollisionCalculations(impactForce, contactNormal);

            _rigidbody.AddForce((directionOfHit + Vector3.up) * impactForce, ForceMode.Impulse);
            Debug.LogFormat("{0} > recieved force: {1}", gameObject.name, impactForce);
        }
    }

    // On collision enter doesn't happen until after the physics updates are done in FixedUpdate
    // so collision info (like contact point, velocity, etc) have already changed from the collision by the time
    // that info is received. This trigger will update this objects velocity/potential contact point which can then be used in place of the collision info
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_preCollisionTrigger == null)
        {
            Debug.LogWarning("OnTriggerEnter for non-preCollision has happened!");
            return;
        }

        if (other.gameObject.layer != LayerMask.NameToLayer("Floor"))
        {
            _preColVel = _rigidbody.velocity.magnitude;
            _preColContactPos = other.transform.position;
        }
    }

    protected virtual void PreCollisionCalculations(float force, Vector3 contactNormal) { }

    public float GetCollisionForce(float otherMass)
    {
        float force = Utils.GetImpactForce(_rigidbody.mass, _preColVel);
        float massDiff = Mathf.Abs(_rigidbody.mass - otherMass);
        float modifier = _rigidbody.mass / massDiff;

        Debug.LogFormat("> ({0}, m: {1}, v: {2}, diff: {3}, modifier: {4}, force: {5}, modified force: {6})",
            this.gameObject.name, _rigidbody.mass, _preColVel, massDiff, modifier, force, force * modifier);

        return force * modifier;
    }
}
