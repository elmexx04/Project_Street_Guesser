using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
// Editor Namespace
public class SplineEditor : MonoBehaviour
{
    [Header("Path")]
    [SerializeField]
    private Spline _spline;
    public bool selected;

    [SerializeField] private List<Vector3> _points = new List<Vector3>();

    [Header("Points")]
    [SerializeField] private GameObject _pointPrefab;

    [SerializeField] private Vector3 _placementOffset = new Vector3(0, 0.01f, 0);
    [Range(0.01f, 2f)]
    [SerializeField] private float _anchorScale = 0.3f;
    [Range(0.01f, 2f)]
    [SerializeField] private float _controlScale = 0.1f;

    [SerializeField] private Color _anchorColor = Color.red;
    [SerializeField] private Color _controlColor = Color.green;
    [SerializeField] private Color _startAnchorColor = Color.cyan;

    [Header("Bezier Settings")]
    [SerializeField] private bool _isClosed;

    [SerializeField] private List<SplinePoint> _pointObjects = new List<SplinePoint>();


    [Header("Mesh Settings")]
    [SerializeField] private bool _visualizePath = true;

    [SerializeField] private Color _pathColor;

    [Range(0.05f, 1)]
    [Tooltip("Has to end on a 0 or 5")]
    [SerializeField] private float _spacing = 0.05f;

    [SerializeField] private float _roadWidth = 0.5f;

    [Range(0.01f, 1)]
    [SerializeField] private float _minVertsDst = 0.05f;
    [SerializeField] private float _tiling = 50;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;

    private bool _splineInitialized;

    /// <summary>
    /// Return the current SplineData
    /// </summary>
    //public Spline Spline { get => _spline; }

    /// <summary>
    /// Return spline looped state
    /// </summary>
    public bool IsClosed { get => _isClosed; }

    /// <summary>
    /// Returns List of SplinePoints
    /// </summary>
    public List<SplinePoint> PointObjects { get => _pointObjects; }

    /// <summary>
    /// Returns List of the Anchors
    /// </summary>
    public List<SplinePoint> AnchorObjects
    {
        get
        {
            List<SplinePoint> anchorObjects = new List<SplinePoint>();

            for (int i = 0; i < _pointObjects.Count; i++)
            {
                if (i % 3 == 0)
                {
                    anchorObjects.Add(_pointObjects[i]);
                }
            }

            return anchorObjects;
        }
    }
    /// <summary>
    /// Spacing between Points
    /// </summary>
    public float Spacing
    {
        get => _spacing;
        set
        {
            _spacing = value - value % 0.05f;
        }
    }

    private void Start()
    {
        FindAnyObjectByType<CamMovement>().SwitchSelectedEditor(this);
        // Now happens when SplineConnecor.Spline is set
        InitializeSpline();

        _meshRenderer.material.color = _pathColor;
    }

    private void Update()
    {
        if (_splineInitialized)
        {
            if (_isClosed != _spline.IsClosed)
            {
                ToggleClosed();
            }

            if (selected)
            {
                Inputs();
            }
            UpdatePoints();
        }
    }

