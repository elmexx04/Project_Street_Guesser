using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamMovement : MonoBehaviour
{
    public GameObject splinePrefab;
    public SplineEditor selectedEditor;

    [Header("Inputs")]
    private Vector2 _mouseInput;
    private Vector2 _keyInput;
    private float _heightInput;
    private bool _lookInput;
    private bool _sprintInput;

    private Vector2 _cameraRot;
    private Vector3 _worldMousePos;

    public LayerMask selectedLayer;
    public LayerMask clickableLayers;
    private LayerMask raycastLayer;

    [Header("Movement")]
    [SerializeField] private float _mouseSense = 300f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _heightSpeed = 5f;

    [Header("Inputs")]
    [SerializeField] private InputActionProperty _moveButtons;
    [SerializeField] private InputActionProperty _lookButton;
    [Space]
    [SerializeField] private InputActionProperty _sprintButton;
    [SerializeField] private InputActionProperty _heightButton;

    [Header("Info")]
    public float lookValue;
    bool _mousePosSet;
    Vector2 _mousePos;
    bool _startLook;
    public Vector3 WorldMousePos
    {
        get => _worldMousePos;
    }

    void Start()
    {
        _cameraRot = transform.eulerAngles;
        raycastLayer = clickableLayers | selectedLayer;
    }

    void Update()
    {
        if (_lookInput)
        {
            if (_startLook)
            {
                CameraRotation();
            }
            _startLook = true;
        }
        else
        {
            _startLook = false;
        }
        CameraMovement();

        CheckPath();
    }

    private void CheckPath()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayer))
        {
            hit.transform.gameObject.layer = ToSingleLayer(selectedLayer);
            _worldMousePos = hit.point;

            if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1) && splinePrefab != null)
            {
                if (!hit.transform.TryGetComponent(out SplineEditor editor))
                {
                    Instantiate(splinePrefab, hit.point + new Vector3(0, 0.01f, 0), Quaternion.Euler(Vector3.zero));
                }
            }
            if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.transform.TryGetComponent(out SplineEditor editor))
                {
                    SwitchSelectedEditor(editor);
                }
                else
                {
                    if (selectedEditor != null)
                        selectedEditor.selected = false;
                    selectedEditor = null;
                }
            }
        }
    }

    private void CameraRotation()
    {
        _mouseInput.x = Input.GetAxis("Mouse X") * _mouseSense;
        _mouseInput.y = Input.GetAxis("Mouse Y") * _mouseSense;

        _cameraRot.x += -_mouseInput.y;
        _cameraRot.y += _mouseInput.x;

        _cameraRot.x = Mathf.Clamp(_cameraRot.x, -90, 90);

        transform.localRotation = Quaternion.Euler(_cameraRot.x, _cameraRot.y, 0);
    }

    private void CameraMovement()
    {
        float speed;
        if (!_sprintInput)
        {
            speed = _moveSpeed;
        }
        else
        {
            speed = _sprintSpeed;
        }
        transform.position += _keyInput.y * speed * Time.deltaTime * transform.forward;
        transform.position += _keyInput.x * speed * Time.deltaTime * transform.right;

        transform.position += new Vector3(0, _heightInput * _heightSpeed * Time.deltaTime, 0);
    }

    #region Inputs
    private void OnEnable()
    {
        _moveButtons.action.started += Move;
        _moveButtons.action.performed += Move;
        _moveButtons.action.canceled += Move;

        _sprintButton.action.started += Sprint;
        _sprintButton.action.canceled += Sprint;

        _heightButton.action.started += Height;
        _heightButton.action.canceled += Height;

        _lookButton.action.started += Look;
        _lookButton.action.canceled += Look;
    }

    private void OnDisable()
    {
        _moveButtons.action.started -= Move;
        _moveButtons.action.performed -= Move;
        _moveButtons.action.canceled -= Move;

        _sprintButton.action.started -= Sprint;
        _sprintButton.action.canceled -= Sprint;

        _heightButton.action.started -= Height;
        _heightButton.action.canceled -= Height;

        _lookButton.action.started -= Look;
        _lookButton.action.canceled -= Look;
    }

    private void Move(InputAction.CallbackContext context)
    {
        _keyInput = context.ReadValue<Vector2>();
    }
    private void Look(InputAction.CallbackContext context)
    {
        // Debug.Log(context.action.ReadValue<float>() > 0.1f ? "Pressed Look" : "Released Look");
        _lookInput = context.ReadValue<float>() > 0.1f;
        lookValue = context.ReadValue<float>();

        if (_lookInput && !_mousePosSet)
        {
            Cursor.visible = false;
            _mousePos = Input.mousePosition;
            _mousePosSet = true;
        }
        else if (!_lookInput && _mousePosSet)
        {
            Mouse.current.WarpCursorPosition(_mousePos);
            Cursor.visible = true;
            _mousePosSet = false;
        }
    }
    private void Height(InputAction.CallbackContext context)
    {
        _heightInput = context.ReadValue<float>();
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        _sprintInput = context.ReadValue<float>() > 0.1f;
    }
    #endregion


    public void SwitchSelectedEditor(SplineEditor newSelect)
    {
        if (selectedEditor == null)
        {
            selectedEditor = newSelect;
            selectedEditor.selected = true;
        }
        else if (newSelect == selectedEditor)
        {
            selectedEditor.selected = false;
            selectedEditor = null;
        }
        else
        {
            selectedEditor.selected = false;
            selectedEditor = newSelect;
            selectedEditor.selected = true;
        }
    }

    public int ToSingleLayer(LayerMask mask)
    {
        int value = mask.value;
        if (value == 0) return 0;  // Early out
        for (int l = 1; l < 32; l++)
            if ((value & (1 << l)) != 0) return l;  // Bitwise
        return -1;  // This line won't ever be reached but the compiler needs it
    }
}
