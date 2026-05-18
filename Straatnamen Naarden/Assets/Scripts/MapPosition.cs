using UnityEngine;
using UnityEngine.Splines;

public enum Region
{
    GroteMap,
    Bedrijventerrein,
    Oost,
    Noord,
    Zuid,
    Vesting
}

[System.Serializable]
public class MapPosition
{
    public int score;
    public Spline position;
    public Region region;
}