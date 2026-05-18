using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class StreetData
{
    public string streetName;
    public List<MapPosition> positions = new List<MapPosition>();

    public float GetDistanceFromRegion(Region region, Vector2 guess)
    {
        foreach (var pos in positions)
        {
            if (pos.region == region)
            {
                Vector2[] splinePoints = GetSplinePoints(pos.position);
                float minDistance = float.MaxValue;

                foreach (var point in splinePoints)
                {
                    float distance = Vector2.Distance(guess, point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }

                return minDistance;
            }
        }

        Debug.LogWarning("Geen positie gevonden voor region: " + region);
        return 999f;
    }

    private Vector2[] GetSplinePoints(Spline spline)
    {
        List<Vector2> points = new List<Vector2>();
        // for (int i = 0; i < spline.Count; i++)
        // {
        //     // points.Add(spline.GetPoint(i));
        // }
        return points.ToArray();
    }
}