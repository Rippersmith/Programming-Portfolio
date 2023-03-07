using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonGenerator : MonoBehaviour {
    public GameObject lastMoon;
    public Sprite[] moonOverlays;
    public Transform moonGenerationPoint;
    public GameObject speedUp, slowDown, detonator;
    public PlanetPooling moonPooler;

    // Update is called once per frame
    void Update() {
        if (lastMoon.transform.position.x < transform.position.x)
            SpawnNewMoon();

        RandomPlanetSpawn();
    }

    //TODO: need to add an OverlapCircle safeguard to prevent this from spawning over the RandomPlanetSpawn() planets
    void SpawnNewMoon()
    {
        int moonNum = Random.Range(0, 3);
        //spawn the moon
        GameObject newMoon = moonPooler.SpawnFromPool(moonNum, moonGenerationPoint.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))));

        //make sure that this moon is far enough from the last moon that was spawned/position it in the right spot
        float distance = (((lastMoon.transform.localScale.x * 2) + (newMoon.transform.localScale.x * 2)) * .1f) + .5f;
        newMoon.transform.position = lastMoon.transform.position + (Vector3)MoveMoonToNewPoint(lastMoon.transform.position, Random.Range(-80f + (distance / 2), 80f - (distance / 2)), distance);

        SpawnRandomItem(newMoon);
        lastMoon = newMoon;
    }

    //use this to get the angle the moon will be from the last moon, and the distance
    //this will happen if the moon overlaps with another existing moon
    Vector2 MoveMoonToNewPoint(Vector2 currPos, float degrees, float radius)
    {
        Vector2 randomCircle = new Vector2(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad));
        Vector2 newPos = randomCircle * radius;

        if (currPos.y + newPos.y > 4.5f)
            return MoveMoonToNewPoint(currPos, DecreaseAngle(degrees, radius), radius);
        else if (currPos.y + newPos.y < -4.5f)
            return MoveMoonToNewPoint(currPos, IncreaseAngle(degrees, radius), radius);

        return newPos;
    }

    //use this to decrease the angle the planet is at
    float DecreaseAngle(float degrees, float distance)
    {
        float newDegrees = degrees;
        newDegrees -= Random.Range(5f, 75f - distance);
        return newDegrees;
    }

    float IncreaseAngle(float degrees, float distance)
    {
        float newDegrees = degrees;
        newDegrees += Random.Range(5f, 75f - distance);
        return newDegrees;
    }

    void SpawnRandomItem(GameObject newMoon)
    {
        int randomItemChance = Random.Range(6, 21); //20% chance of an item
        //odds: 6: Speed Up   7: Slow Down   8: Instant Death
        if (randomItemChance >= 8)
        {       
            GameObject newItem = moonPooler.SpawnItem(randomItemChance, newMoon.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))), newMoon);
        }
    }

    void RandomPlanetSpawn()
    {
        Vector2 randomPos = new Vector2(moonGenerationPoint.position.x + 2f, Random.Range(-4.5f, 4.5f));
        float randomScale = Random.Range(8.5f, 26f);

        if (!Physics2D.OverlapCircle(randomPos, (randomScale * .2f)))
        {
            int moonNum = Random.Range(3, 6);

            //spawn the moon
            GameObject newRandomMoon = moonPooler.SpawnFromPool(moonNum, randomPos, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))));

            if (!Physics2D.OverlapCircle(randomPos, (randomScale * .2f)))
                newRandomMoon.SetActive(false);
            else
                SpawnRandomItem(newRandomMoon);
        }
    }
}
