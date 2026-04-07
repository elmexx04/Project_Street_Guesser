using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class StreetQuizManager : MonoBehaviour
{
    public StreetDatabase database;
    public SelectMode selectMode;

    public TextMeshProUGUI questionText;

    private StreetData currentStreet;
    private StreetData lastStreet;
    private bool poolInitialized = false;
    

    void Start()
    {
        LoadProgress();
    }

    public void SkipQuestion()
    {
        if (currentStreet == null) return;

        Vector2 correctPos = currentStreet.GetPositionForRegion(selectMode.currentRegion);

        wrongMarker.SetActive(true);
        wrongMarker.GetComponent<RectTransform>().anchoredPosition = correctPos;

        Debug.Log("Vraag geskipt: " + currentStreet.streetName);

        feedbackPanel.SetActive(true);
        feedbackText.text = "Overgeslagen!";

        mapClick.ResetClick();
        mapClick.canClick = false;

        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
        }

        currentStreet.positions[0].score += pointsForSkip;

        if (currentStreet.positions[0].score <= maxMinusScore)
        {
            currentStreet.positions[0].score = maxMinusScore;
        }


        Debug.Log("Score van straat " + currentStreet.streetName + ": " + currentStreet.positions[0].score);

        checkButton.SetActive(false);
        nextQuestionButton.SetActive(true);
        skipButton.SetActive(false);

        SaveProgress();
    }

    public void PickRandomStreet()
    {
        Region currentRegion = selectMode.currentRegion;
      
        List<StreetData> validStreets = new List<StreetData>();
        bool allLearned = true;

        foreach (var street in database.streets)
        {
            if (street.positions[0].score < scoreForNext)
            {
                allLearned = false;
                break;
            }
        }

        if (allLearned)
        {
            Debug.Log("Alles geleerd!");
            questionText.text = "Alles geleerd 🎉";

            return;
        }

        foreach (var street in database.streets)
        {
            foreach (var pos in street.positions)
            {
                if (pos.region == currentRegion)
                {
                    validStreets.Add(street);
                    break;
                }
            }
        }

        if (validStreets.Count == 0)
        {
            Debug.LogWarning("Geen straten gevonden voor regio: " + currentRegion);
            return;
        }

        if (!poolInitialized)
        {
            InitializePool(validStreets);
            poolInitialized = true;
        }

        if (activePool.Count == 1)
        {
            currentStreet = activePool[0];
        }
        else
        {
            do
            {
                currentStreet = activePool[Random.Range(0, activePool.Count)];
            }
            while (currentStreet == lastStreet);
        }

        lastStreet = currentStreet;

        feedbackPanel.SetActive(false);

        // Toon vraag
        questionText.text = "Waar ligt: " + currentStreet.streetName;

        Debug.Log("Gekozen straat: " + currentStreet.streetName);

        
        mapClick.ResetClick();
        mapClick.canClick = true;

        checkButton.SetActive(true);
        nextQuestionButton.SetActive(false);
        skipButton.SetActive(true);
        correctMarker.SetActive(false);
        wrongMarker.SetActive(false);
        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
        }
    }

    public StreetData GetCurrentStreet()
    {
        return currentStreet;
    }

    public MapClickHandler mapClick;
    public float guessDistance = 25f;
    public int pointsForCorrect = 1;
    public int pointsForWrong = -1;
    public int pointsForSkip = -1;

    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;

    public GameObject nextQuestionButton; // knop om volgende vraag te laden
    public GameObject checkButton; // knop om antwoord te bevestigen
    public GameObject skipButton; // knop om vraag over te slaan
    public GameObject correctMarker; // groen vinkje
    public GameObject wrongMarker;   // juiste locatie tonen

    public void SubmitGuess()
    {
        if (!mapClick.hasClicked)
        {
            Debug.Log("Nog niet geklikt!");
            return;
        }
        
        checkButton.SetActive(false);
        nextQuestionButton.SetActive(true);
        skipButton.SetActive(false);
        feedbackPanel.SetActive(true);

        Vector2 guess = mapClick.lastClickPosition;

        // juiste positie ophalen
        Vector2 correctPos = currentStreet.GetPositionForRegion(selectMode.currentRegion);

        float distance = Vector2.Distance(guess, correctPos);

        Debug.Log("Distance: " + distance);


        mapClick.canClick = false;

        if (distance <= guessDistance)
        {
            Debug.Log("Correct!");

            correctMarker.SetActive(true);
            correctMarker.GetComponent<RectTransform>().anchoredPosition = correctPos;
            currentStreet.positions[0].score += pointsForCorrect;

            if (currentStreet.positions[0].score >= scoreForNext)
            {
                activePool.Remove(currentStreet);

                if (remainingStreets.Count > 0)
                {
                    AddRandomStreetToPool();
                }
                Debug.Log("Straat geleerd: " + currentStreet.streetName);

                feedbackText.text = "Correct! Straat geleerd!";

                currentStreet.positions[0].score = scoreForNext;
            }
            else
            { 
                feedbackText.text = "Correct!";
            }
        }
        else
        {
            Debug.Log("Fout!");

            wrongMarker.SetActive(true);
            wrongMarker.GetComponent<RectTransform>().anchoredPosition = correctPos;

            DrawLine(guess, correctPos);

            Debug.Log("drawline van " + guess + " naar " + correctPos);

            feedbackText.text = "Fout! Je zat " + Mathf.RoundToInt(distance) + " units van de juiste locatie.";

            currentStreet.positions[0].score += pointsForWrong;

            if (currentStreet.positions[0].score <= maxMinusScore)
            {
                currentStreet.positions[0].score = maxMinusScore;
            }


        }

        Debug.Log("Score van straat " + currentStreet.streetName + ": " + currentStreet.positions[0].score);

        SaveProgress();
    }

    public RectTransform linePrefab;
    public Transform lineParent;

    private RectTransform currentLine;

    void DrawLine(Vector2 start, Vector2 end)
    {
        if (currentLine != null)
            Destroy(currentLine.gameObject);

        currentLine = Instantiate(linePrefab, lineParent);

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        currentLine.sizeDelta = new Vector2(distance, 5f);
        currentLine.anchoredPosition = start + direction / 2f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentLine.rotation = Quaternion.Euler(0, 0, angle);
    }
    public int poolSize = 10;
    public int scoreForNext = 3;
    public int maxMinusScore = -2;

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

    public void SaveProgress()
    {
        SaveData data = new SaveData();

        foreach (var street in database.streets)
        {
            data.streets.Add(new StreetSaveData
            {
                streetName = street.streetName,
                score = street.positions[0].score
            });
        }

        string json = JsonUtility.ToJson(data, true);

        string path = Application.persistentDataPath + "/save.json";

        File.WriteAllText(path, json);

        Debug.Log("Progress opgeslagen op: " + path);
    }

    public void LoadProgress()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (!File.Exists(path))
        {
            Debug.Log("Geen save bestand gevonden");
            return;
        }

        string json = File.ReadAllText(path);

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (var savedStreet in data.streets)
        {
            var street = database.streets.Find(s => s.streetName == savedStreet.streetName);

            if (street != null)
            {
                street.positions[0].score = savedStreet.score;
            }
        }

        Debug.Log("Progress geladen!");
    }
    public void ResetProgress()
    {
        foreach (var street in database.streets)
        {
            street.positions[0].score = 0;
        }

        poolInitialized = false;

        SaveProgress();

        Debug.Log("Progress gereset!");
    }
}