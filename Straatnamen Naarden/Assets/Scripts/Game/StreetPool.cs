using UnityEngine;

public class StreetPool : MonoBehaviour
{
    public StreetQuizManager quizManager;
    public streetdata
    public int poolSize = 10;
    private List<StreetData> activePool = new List<StreetData>();
    private List<StreetData> remainingStreets = new List<StreetData>();

    void InitializePool(List<StreetData> validStreets)
    {
        activePool.Clear();
        remainingStreets = new List<StreetData>(validStreets);

        for (int i = 0; i < poolSize && remainingStreets.Count > 0; i++)
        {
            AddRandomStreetToPool();
        }
    }

    void AddRandomStreetToPool()
    {
        int index = Random.Range(0, remainingStreets.Count);

        activePool.Add(remainingStreets[index]);
        remainingStreets.RemoveAt(index);
    }

    public void ResetPool()
    {
        poolInitialized = false;
        activePool.Clear();
        remainingStreets.Clear();
    }
}
