using System.Collections.Generic;
using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    public List<StreetProgress> progressList = new List<StreetProgress>();

    public int GetScore(string streetName, Region region)
    {
        var data = progressList.Find(p => p.streetName == streetName && p.region == region);
        return data != null ? data.score : 0;
    }

    public void AddScore(string streetName, Region region, int amount, int min, int max)
    {
        var data = progressList.Find(p => p.streetName == streetName && p.region == region);

        if (data == null)
        {
            data = new StreetProgress
            {
                streetName = streetName,
                region = region,
                score = 0
            };
            progressList.Add(data);
        }

        data.score += amount;
        data.score = Mathf.Clamp(data.score, min, max);
    }
}