using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureModel : MonoBehaviour, INeedingRoad
{
    float yHeight = 0;

    public Vector3Int RoadPosition { get; set; }

    [field: SerializeField]
    public CellType BuildingType { get; private set; }
    [field: SerializeField]
    public int BuildingPrefabIndex { get; private set; }

    public void CreateModel(GameObject model, int buildingPrefabIndex, CellType buildingType)
    {
        var structure = Instantiate(model, transform);
        yHeight = structure.transform.position.y;
        BuildingType = buildingType;
        BuildingPrefabIndex = buildingPrefabIndex;
    }

    public void SwapModel(GameObject model, Quaternion rotation)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var structure = Instantiate(model, transform);
        structure.transform.localPosition = new Vector3(0, yHeight, 0);
        structure.transform.localRotation = rotation;
    }
}
