using Core;
using UnityEngine;

public static class ProjMath
{ 
    public static float RotateTowardsPosition(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // [-180 => 180]
    }

    public static Vector2 MoveTowardsAngle(float angle)
    {
        return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public static float SinTime(float m = 1f, bool canBeNegative = false)
    {
        if (!canBeNegative) return Mathf.Sin(Time.timeSinceLevelLoad * m) * (Mathf.Sin(Time.timeSinceLevelLoad * m) > 0 ? 1f : -1f);
        return Mathf.Sin(Time.timeSinceLevelLoad * m);
    }

    public static bool RandomChance(float chance)
    {
        return Random.value < chance;
    }
    
    public static Vector2 MousePosition(InputSystem input)
    {
        if (!Camera.main)
        {
            Debug.LogError("No main camera found");
            return Vector2.zero;
        }

        Vector2 mousePos = input.UI.MousePosition.ReadValue<Vector2>();
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
    }
    
    /// <summary>
    /// For animations. Takes from 0 to 1 and returns from 0 to 1
    /// </summary>
    public static class EasingFunctions
    {
        /// <summary>
        /// Jumping func
        /// </summary>
        /// <param name="x">Current jump moment, has to be from 0 to x1</param>
        /// <param name="x1">Jump length</param>
        /// <param name="y1">Additional y coordinate for jump end</param>
        /// <param name="h">Peak of jump</param>
        /// <returns></returns>
        public static float JumpGraph(float x, float x1, float y1, float h)
        {
            return ((2 * y1 - 4 * h) / (x1 * x1)) * x * x +
                   ((4 * h - y1) / x1) * x;
        }
        
        public static float EaseOutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x - 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x - 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x - 2.625f / d1) * x + 0.984375f;
            }
        }

        public static float EaseInBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }

        public static float EaseOutQuint(float x)
        {
            return 1 - Mathf.Pow(1 - x, 5);
        }
    }
}
