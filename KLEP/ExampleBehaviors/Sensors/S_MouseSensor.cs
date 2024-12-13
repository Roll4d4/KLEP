using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_MouseSensor : KLEPExecutableBase
{
    public bool useRaycastPlane = false;
    public Vector3 planeVector = Vector3.zero;
    public string keyNameToPush;
    public string vec3PropertyName;
    public string transformPropertyName;
    public LayerMask layerMask; // Layer mask for raycasting
    public bool isKeyImmortal = true; // Should the key be pushed every update?
    public bool addRaycastHitToTransforms = true; // Should the raycast hit be added to Transforms?
    public LayerMask uiLayerMask;

    private KLEPKey immortalKeyCache = null;
    private bool isActive = true;
    private Camera mainCamera;

    // List to hold the mouse positions
    public List<Vector2> Positions { get; private set; } = new List<Vector2>();

    // List to hold the transforms (for future use)
    public List<Transform> Transforms { get; private set; } = new List<Transform>();

    public override void Init()
    {
        base.Init();
        mainCamera = Camera.main;
    }

    public override void ExecutableUpdates()
    {
        if (parentNeuron == null)
        {
            Debug.Log("null parent");
            return;
        }

        if (keyLoader == null)
        {
            Debug.Log("Keyloader null");
            return;
        }

        // Check if the mouse is over a UI element
        if (IsMouseOverUI())
        {
            if (isActive)
            {
                isActive = false;
                Debug.Log("Mouse is over UI, deactivating sensor.");
            }
            return;
        }
        else
        {
            if (!isActive)
            {
                isActive = true;
                Debug.Log("Mouse is no longer over UI, reactivating sensor.");
            }
        }

        if (isActive)
        {
            Vector2 mousePosition = Input.mousePosition;
            CreateAndPushMousePositionData(mousePosition);

            Vector3 mouseWorldPosition;
            Transform hitTransform = null;

            if (useRaycastPlane)
            {
                mouseWorldPosition = GetMouseWorldPositionOnPlane();
            }
            else
            {
                RaycastHit hit;
                mouseWorldPosition = GetMouseWorldPosition(out hit);
                if (hit.transform != null)
                {
                    hitTransform = hit.transform;
                }
            }

            if (isKeyImmortal)
            {
                if (immortalKeyCache == null)
                {
                    immortalKeyCache = parentNeuron.keyManager.GetKey(keyNameToPush);
                }

                if (immortalKeyCache != null)
                {
                    if (!string.IsNullOrEmpty(vec3PropertyName))
                    {
                        immortalKeyCache.SetProperty(vec3PropertyName, mouseWorldPosition);
                    }

                    if (!string.IsNullOrEmpty(transformPropertyName) && hitTransform != null)
                    {
                        immortalKeyCache.SetProperty(transformPropertyName, hitTransform);
                    }
                }
            }
            else
            {
                KeyCreationData keyData = KeyCreationService.CreateKeyData(keyNameToPush, 0f, keyLoader);

                if (!string.IsNullOrEmpty(vec3PropertyName))
                {
                    keyData.SetProperty(vec3PropertyName, mouseWorldPosition);
                }

                if (!string.IsNullOrEmpty(transformPropertyName) && hitTransform != null)
                {
                    keyData.SetProperty(transformPropertyName, hitTransform);
                }

                PushKey(keyData);
            }
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void CreateAndPushMousePositionData(Vector2 mousePosition)
    {
        // Add the current mouse position to the Positions list
        Positions.Add(mousePosition);

        // Create a KeyCreationData with the mouse position
        KeyCreationData creationData = KeyCreationService.CreateKeyData("Mouse_Position", 0f, keyLoader);

        // Check if the keyloader is available
        if (keyLoader != null)
        {
            creationData.SetProperty(PropertyNames.Positions.ToString(), Positions);
            creationData.SetProperty(PropertyNames.Transforms.ToString(), Transforms);
        }

        PushKey(creationData);
    }

    private Vector3 GetMouseWorldPositionOnPlane()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, planeVector);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;
    }

    private Vector3 GetMouseWorldPosition(out RaycastHit hit)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    public override bool IsComplete()
    {
        return true;
    }
}
