using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StreetData
{ 
    public string streetName;
    public List<MapPosition> positions = new List<MapPosition>();
    public Vector2 GetPositionForRegion(Region region)
    {
        foreach (var pos in positions)
        {
            if (pos.region == region)
                return pos.position;
        }

        Debug.LogWarning("Geen positie gevonden voor region: " + region);
        return Vector2.zero;
    }
}