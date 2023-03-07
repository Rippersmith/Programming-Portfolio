using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPooling : MonoBehaviour
{
    [System.Serializable]
    public class Pool {
        public int tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<int, Queue<GameObject>> moonPool;
    float randomScaleMin = 8.5f, randomScaleMax = 26f;

    // Start is called before the first frame update
    void Start()
    {
        moonPool = new Dictionary<int, Queue<GameObject>>(); 

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            moonPool.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(int poolTag, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (!moonPool.ContainsKey(poolTag))
        {
            return null;
        }

        GameObject objectToSpawn = moonPool[poolTag].Dequeue();
        MoonGravity moonScript = objectToSpawn.GetComponent<MoonGravity>();

        //"instantiate" the moon
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = spawnPosition;
        objectToSpawn.transform.rotation = spawnRotation;

        //randomize the size of the moon
        float randomScale = Random.Range(randomScaleMin, randomScaleMax);
        objectToSpawn.transform.localScale = Vector2.one * randomScale;

        //randomly change the colors of the 2 layers of the moon spawned
        moonScript.moonMain.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        moonScript.moonOverlay.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        //re-queue the moon
        moonPool[poolTag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public GameObject SpawnItem(int poolTag, Vector3 spawnPosition, Quaternion spawnRotation, GameObject parentMoon)
    {
        if (!moonPool.ContainsKey(poolTag))
        {
            return null;
        }

        //"instantiate" the item
        GameObject itemToSpawn = moonPool[poolTag].Dequeue();
        itemToSpawn.SetActive(true);

        //set rotation of the item
        itemToSpawn.transform.rotation = spawnRotation;

        //set the position of the item at the moon's center, then move it to the edge
        itemToSpawn.transform.position = spawnPosition + (itemToSpawn.transform.up * (parentMoon.gameObject.transform.localScale.x * .1f)) + (itemToSpawn.transform.up * (itemToSpawn.transform.localScale.y * .15f));

        //make the item a parent of the moon
        itemToSpawn.transform.parent = parentMoon.transform;

        //re-queue the item
        moonPool[poolTag].Enqueue(itemToSpawn);

        return itemToSpawn;
    }

}
