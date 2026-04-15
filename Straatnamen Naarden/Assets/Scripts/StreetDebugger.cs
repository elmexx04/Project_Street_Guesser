using UnityEngine;

public class StreetDebugger : MonoBehaviour
{
    public StreetDatabase database;
    public SelectMode selectMode;

    public GameObject markerPrefab;
    public Transform markerParent; // onder je Canvas

    public void ShowAllStreets()
    {
        ClearMarkers();

        Region currentRegion = selectMode.currentRegion;

        foreach (var street in database.streets)
        {
            foreach (var pos in street.positions)
            {
                if (pos.region == currentRegion)
                {
                    GameObject marker = Instantiate(markerPrefab, markerParent);

                    RectTransform rt = marker.GetComponent<RectTransform>();
                    rt.anchoredPosition = pos.position;

                    marker.name = street.streetName;

                    break;
                }
            }
        }

        Debug.Log("Alle straten weergegeven voor: " + currentRegion);
    }

    public void ClearMarkers()
    {
        foreach (Transform child in markerParent)
        {
            Destroy(child.gameObject);
        }
    }
}