    /// <summary>
    /// Initializes Spline
    /// </summary>
    public void InitializeSpline()
    {
        if (_spline)
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();

            SpawnPoints();
            _splineInitialized = true;
        }
        else
        {
            Debug.LogError("No Spline Found");
            enabled = false;
        }
    }

    /// <summary>
    /// Handles Spline Input
    /// </summary>
    private void Inputs()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Vector3 mousePos = Camera.main.GetComponent<CamMovement>().WorldMousePos;


            if (!_isClosed)
            {
                Debug.Log("Spawn");
                SpawnPoints(Input.mousePosition + _placementOffset);
            }
            else
            {
                Debug.Log("Split");
                SplitPoints(Input.mousePosition + _placementOffset);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out SplinePoint point))
                {
                    for (int i = 0; i < _pointObjects.Count; i++)
                    {
                        if (_pointObjects[i] == point)
                        {
                            DeletePoints(i);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawns the SplinePoints
    /// </summary>
    /// <param name="anchorPos">Position for new Point</param>
    private void SpawnPoints(Vector3? anchorPos = null)
    {
        if (_spline == null)
        {
            Debug.LogError("Spline was null");
            return;
        }

        if (anchorPos != null) _spline.AddSegment(anchorPos.Value);

        // Spawning points from the last index based on Splinepoints
        if (_spline.NumPoints > 0)
        {
            int num = _spline.NumPoints == 4 ? 4 : 3;

            for (int i = _spline.NumPoints - num; i < _spline.NumPoints; i++)
            {
                GameObject pointObject = Instantiate(_pointPrefab, _spline[i], Quaternion.identity, transform);

                if (pointObject.TryGetComponent(out SplinePoint point))
                {
                    point.PointType = (i % 3 == 0) ? PointType.ANCHORPOINT : PointType.CONTROLPOINT;
                    //point.PointPos = _spline[i];

                    // SPLITPOINTS WERKT NIET GOED HIERMEE
                    switch (point.PointType)
                    {
                        case PointType.ANCHORPOINT:
                            {
                                if (i == _spline.NumPoints - 1)
                                {
                                    point.transform.LookAt(_spline[i - 1], Vector3.up);
                                    point.transform.eulerAngles = new Vector3(point.transform.eulerAngles.x, point.transform.eulerAngles.y + 180, point.transform.eulerAngles.z);
                                }
                                else
                                {
                                    point.transform.LookAt(_spline[i + 1], Vector3.up);
                                }

                                point.PointRot = point.transform.eulerAngles;

                                point.Scale = _anchorScale;

                                if (i == 0)
                                {
                                    point.Color = _startAnchorColor;
                                }
                                else
                                {
                                    point.Color = _anchorColor;
                                }
                                break;
                            }
                        case PointType.CONTROLPOINT:
                            {
                                point.Scale = _controlScale;
                                point.Color = _controlColor;

                                if (i == _spline.NumPoints - 2 || (i + 1) % 3 == 0)
                                {
                                    point.AnchorPos = _spline[i + 1];
                                }
                                else if ((i - 1) % 3 == 0)
                                {
                                    point.AnchorPos = _spline[i - 1];
                                }

                                break;
                            }
                    }

                    _pointObjects.Add(point);
                }
            }

            GenerateMesh();
        }
        else
        {
            Debug.LogError("No Points Initialized On Spline");
        }

    }

    /// <summary>
    /// Split based on MousePos
    /// </summary>
    /// <param name="mousePos"></param>
    private void SplitPoints(Vector3 mousePos)
    {
        int selectedSegmentIndex = -1;
        float closetDst = float.MaxValue;

        for (int i = 0; i < _spline.NumSegments; i++)
        {
            Vector3[] points = _spline.GetPointsInSegment(i);

            float totalDst = Vector3.Distance(mousePos, points[0]) + Vector3.Distance(mousePos, points[3]);

            if (totalDst < closetDst)
            {
                closetDst = totalDst;
                selectedSegmentIndex = i;
            }
        }

        if (selectedSegmentIndex != -1)
        {
            _spline.SplitSegment(mousePos, selectedSegmentIndex);

            //_points.InsertRange(segementIndex * 3 + 2, new Vector3[] { Vector3.zero, anchorPos, Vector3.zero });

            for (int i = selectedSegmentIndex * 3 + 1; i < selectedSegmentIndex * 3 + 4; i++)
            {
                GameObject pointObject = Instantiate(_pointPrefab, _spline[i], Quaternion.identity, transform);

                if (pointObject.TryGetComponent<SplinePoint>(out SplinePoint point))
                {
                    point.PointType = (i % 3 == 0) ? PointType.ANCHORPOINT : PointType.CONTROLPOINT;

                    switch (point.PointType)
                    {
                        case PointType.ANCHORPOINT:
                            {
                                if (i == _spline.NumPoints - 1)
                                {
                                    point.transform.LookAt(_spline[i - 1], Vector3.up);
                                    point.transform.eulerAngles = new Vector3(point.transform.eulerAngles.x, point.transform.eulerAngles.y + 180, point.transform.eulerAngles.z);
                                }
                                else
                                {
                                    point.transform.LookAt(_spline[i + 1], Vector3.up);
                                }

                                point.PointRot = point.transform.eulerAngles;

                                point.Scale = _anchorScale;

                                if (i == 0)
                                {
                                    point.Color = _startAnchorColor;
                                }
                                else
                                {
                                    point.Color = _anchorColor;
                                }
                                break;
                            }
                        case PointType.CONTROLPOINT:
                            {
                                point.Scale = _controlScale;
                                point.Color = _controlColor;

                                if (i == _spline.NumPoints - 2 || (i + 1) % 3 == 0)
                                {
                                    point.AnchorPos = _spline[i + 1];
                                }
                                else if ((i - 1) % 3 == 0)
                                {
                                    point.AnchorPos = _spline[i - 1];
                                }

                                break;
                            }
                    }

                    _pointObjects.Insert(i, point);
                    point.transform.SetSiblingIndex(i);
                }
            }

            GenerateMesh();
        }
    }

    /// <summary>
    /// Toggle whether spline is looped
    /// </summary>
    private void ToggleClosed()
    {
        _spline.IsClosed = _isClosed;

        if (_isClosed)
        {
            for (int i = _spline.NumPoints - 2; i < _spline.NumPoints; i++)
            {
                GameObject pointObject = Instantiate(_pointPrefab, _spline[i], Quaternion.identity, transform);

                if (pointObject.TryGetComponent(out SplinePoint point))
                {
                    point.PointType = PointType.CONTROLPOINT;

                    point.Scale = _controlScale;
                    point.Color = _controlColor;

                    point.AnchorPos = i == _spline.NumPoints - 2 ? _spline[i - 1] : _spline[0];

                    _pointObjects.Add(point);
                }
            }
        }
        else
        {
            Destroy(_pointObjects[_spline.NumPoints].gameObject);
            Destroy(_pointObjects[_spline.NumPoints + 1].gameObject);

            _pointObjects.RemoveRange(_spline.NumPoints, 2);
        }

        GenerateMesh();
    }

    /// <summary>
    /// Deletes SplinePoint at AnchorIndex
    /// </summary>
    /// <param name="anchorIndex">Index of anchor to remove</param>
    private void DeletePoints(int anchorIndex)
    {
        if (_spline.NumPoints == 4) return;

        _spline.DeleteSegment(anchorIndex);

        if (_pointObjects.Count > 2 || !_spline.IsClosed && _pointObjects.Count > 1)
        {
            if (anchorIndex == 0)
            {
                if (_spline.IsClosed)
                {
                    _pointObjects[_pointObjects.Count - 1] = _pointObjects[2];
                }

                Destroy(_pointObjects[0].gameObject);
                Destroy(_pointObjects[1].gameObject);
                Destroy(_pointObjects[2].gameObject);

                _pointObjects.RemoveRange(0, 3);
            }
            else if (anchorIndex == _pointObjects.Count - 1 && !_spline.IsClosed)
            {
                Destroy(_pointObjects[anchorIndex - 2].gameObject);
                Destroy(_pointObjects[anchorIndex - 1].gameObject);
                Destroy(_pointObjects[anchorIndex].gameObject);

                _pointObjects.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                Destroy(_pointObjects[anchorIndex - 1].gameObject);
                Destroy(_pointObjects[anchorIndex].gameObject);
                Destroy(_pointObjects[anchorIndex + 1].gameObject);

                _pointObjects.RemoveRange(anchorIndex - 1, 3);
            }
        }
        GenerateMesh();
    }

    /// <summary>
    /// Checks if SplinePoints position or rotation change and update SplineData
    /// </summary>
    private void UpdatePoints()
    {
        for (int i = 0; i < _spline.NumPoints; i++)
        {
            // If AnchorPoint was rotated, updates ControlPoint Position
            if (i % 3 == 0 && _pointObjects[i].transform.eulerAngles != _pointObjects[i].PointRot)
            {
                if (i == _spline.NumPoints - 1)
                {
                    float dst = Vector3.Distance(_pointObjects[i].transform.position, _pointObjects[i - 1].transform.position);
                    Vector3 target = _pointObjects[i].transform.forward * dst;

                    _pointObjects[i - 1].transform.position = _pointObjects[i].transform.position - target;
                }
                else
                {
                    float dst = Vector3.Distance(_pointObjects[i].transform.position, _pointObjects[i + 1].transform.position);
                    Vector3 target = _pointObjects[i].transform.forward * dst;

                    _pointObjects[i + 1].transform.position = _pointObjects[i].transform.position + target;
                }

                _pointObjects[i].PointRot = _pointObjects[i].transform.eulerAngles;
                GenerateMesh();
            }

            // Updates SplinePoint position based on SplineData position
            if (_spline[i] != _pointObjects[i].transform.position)
            {
                _spline.MovePoint(i, _pointObjects[i].transform.position);

                for (int j = 0; j < _spline.NumPoints; j++)
                {
                    if (j % 3 != 0 && _pointObjects[j].transform.position != _spline[j])
                    {
                        _pointObjects[j].transform.position = _spline[j];
                    }
                }

                // SplinePoint is AnchorPoint
                if (i % 3 == 0)
                {
                    // If its last AnchorPoint, updates the last ControlPoint
                    if (i == _spline.NumPoints - 1)
                    {
                        _pointObjects[i - 1].AnchorPos = _pointObjects[i].transform.position;

                        if (_isClosed)
                        {
                            _pointObjects[_spline.NumPoints - 2].AnchorPos = _pointObjects[i].transform.position;
                        }
                    }
                    // If its first AnchorPoint, updates the first ControlPoint
                    else if (i == 0)
                    {
                        _pointObjects[i + 1].AnchorPos = _pointObjects[i].transform.position;

                        if (_isClosed)
                        {
                            _pointObjects[_spline.NumPoints - 1].AnchorPos = _pointObjects[i].transform.position;
                        }
                    }
                    // If all other AnchorPoints, updates both ControlPoints
                    else
                    {
                        _pointObjects[i + 1].AnchorPos = _pointObjects[i].transform.position;
                        _pointObjects[i - 1].AnchorPos = _pointObjects[i].transform.position;
                    }
                }
                // SplinePoint is ControlPoint
                else // Could be improved!
                {
                    // Updates AnchorRotation based on ControlPoint Position

                    // If not IsClosed or IsClosed and if its not the last AnchorPoint
                    if (!_isClosed || _isClosed && i < _spline.NumPoints - 2)
                    {
                        if (i == _spline.NumPoints - 2 || (i + 1) % 3 == 0)
                        {
                            _pointObjects[i + 1].transform.LookAt(_pointObjects[i].transform.position, Vector3.up);
                            _pointObjects[i + 1].transform.eulerAngles = new Vector3(_pointObjects[i + 1].transform.eulerAngles.x, _pointObjects[i + 1].transform.eulerAngles.y + 180, _pointObjects[i + 1].transform.eulerAngles.z);
                            _pointObjects[i + 1].PointRot = _pointObjects[i + 1].transform.eulerAngles;

                            _pointObjects[i].AnchorPos = _pointObjects[i + 1].transform.position;

                            if (i + 2 < _spline.NumPoints - 1)
                            {
                                _pointObjects[i + 2].AnchorPos = _pointObjects[i + 1].transform.position;
                            }

                        }
                        else if ((i - 1) % 3 == 0)
                        {
                            _pointObjects[i - 1].transform.LookAt(_pointObjects[i].transform.position, Vector3.up);
                            _pointObjects[i - 1].PointRot = _pointObjects[i - 1].transform.eulerAngles;

                            _pointObjects[i].AnchorPos = _pointObjects[i - 1].transform.position;

                            if (i - 2 > 0)
                            {
                                _pointObjects[i - 2].AnchorPos = _pointObjects[i - 1].transform.position;
                            }

                            if (IsClosed && i - 1 == 0)
                            {
                                _pointObjects[_spline.NumPoints - 1].AnchorPos = _pointObjects[0].transform.position;
                            }
                        }
                    }
                    else
                    {
                        // Takes the last two ControlPoints correlating to the first and the last AnchorPoint 

                        // Updates the last AnchorPoint
                        if (i == _spline.NumPoints - 2)
                        {
                            _pointObjects[i - 1].transform.LookAt(_pointObjects[i].transform.position, Vector3.up);
                            _pointObjects[i - 1].PointRot = _pointObjects[i - 1].transform.eulerAngles;

                            _pointObjects[i].AnchorPos = _pointObjects[i - 1].transform.position;
                            _pointObjects[i - 2].AnchorPos = _pointObjects[i - 1].transform.position;
                        }
                        // Updates the first AnchorPoint
                        else if (i == _spline.NumPoints - 1)
                        {
                            _pointObjects[0].transform.LookAt(_pointObjects[i].transform.position, Vector3.up);
                            _pointObjects[0].transform.eulerAngles = new Vector3(_pointObjects[0].transform.eulerAngles.x, _pointObjects[0].transform.eulerAngles.y + 180, _pointObjects[0].transform.eulerAngles.z);
                            _pointObjects[0].PointRot = _pointObjects[0].transform.eulerAngles;

                            _pointObjects[_spline.NumPoints - 1].AnchorPos = _pointObjects[0].transform.position;
                            _pointObjects[1].AnchorPos = _pointObjects[0].transform.position;
                        }
                    }
                }

                GenerateMesh();
            }

            // If NpcRotation was rotated
            if (i % 3 == 0 && _pointObjects[i].VisualRotation != _pointObjects[i].NpcRotation)
            {
                _pointObjects[i].NpcRotation = _pointObjects[i].VisualRotation;
            }

            float pointScale = (i % 3 == 0) ? _anchorScale : _controlScale;

            if (_pointObjects[i].Scale != pointScale)
            {
                _pointObjects[i].Scale = pointScale;
            }

            if (i % 3 == 0)
            {
                if (i == 0 && _pointObjects[i].Color != _startAnchorColor)
                {
                    _pointObjects[i].Color = _startAnchorColor;
                }
                else if (i > 0 && _pointObjects[i].Color != _anchorColor)
                {
                    _pointObjects[i].Color = _anchorColor;
                }
            }
            else
            {
                if (_pointObjects[i].Color != _controlColor)
                {
                    _pointObjects[i].Color = _controlColor;
                }
            }

        }
    }

    /// <summary>
    /// Generates the Spline Mesh
    /// </summary>
    private void GenerateMesh()
    {
        Vector3[] points = _spline.CalculateTimeBasedPoints(_spacing);

        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (points.Length - 1) + (_isClosed ? 2 : 0);

        int[] tris = new int[3 * numTris];

        int vertIndex = 0;
        int triIndex = 0;

        Vector3 dir = Vector3.zero;

        for (int i = 0; i < points.Length; i++)
        {
            if (i < points.Length - 1)
            {
                dir = points[i + 1] - points[i];

                Vector3 left = new Vector3(-dir.z, 0, dir.x).normalized;

                verts[vertIndex] = transform.InverseTransformPoint(points[i]) + left * _roadWidth / 2;
                verts[vertIndex + 1] = transform.InverseTransformPoint(points[i]) - left * _roadWidth / 2;
            }
            else
            {
                Vector3 left = new Vector3(-dir.z, 0, dir.x).normalized;

                verts[vertIndex] = transform.InverseTransformPoint(points[i]) + left * _roadWidth / 2;
                verts[vertIndex + 1] = transform.InverseTransformPoint(points[i]) - left * _roadWidth / 2;
            }

            if (i < points.Length - 1 || _isClosed)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }

            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);

            uvs[vertIndex] = new Vector2(0, v);
            uvs[vertIndex + 1] = new Vector2(1, v);

            normals[vertIndex] = Vector3.up;
            normals[vertIndex + 1] = Vector3.up;

            vertIndex += 2;
            triIndex += 6;
        }

        for (int i = 0; i < verts.Length; i += 2)
        {
            if (i < verts.Length - 3)
            {
                float vertsDstLeft = Vector3.Distance(verts[i], verts[i + 2]);

                if (vertsDstLeft <= _minVertsDst)
                {
                    Vector3 mergedVert = Vector3.Lerp(verts[i], verts[i + 2], 0.5f);
                    verts[i] = mergedVert;
                    verts[i + 2] = mergedVert;
                }

                float vertsDstRight = Vector3.Distance(verts[i + 1], verts[i + 3]);

                if (vertsDstRight <= _minVertsDst)
                {
                    Vector3 mergedVert = Vector3.Lerp(verts[i + 1], verts[i + 3], 0.5f);
                    verts[i + 1] = mergedVert;
                    verts[i + 3] = mergedVert;
                }
            }
        }

        int textureRepeat = Mathf.RoundToInt(_tiling * points.Length * _spacing * .05f);
        if (_meshRenderer.material == null)
        {
            Debug.LogError("SplineEditor Has No Material!");
        }
        else
        {
            _meshRenderer.sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
        }

        Mesh mesh = new Mesh
        {
            name = "Path Mesh",
            vertices = verts,
            triangles = tris,
            normals = normals,
            uv = uvs,
        };

        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Toggle whether the mesh is shown
    /// </summary>
    /// <param name="visualPath"></param>
    private void ToggleVisuals(bool visualPath)
    {
        for (int i = 0; i < _spline.NumPoints; i++)
        {
            _pointObjects[i].gameObject.SetActive(visualPath);
        }

        _meshRenderer.enabled = visualPath;
    }

    /// <summary>
    /// Toggle the ControlPoints
    /// </summary>
    /// <param name="isSelected"></param>
    private void SelectPath(bool isSelected)
    {
        for (int i = 0; i < _spline.NumPoints; i++)
        {
            if (i % 3 != 0)
            {
                _pointObjects[i].gameObject.SetActive(isSelected);
            }
        }
    }
}
