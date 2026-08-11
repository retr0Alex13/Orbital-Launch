using System.Collections.Generic;
using UnityEngine;

public class TapPool : MonoBehaviour
{
    public static TapPool SharedInstance { get; private set; }

    [SerializeField]
    private int amountToPool;

    [SerializeField]
    private GameObject tapVFX;

    private List<GameObject> pooledObjects;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(tapVFX);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
