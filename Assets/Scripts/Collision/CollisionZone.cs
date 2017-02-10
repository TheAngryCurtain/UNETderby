using UnityEngine;
using System.Collections;

public class CollisionZone : MonoBehaviour
{
    public enum eZoneDurability { Strong, Regular, Weak };

    [SerializeField] private eZoneDurability _frontZone;
    [SerializeField] private eZoneDurability _sideZone;
    [SerializeField] private eZoneDurability _backZone;

    public float GetZoneDamageFactor(Vector3 contactNormal)
    {
        float dot = Vector3.Dot(transform.forward, contactNormal);
        int roundedDot = Mathf.RoundToInt(dot);

        float zoneFactor = 1f;

        // dot returns 1 = same, 0 = perpendicular, -1 = opposite
        // normal faces toward the hit (?) so they are reversed
        switch (roundedDot)
        {
            case -1:
                // hit from front

                Debug.Log("Front hit");
                if (_frontZone == eZoneDurability.Strong) zoneFactor = 0.5f;
                else if (_frontZone == eZoneDurability.Weak) zoneFactor = 2f;


                break;

            case 0:
                // hit from the side
                Debug.Log("Side hit");

                if (_sideZone == eZoneDurability.Strong) zoneFactor = 0.5f;
                else if (_sideZone == eZoneDurability.Weak) zoneFactor = 2f;

                break;

            case 1:
                // hit from behind
                Debug.Log("Behind hit");

                if (_backZone == eZoneDurability.Strong) zoneFactor = 0.5f;
                else if (_backZone == eZoneDurability.Weak) zoneFactor = 2f;

                break;
        }

        return zoneFactor;
    }
}
