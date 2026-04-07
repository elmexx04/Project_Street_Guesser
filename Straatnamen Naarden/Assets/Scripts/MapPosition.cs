using UnityEngine;

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
    public Vector2 position;
    public Region region;
}