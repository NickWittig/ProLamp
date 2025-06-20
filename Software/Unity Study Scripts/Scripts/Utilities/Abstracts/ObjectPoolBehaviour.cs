using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolBehaviour : MonoBehaviour
{
    private List<GameObject> pooledObjects;
    [SerializeField] protected GameObject objectToPool;
    [SerializeField] protected int amountToPool;
    private int activeObjectNumber = 0; 

    protected void Initialize()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (var i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool, this.transform);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }
    public GameObject GetNextObject()
    {
        for (var i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                activeObjectNumber ++;
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }
        return null;
    }

    public bool TryDeactiveLastObject()
    {
        if (activeObjectNumber <= 0) {
            pooledObjects[0].SetActive(false);
            activeObjectNumber = 0;
            return false;
        }
        pooledObjects[activeObjectNumber - 1].SetActive(false);
        activeObjectNumber--;
        return true;
    }

    public void DeactivateAllObjects()
    {
        while (TryDeactiveLastObject());
    }

    public int GetActiveObjectNumber()
    {
       return activeObjectNumber;
    }
}
