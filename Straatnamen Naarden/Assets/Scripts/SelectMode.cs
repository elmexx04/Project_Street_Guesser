using TMPro;
using UnityEngine;

public class SelectMode : MonoBehaviour
{
    public TextMeshProUGUI modeText;

    public int modeIndex;

    public Region currentRegion;

    public GameObject[] maps; // 0 = alles, 1 = zuid, etc.

    public void Start()
    {
        modeIndex = 0;
        UpdateMode();
    }

    public void NextMode()
    {
        modeIndex++;
        if (modeIndex >= maps.Length)
        {
            modeIndex = 0;
        }
        UpdateMode();
    }

    public void PreviousMode()
    {
        modeIndex--;
        if (modeIndex < 0)
        {
            modeIndex = maps.Length - 1;
        }
        UpdateMode();
    }

    void UpdateMode()
    {
        // Zet alle maps uit
        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].SetActive(false);
        }

        // Stel region + tekst in
        switch (modeIndex)
        {
            case 0:
                modeText.text = "Naarden alles";
                currentRegion = Region.GroteMap;
                break;

            case 1:
                modeText.text = "Naarden Zuid";
                currentRegion = Region.Zuid;
                break;

            case 2:
                modeText.text = "Naarden bedrijventerrein";
                currentRegion = Region.Bedrijventerrein;
                break;

            case 3:
                modeText.text = "Naarden Noord";
                currentRegion = Region.Noord;
                break;

            case 4:
                modeText.text = "Naarden Vesting";
                currentRegion = Region.Vesting;
                break;

            case 5:
                modeText.text = "Naarden Oost";
                currentRegion = Region.Oost;
                break;
        }
    }
    public void ActivateCurrentMap()
    {
        maps[modeIndex].SetActive(true);
    }
}