using UnityEngine;
using System.Collections;

public class CollidableVehicle : CollidableObject, IDamagable
{
    [SerializeField] private CollisionZone _colZone;
    [SerializeField] protected Health _health;

    protected override void PreCollisionCalculations(float force, Vector3 contactNormal)
    {
        float zoneDamageModifier = _colZone.GetZoneDamageFactor(contactNormal);
        float damageValue = force * zoneDamageModifier;
        int finalDamage = Mathf.RoundToInt(damageValue) * -1;

        AffectHealth(finalDamage);

        Debug.LogFormat("   {0}, damage: {1}", gameObject.name, finalDamage);
    }

    public void AffectHealth(int amount)
    {
        _health.AffectHealth(amount);
    }

    public bool IsDefeated()
    {
        return _health.IsDefeated;
    }
}
