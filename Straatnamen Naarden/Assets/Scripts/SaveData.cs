using System.Collections.Generic;

[System.Serializable]
public class StreetSaveData
{
    public string streetName;
    public int score;
}

[System.Serializable]
public class SaveData
{
    public List<StreetSaveData> streets = new List<StreetSaveData>();
}