using UnityEngine;
using System.Collections;

public class ZoneCollision : MonoBehaviour, IDamagable
{
    public enum eZoneDurability { Strong, Regular, Weak };

    [SerializeField] private Health _health;

    [SerializeField] private eZoneDurability _frontZone;
    [SerializeField] private eZoneDurability _sideZone;
    [SerializeField] private eZoneDurability _backZone;

    protected virtual void OnCollisionEnter(Collision other)
    {
        // TODO
        // need to consider 2 situations:
        // 1) you crash into something
        // 2) something crashes into you
        // the below situation works if soemthing else crashes into you, but not if you crash into something stationary (or close to it)

        Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>();
        if (otherBody != null)
        {
            Vector3 contactNormal = other.contacts[0].normal;
            int dot = (int)Vector3.Dot(transform.forward, contactNormal);
            float mass = otherBody.mass;
            float speed = otherBody.velocity.magnitude;
            float zoneDamageMultiplier = 1f;

            switch (dot)
            {
                case -1:
                    // hit from behind
                    if (_backZone == eZoneDurability.Strong) zoneDamageMultiplier = 0.5f;
                    else if (_backZone == eZoneDurability.Weak) zoneDamageMultiplier = 2f;

                    break;

                case 0:
                    // hit from the side
                    if (_sideZone == eZoneDurability.Strong) zoneDamageMultiplier = 0.5f;
                    else if (_sideZone == eZoneDurability.Weak) zoneDamageMultiplier = 2f;

                    break;

                case 1:
                    // hit head on
                    if (_frontZone == eZoneDurability.Strong) zoneDamageMultiplier = 0.5f;
                    else if (_frontZone == eZoneDurability.Weak) zoneDamageMultiplier = 2f;

                    break;
            }

            // find out how hard you were hit
            float force = Utils.GetImpactForce(mass, speed);
        }
    }

    #region IDamagable
    public void AffectHealth(int amount)
    {
        _health.AffectHealth(amount);
    }

    public bool IsDefeated()
    {
        return _health.IsDefeated;
    }
    #endregion
}
