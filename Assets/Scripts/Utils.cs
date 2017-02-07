using UnityEngine;
using System.Collections;

public class Utils
{
    public static float GetSignedAngle(Vector3 v1, Vector3 v2)
    {
        float angle = Vector3.Angle(v1, v2);
        if (Vector3.Cross(v1, v2).y < 0f)
        {
            angle *= -1f;
        }

        return angle;
    }

    public static float GetSignedAngle(Vector2 v1, Vector2 v2)
    {
        float angle = Vector2.Angle(v1, v2);
        if (Vector3.Cross(v1, v2).z > 0f)
        {
            angle *= -1f;
        }

        return angle;
    }
}
