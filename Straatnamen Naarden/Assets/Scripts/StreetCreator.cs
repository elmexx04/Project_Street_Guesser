using UnityEngine;
using TMPro;

public class StreetCreator : MonoBehaviour
{
    public StreetDatabase database;
    public SelectMode selectMode;
    public MapClickHandler mapClick;

    public TMP_InputField streetNameInput;

    public void SaveStreet()
    {
        if (!mapClick.hasClicked)
        {
            Debug.Log("Klik eerst op de kaart!");
            return;
        }

        if (string.IsNullOrEmpty(streetNameInput.text))
        {
            Debug.Log("Voer een straatnaam in!");
            return;
        }

        string streetName = streetNameInput.text;
        Vector2 pos = mapClick.lastClickPosition;
        Region region = selectMode.currentRegion;

        // Check of straat al bestaat
        StreetData existingStreet = database.streets.Find(s => s.streetName == streetName);

        if (existingStreet != null)
        {
            // Voeg nieuwe map positie toe
            existingStreet.positions.Add(new MapPosition
            {
                region = region,
                position = pos
            });

            Debug.Log("Positie toegevoegd aan bestaande straat: " + streetName);
        }
        else
        {
            // Maak nieuwe straat
            StreetData newStreet = new StreetData();
            newStreet.streetName = streetName;

            newStreet.positions.Add(new MapPosition
            {
                region = region,
                position = pos
            });

            database.streets.Add(newStreet);

            Debug.Log("Nieuwe straat toegevoegd: " + streetName);
        }

        // reset input
        streetNameInput.text = "";

        // optional: reset klik
        mapClick.ResetClick();
    }
}