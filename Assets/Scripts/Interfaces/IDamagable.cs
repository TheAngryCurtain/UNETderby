using UnityEngine;
using System.Collections;

public interface IDamagable
{
    void AffectHealth(int amount);
    bool IsDefeated();
}
