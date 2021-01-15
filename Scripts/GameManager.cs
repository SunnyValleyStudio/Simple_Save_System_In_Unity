using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;

    public UIController uiController;

    public StructureManager structureManager;

    public ObjectDetector objectDetector;

    public SaveSystem saveSystem;

    void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnBigStructurePlacement += BigStructurePlacement;
        inputManager.OnEscape += HandleEscape;
    }

    private void HandleEscape()
    {
        ClearInputActions();
        uiController.ResetButtonColor();
    }

    private void BigStructurePlacement()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceBigStructure, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceSpecial, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceHouse, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(roadManager.PlaceRoad, pos);
        };
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
        inputManager.OnMouseHold += (pos) =>
        {
            ProcessInputAndCall(roadManager.PlaceRoad, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void ClearInputActions()
    {
        inputManager.ClearEvents();
    }

    private void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        Vector3Int? result = objectDetector.RaycastGround(ray);
        if (result.HasValue)
            callback.Invoke(result.Value);
    }



    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
    }

    public void SaveGame()
    {
        SaveDataSerialization saveData = new SaveDataSerialization();
        foreach (var structureData in structureManager.GetAllStructures())
        {
            saveData.AddStructureData(structureData.Key, structureData.Value.BuildingPrefabIndex, structureData.Value.BuildingType);
        }
        var jsonFormat = JsonUtility.ToJson(saveData);
        Debug.Log(jsonFormat);
        saveSystem.SaveData(jsonFormat);
    }

    public void LoadGame()
    {
        var jsonFormatData = saveSystem.LoadData();
        if (String.IsNullOrEmpty(jsonFormatData))
            return;
        SaveDataSerialization saveData = JsonUtility.FromJson<SaveDataSerialization>(jsonFormatData);
        structureManager.ClearMap();
        foreach (var structureData in saveData.structuresData)
        {
            Vector3Int position = Vector3Int.RoundToInt(structureData.position.GetValue());
            if (structureData.buildingType == CellType.Road)
            {
                roadManager.PlaceRoad(position);
                roadManager.FinishPlacingRoad();
            }
            else
            {
                structureManager.PlaceLoadedStructure(position, structureData.buildingPrefabindex, structureData.buildingType);
            }
        }
    }
}
