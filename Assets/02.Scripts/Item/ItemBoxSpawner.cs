using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxSpawner : MonoBehaviour
{
    [SerializeField] private ItemData[] itemDatas;
    [SerializeField] private GameObject itemObjectPrefab;
    private ItemObject spawnedItemObject;

    private void Start()
    {
        if(spawnedItemObject == null)
            Spawn();
    }

    private void Update()
    {
        if (!spawnedItemObject.gameObject.activeInHierarchy)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        if (itemDatas.Length == 0) return;

        int randomIndex = Random.Range(0, itemDatas.Length);
        spawnedItemObject = ObjectPoolingManager.Instance.Get(itemObjectPrefab, transform.position).GetComponent<ItemObject>();
        spawnedItemObject.ItemData = itemDatas[randomIndex];
    }




}
