using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestSpawner : MonoBehaviour
{
    public SaveSystem saveSystem;

    public GameObject prefab;

    public List<GameObject> createdPrefabs = new List<GameObject>();

    public void Clear()
    {
        foreach (var item in createdPrefabs)
        {
            Destroy(item);
        }
        createdPrefabs.Clear();
    }

    public void SpawnPrefab()
    {
        var position = Random.insideUnitSphere * 5;
        createdPrefabs.Add(Instantiate(prefab, position, Quaternion.identity));
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        foreach (var item in createdPrefabs)
        {
            data.Add(item.transform.position);
        }
        var dataToSave = JsonUtility.ToJson(data);
        saveSystem.SaveData(dataToSave);
    }

    public void LoadGame()
    {

        Clear();
        string dataToLoad = "";
        dataToLoad = saveSystem.LoadData();
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            SaveData data = JsonUtility.FromJson<SaveData>(dataToLoad);
            foreach (var positionData in data.positionData)
            {
                createdPrefabs.Add(Instantiate(prefab, positionData.GetValue(), Quaternion.identity));
            }
        }
    }

    [Serializable]
    public class SaveData
    {
        public List<Vector3Serialization> positionData;

        public SaveData()
        {
            positionData = new List<Vector3Serialization>();
        }

        public void Add(Vector3 position)
        {
            positionData.Add(new Vector3Serialization(position));
        }
    }

    [Serializable]
    public class Vector3Serialization
    {
        public float x, y, z;

        public Vector3Serialization(Vector3 position)
        {
            this.x = position.x;
            this.y = position.y;
            this.z = position.z;
        }

        public Vector3 GetValue()
        {
            return new Vector3(x, y, z);
        }
    }
}
