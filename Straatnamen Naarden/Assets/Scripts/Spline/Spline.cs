using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Spline : MonoBehaviour
{
    /// <summary>
    /// Points to calculate curve from (Anchors- and ControlPoints
    /// </summary>
    private List<Vector3> _points;

    [Header("Settings")]
    private bool _isClosed;
    private bool _autoSetControlPoints;

    /// <summary>
    /// Initialize Path
    /// </summary>
    /// <param name="centre"></param>
    public Spline(Transform centre)
    {
        _points = new List<Vector3>
        {
                centre.position +  -centre.right,
                centre.position + (-centre.right + centre.forward) * 0.5f,
                centre.position + (centre.right + -centre.forward) * 0.5f,
                centre.position + centre.right,
        };
    }

    /// <summary>
    /// Load Path
    /// </summary>
    /// <param name="points"></param>
    public Spline(List<Vector3> points)
    {
        this._points = points;
    }

    #region Getters

    /// <summary>
    /// Returns a point on spline at index i
    /// </summary>
    /// <param name="i">point index</param>
    /// <returns></returns>
    public Vector3 this[int i] => _points[i];

    /// <summary>
    /// Adds and removes points based on _isClosed value
    /// </summary>
    public bool IsClosed
    {
        get => _isClosed;
        set
        {
            if (_isClosed != value)
            {
                _isClosed = value;

                if (_isClosed)
                {
                    _points.Add(_points[_points.Count - 1] * 2 - _points[_points.Count - 2]);
                    _points.Add(_points[0] * 2 - _points[1]);

                    if (_autoSetControlPoints)
                    {
                        AutoSetAnchorControlPoints(0);
                        AutoSetAnchorControlPoints(_points.Count - 3);
                    }
                }
                else
                {
                    _points.RemoveRange(_points.Count - 2, 2);

                    if (_autoSetControlPoints)
                    {
                        AutoSetStartAndEndControls();
                    }
                }
            }
        }
    }

    /// <summary>
    /// When true, automatically sets position of controlpoints
    /// </summary>
    public bool AutoSetControlPoints
    {
        get
        {
            return _autoSetControlPoints;
        }
        set
        {
            if (_autoSetControlPoints != value)
            {
                _autoSetControlPoints = value;

                if (_autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    /// <summary>
    /// Number of points
    /// </summary>
    public int NumPoints
    {
        get
        {
            if (_points == null)
            {
                Debug.LogError("_points was null");
                return 0;
            }
            return _points.Count;
        }
    }




    /// <summary>
    ///  Num ber of segments (space between 2 anchors)
    /// </summary>
    public int NumSegments => _points.Count / 3;

    /// <summary>
    /// Converts a looped index to correct spline index (end of loop index goes back to beginning)
    /// </summary>
    /// <param name="i">Loop index to convert to spline index</param>
    /// <returns></returns>
    private int LoopIndex(int i)
    {
        return (i + _points.Count) % _points.Count;
    }

    #endregion

    #region Spline Changing

    /// <summary>
    /// Adds a segment on spline at anchorpPos (adds corresponding control points)
    /// </summary>
    /// <param name="anchorPos">Position to add new segment</param>
    public void AddSegment(Vector3 anchorPos)
    {
        _points.Add(_points[_points.Count - 1] * 2 - _points[_points.Count - 2]);
        _points.Add((_points[_points.Count - 1] + anchorPos) * 0.5f);
        _points.Add(anchorPos);

        if (_autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(_points.Count - 1);
        }
    }

    /// <summary>
    /// Adds a segment between 2 anchors
    /// </summary>
    /// <param name="anchorPos">Position to spil segment</param>
    /// <param name="segementIndex">Index to add segement</param>
    public void SplitSegment(Vector3 anchorPos, int segementIndex)
    {
        _points.InsertRange(segementIndex * 3 + 2, new Vector3[] { Vector3.zero, anchorPos, Vector3.zero });

        if (_autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(segementIndex * 3 + 3);
        }
        else
        {
            AutoSetAnchorControlPoints(segementIndex * 3 + 3);
        }
    }

    /// <summary>
    /// Removes a segment
    /// </summary>
    /// <param name="anchorIndex">Index to remove</param>
    public void DeleteSegment(int anchorIndex)
    {
        if (NumSegments > 2 || !_isClosed && NumSegments > 1)
        {
            if (anchorIndex == 0)
            {
                if (_isClosed)
                {
                    _points[_points.Count - 1] = _points[2];
                }

                _points.RemoveRange(0, 3);
            }
            else if (anchorIndex == _points.Count - 1 && !_isClosed)
            {
                _points.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                _points.RemoveRange(anchorIndex - 1, 3);
            }
        }
    }

    /// <summary>
    /// Get position of connected points
    /// </summary>
    /// <param name="i">Index of points</param>
    /// <returns>Position of connected points</returns>
    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[] { _points[i * 3], _points[i * 3 + 1], _points[i * 3 + 2], _points[LoopIndex(i * 3 + 3)] };
    }

    /// <summary>
    /// Move point i to pos
    /// </summary>
    /// <param name="i">Index of point to move</param>
    /// <param name="pos">Position to move point i to</param>
    public void MovePoint(int i, Vector3 pos)
    {
        Vector3 deltaMove = pos - _points[i];
        _points[i] = pos;

        if (_autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(i);
        }
        else
        {
            if (i % 3 == 0)
            {
                if (i + 1 < _points.Count || _isClosed)
                {
                    _points[LoopIndex(i + 1)] += deltaMove;
                }

                if (i - 1 > 0 || _isClosed)
                {
                    _points[LoopIndex(i - 1)] += deltaMove;
                }
            }
            else
            {
                bool nextPointIsAnchor = (i + 1) % 3 == 0;

                int correspondingControlIndex = nextPointIsAnchor ? i + 2 : i - 2;
                int anchorIndex = nextPointIsAnchor ? i + 1 : i - 1;

                if (correspondingControlIndex >= 0 && correspondingControlIndex < _points.Count || _isClosed)
                {
                    float dist = (_points[LoopIndex(anchorIndex)] - _points[LoopIndex(correspondingControlIndex)]).magnitude;
                    Vector3 dir = (_points[LoopIndex(anchorIndex)] - pos).normalized;

                    _points[LoopIndex(correspondingControlIndex)] = _points[LoopIndex(anchorIndex)] + dir * dist;
                }
            }
        }
    }

    /// <summary>
    /// Calculate Spline with points evenly spaced
    /// </summary>
    /// <param name="spacing">Spacing between points</param>
    /// <param name="resolution">Quality of points</param>
    /// <returns>Evenly spaced positions on spline</returns>
    public Vector3[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
    {
        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(_points[0]);
        Vector3 previousPoint = _points[0];
        float dstSinceLastEvenPoint = 0;

        for (int i = 0; i < NumSegments; i++)
        {
            Vector3[] p = GetPointsInSegment(i);

            float controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
            float estimatedCureveLength = Vector3.Distance(p[0], p[3]) + controlNetLength / 2;
            int divisions = Mathf.CeilToInt(estimatedCureveLength * resolution * 10);

            float t = 0;

            while (t <= 1)
            {
                t += 1f / divisions;

                Vector3 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                dstSinceLastEvenPoint += Vector3.Distance(previousPoint, pointOnCurve);

                while (dstSinceLastEvenPoint >= spacing)
                {
                    float overshootDst = dstSinceLastEvenPoint - spacing;
                    Vector3 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;

                    evenlySpacedPoints.Add(newEvenlySpacedPoint);

                    dstSinceLastEvenPoint = overshootDst;
                    previousPoint = newEvenlySpacedPoint;
                }

                previousPoint = pointOnCurve;
            }
        }

        return evenlySpacedPoints.ToArray();
    }

    /// <summary>
    /// Calculate points on spline every spacing seconds
    /// </summary>
    /// <param name="spacing">Time between points</param>
    /// <returns>Time spaced positions on spline</returns>
    public Vector3[] CalculateTimeBasedPoints(float spacing)
    {
        List<Vector3> timeSpacedPoints = new List<Vector3>();

        for (int i = 0; i < NumSegments; i++)
        {
            Vector3[] p = GetPointsInSegment(i);

            decimal t = 0;

            while (t <= 1)
            {
                Vector3 newTimeSpacedPoint = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], (float)t);

                if (timeSpacedPoints.Count == 0 || timeSpacedPoints[timeSpacedPoints.Count - 1] != newTimeSpacedPoint)
                {
                    timeSpacedPoints.Add(newTimeSpacedPoint);
                }

                t += (decimal)spacing;
            }
        }

        return timeSpacedPoints.ToArray();
    }

    /// <summary>
    /// Updates position of effected controlpoints of moved anchor automatically
    /// </summary>
    /// <param name="updatedAnchorIndex">Index of acnhor moved</param>
    private void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
    {
        for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i += 3)
        {
            if (i >= 0 && i < _points.Count || _isClosed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    /// <summary>
    /// Automatically updates position of all controlpoints
    /// </summary>
    private void AutoSetAllControlPoints()
    {
        for (int i = 0; i < _points.Count; i += 3)
        {
            AutoSetAnchorControlPoints(i);
        }

        AutoSetStartAndEndControls();
    }

    /// <summary>
    /// Update postion of controlpoints connected to anchor at anchorIndex
    /// </summary>
    /// <param name="anchorIndex">Index of affected anchor</param>
    private void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector3 anchorPos = _points[anchorIndex];
        Vector3 dir = Vector3.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0 || _isClosed)
        {
            Vector3 offset = _points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }

        if (anchorIndex + 3 >= 0 || _isClosed)
        {
            Vector3 offset = _points[LoopIndex(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }

        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;

            if (controlIndex >= 0 && controlIndex < _points.Count || _isClosed)
            {
                _points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * 0.5f;
            }
        }
    }

    /// <summary>
    /// Automatically sets position of first and last controlpoints
    /// </summary>
    private void AutoSetStartAndEndControls()
    {
        if (!_isClosed)
        {
            _points[1] = (_points[0] + _points[2]) * 0.5f;
            _points[_points.Count - 2] = _points[_points.Count - 1] + _points[_points.Count - 3] * 0.5f;
        }
    }

    #endregion

    private void Start()
    {
        Transform centre = transform;
        _points = new List<Vector3>
        {
            centre.position +  -centre.right,
            centre.position + (-centre.right + centre.forward) * 0.5f,
            centre.position + (centre.right + -centre.forward) * 0.5f,
            centre.position + centre.right,
        };
    }



}