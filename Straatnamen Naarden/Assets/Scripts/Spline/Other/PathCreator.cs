using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [HideInInspector] public Spline path;

    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;

    [Space]

    public float anchorDiameter = .1f;
    public float controlDiameter = .075f;

    [Space]

    public bool displayControlPoints = true;

    public void CreatePath()
    {
        path = new Spline(transform);
    }

    void Reset()
    {
        CreatePath();
    }
}
