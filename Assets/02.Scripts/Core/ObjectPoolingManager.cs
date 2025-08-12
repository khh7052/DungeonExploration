using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();

    protected override void Initialize()
    {
        base.Initialize();
        poolDictionary = new();
        instanceToPrefab = new();
    }
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new();

        GameObject instanceObject = null;

        while (poolDictionary[prefab].Count > 0)
        {
            instanceObject = poolDictionary[prefab].Dequeue();
            if (instanceObject) break;
        }

        if (instanceObject == null)
            instanceObject = Instantiate(prefab);

        instanceObject.transform.SetPositionAndRotation(position, rotation);
        instanceObject.SetActive(true);

        instanceToPrefab[instanceObject] = prefab;
        return instanceObject;
    }

    public GameObject Get(GameObject prefab, Vector3 position) => Get(prefab, position, Quaternion.identity);

    public void Return(GameObject instance)
    {
        if (instance == null) return;

        if (instanceToPrefab.TryGetValue(instance, out GameObject prefab))
        {
            if (!poolDictionary.ContainsKey(prefab))
                poolDictionary[prefab] = new();

            // 중복 방지
            if (!poolDictionary[prefab].Contains(instance))
            {
                instance.SetActive(false);
                poolDictionary[prefab].Enqueue(instance);
            }
        }
        else
        {
            Destroy(instance);
        }
    }

}
