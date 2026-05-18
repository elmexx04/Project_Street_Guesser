using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePoint : MonoBehaviour
{
    [SerializeField] private PointType _pointType;
    [SerializeField] private Vector3 _pointPos;
    [SerializeField] private Vector3 _pointRot;

    [Header("Control Point")]
    [SerializeField] private Vector3 _anchorPos;
    [SerializeField] private LineRenderer _line;

    [Header("Anchor Settings")]
    [SerializeField] private float _npcSpeed = 4;
    [SerializeField] private float _npcWaitTime;
    [SerializeField] private Vector3 _npcRotation;
    [SerializeField] private Transform _visualRotationTransform;

    /// <summary>
    /// Remembers if point is an anchor or controlpoint
    /// </summary>
    public PointType PointType
    {
        get => _pointType;
        set
        {
            _pointType = value;

            switch (_pointType)
            {
                case PointType.ANCHORPOINT:
                    {
                        name = "AnchorPoint";

                        _visualRotationTransform.gameObject.SetActive(true);

                        break;
                    }
                case PointType.CONTROLPOINT:
                    {
                        name = "ControlPoint";

                        _line.gameObject.SetActive(true);

                        break;
                    }
            }
        }
    }

    /// <summary>
    /// Rotation of points (to check if point was rotated)
    /// </summary>
    public Vector3 PointRot
    {
        get => _pointRot;
        set => _pointRot = value;
    }

    /// <summary>
    /// Scale of point object
    /// </summary>
    public float Scale
    {
        get => transform.localScale.x;
        set => transform.localScale = Vector3.one * value;
    }

    /// <summary>
    /// Color of point object
    /// </summary>
    public Color Color
    {
        get => GetComponent<MeshRenderer>().material.color;
        set => GetComponent<MeshRenderer>().material.color = value;
    }

    /// <summary>
    /// Anchor position for controlpoint
    /// </summary>
    public Vector3 AnchorPos
    {
        set
        {
            _anchorPos = value;
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, _anchorPos);
        }
    }

    /// <summary>
    /// Draws line between controlpoint and anchorpoint
    /// </summary>
    public LineRenderer Line
    {
        get => _line;
    }

    /// <summary>
    /// When this is an anchor, sets speed at which an NPC will travel from waypoint
    /// </summary>
    public float NpcSpeed
    {
        get => _npcSpeed;
    }

    /// <summary>
    /// When this is an anchor, time to wait at this waypoint before continuing
    /// </summary>
    public float NpcWaitTime
    {
        get => _npcWaitTime;
    }

    /// <summary>
    /// When this is an anchor and NPC is waiting, NPC will rotate to this rotation 
    /// </summary>
    public Vector3 NpcRotation
    {
        get => _npcRotation;
        set
        {
            _npcRotation = value;
        }
    }

    public Vector3 VisualRotation
    {
        get => _visualRotationTransform.localEulerAngles;
    }
}

/// <summary>
/// Type of this point (anchor or control)
/// </summary>
public enum PointType
{
    ANCHORPOINT,
    CONTROLPOINT,
}

