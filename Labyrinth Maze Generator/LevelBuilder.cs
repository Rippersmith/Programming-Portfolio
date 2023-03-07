using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

public class LevelBuilder : MonoBehaviour
{
    public GameObject test;
    public Room startRoomPrefab;
    public Room endRoomPrefab;

    public GameObject[] decalPrefabs = new GameObject[2];

    public List<Room> roomPrefabs = new List<Room>();

    public GameObject player;
    public GameObject monster;
    public int monstersToSpawn = 2;

    public Vector2 iterationRange = new Vector2(7, 10);

    List<Doorway> availableDoorways = new List<Doorway>();

    StartRoom startRoom;
    EndRoom endRoom;
    List<Room> placedRooms = new List<Room>();

    LayerMask roomLayerMask;

    List<GameObject> rooms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        roomLayerMask = LayerMask.GetMask("Room");
        StartCoroutine("LevelGenerator");
    }

    IEnumerator LevelGenerator()
    {
        WaitForSeconds startup = new WaitForSeconds(1);
        WaitForFixedUpdate interval = new WaitForFixedUpdate();

        //yield return startup;

        //place start room
        PlaceStartRoom();
        //NavMeshSurface nav = rooms[0].GetComponent<NavMeshSurface>();
        yield return interval;

        //Random iterations
        int iterations = Random.Range((int)iterationRange.x, (int)iterationRange.y);

        for (int i = 0; i < iterations; i++)
        {
            //place random room from list
            PlaceRoom();
            yield return interval;
        }
        DeleteDoorwaysForJunctions();

        //place end room
        PlaceEndRoom();
        yield return interval;

        foreach (Room room in placedRooms)
        {
            for (int i = 0; i < 4; i++)
            {
                //use CreateDecal() AFTER generation to prevent weird cut-off decals
                int decalNum = Random.Range(0, decalPrefabs.Length);
                CreateDecal(room.gameObject.transform, decalPrefabs[decalNum]);
            }
        }

        NavMeshSurface nav = rooms[0].GetComponent<NavMeshSurface>();
        nav.BuildNavMesh();
        //}

        Instantiate(player, startRoom.transform.position, Quaternion.identity);

        SpawnMonsters();

        //end level generation
        Debug.Log("Success!!");
        //yield return new WaitForSeconds(1);
        //ResetLevelGenerator();
    }

    void PlaceStartRoom()
    {
        //instantiate room
        startRoom = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity) as StartRoom;
        startRoom.transform.parent = this.transform;

        rooms.Add(startRoom.gameObject);

        //get doorways from start room & add them randomly to the list of avaioable doorways
        AddDoorwaysToList(startRoom, ref availableDoorways);
    }

    void AddDoorwaysToList(Room room, ref List<Doorway> list)
    {
        list.AddRange(room.doorways);
        /*foreach (Doorway doorway in room.doorways)
        {
            int r = Random.Range(0, list.Count);
            list.Insert(r, doorway);
        }*/
    }

    void PlaceRoom()
    {
        //instantiate room
        Room currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], Vector3.zero, Quaternion.identity) as Room;

        rooms.Add(currentRoom.gameObject);

        List<Doorway> allAvailableDoorways = new List<Doorway>(availableDoorways);
        List<Doorway> currentRoomDoorways = new List<Doorway>();

        AddDoorwaysToList(currentRoom, ref currentRoomDoorways);

        //get doorways from current rrom & add new doorways to list of available doorways
        AddDoorwaysToList(currentRoom, ref availableDoorways);
        bool roomPlaced = false;

        //try all available doorways
        foreach (Doorway availableDoorway in allAvailableDoorways)
        {
            //try all available doorways in current room
            //foreach (Doorway currentDoorway in currentRoomDoorways)
            //{
            Doorway currentDoorway = currentRoomDoorways[Random.Range(0, currentRoomDoorways.Count)];
            //Position room
            PositionRoomAtDoorway(ref currentRoom, currentDoorway, availableDoorway);

            //check room overlap
            if (CheckRoomOverlap(currentRoom.transform))
            {
                continue;
            }

            roomPlaced = true;

            //use CreateDecal() AFTER generation to prevent weird cut-off decals
            //CreateDecal(currentRoom.gameObject.transform);

            //add room to list
            placedRooms.Add(currentRoom);

            //remove accepted doorways
            //currentDoorway.gameObject.SetActive(false);
            availableDoorways.Remove(currentDoorway);
            Destroy(currentDoorway.gameObject);
            //availableDoorway.gameObject.SetActive(false);
            availableDoorways.Remove(availableDoorway);
            Destroy(availableDoorway.gameObject);

            //exit loop
            break;
            //}
            //check if room has been placed
            if (roomPlaced)
            {
                break;
            }
        }

        //room can't be placed. Restart generator & try again
        if (!roomPlaced)
        {
            Debug.Log("Error! Room can't be placed. Starting over...");
            Destroy(currentRoom.gameObject);
            ResetLevelGenerator();
        }
    }

    void PlaceEndRoom()
    {
        //instantiate room
        endRoom = Instantiate(endRoomPrefab) as EndRoom;
        endRoom.transform.parent = this.transform;

        //create doorway lists to loop over
        List<Doorway> allAvailableDoorways = new List<Doorway>(availableDoorways);
        Doorway doorway = endRoom.doorways[0];

        bool roomPlaced = false;

        //try all available doorways
        foreach (Doorway availableDoorway in allAvailableDoorways)
        {
            Room room = (Room)endRoom;

            //Position room
            PositionRoomAtDoorway(ref room, doorway, availableDoorway);

            //check room overlap
            if (CheckRoomOverlap(endRoom.transform))
            {
                continue;
            }

            roomPlaced = true;

            //remove accepted doorways
            doorway.gameObject.SetActive(false);
            availableDoorways.Remove(doorway);
            availableDoorway.gameObject.SetActive(false);
            availableDoorways.Remove(availableDoorway);

            //exit loop
            break;
        }

        //room can't be placed. Restart generator & try again
        if (!roomPlaced)
        {
            Debug.Log("Error! Room can't be placed. Starting over...");
            ResetLevelGenerator();
        }
    }

    void DeleteDoorwaysForJunctions()
    {
        foreach(Doorway door in availableDoorways)
        {
            Collider[] doorColliders = Physics.OverlapBox(door.transform.position, Vector3.one , Quaternion.identity);
            if (doorColliders.Length == 2)
            {
                Destroy(doorColliders[0].gameObject);
                Destroy(doorColliders[1].gameObject);
            }
        }
    }

    void PositionRoomAtDoorway(ref Room room, Doorway roomDoorway, Doorway targetDoorway)
    {
        //reset room position & rotation
        room.transform.position = Vector3.zero;
        room.transform.rotation = Quaternion.identity;

        //rotate room to match previous doorway orientation
        Vector3 targetDoorwayEuler = targetDoorway.transform.eulerAngles;
        Vector3 roomDoorwayEuler = roomDoorway.transform.eulerAngles;
        float delta = Mathf.DeltaAngle(roomDoorwayEuler.y, targetDoorwayEuler.y);
        Quaternion currentRoomTargetRotation = Quaternion.AngleAxis(delta, Vector3.up);
        room.transform.rotation = currentRoomTargetRotation * Quaternion.Euler(0, 180, 0);

        //position room
        Vector3 roomPositionOffset = roomDoorway.transform.position;
        room.transform.position = targetDoorway.transform.position - roomPositionOffset;
    }

    //TODO: try and change this so that you check for a room BEFORE you plop a new room down.
    bool CheckRoomOverlap (Transform room)
    {

        Collider[] colliders = Physics.OverlapBox(room.position, new Vector3 (1, 3f, 1)/*Vector3.one * 2f*/, room.rotation, roomLayerMask);
        if (colliders.Length > 0)
        {
            //Debug.LogError("Overlap detected");
            return true;

        }

        return false;
    }

    //TODO: fill this out a LOT more!!!
    void CreateDecal(Transform newRoom, GameObject decalPrefab)
    {
        if (newRoom.gameObject.name != "Slope(Clone)")
        {
            //int easyRange1, easyRange2, easyRange3;
            //rotation adjustment
            //if (newRoom.gameObject.name != "Slope(Clone)"){
            //int easyRange1 = Random.Range(-3, 4) * 45;
            //int easyRange2 = Random.Range(-3, 4) * 45;
            //int easyRange3 = Random.Range(-3, 4) * 45; 

            int easyRange1 = Random.Range(-180, 180);
            int easyRange2 = Random.Range(-180, 180);
            int easyRange3 = Random.Range(-180, 180);

            //}else{
            //easyRange1 = (Random.Range(-2, 1) * 90) + 63;
            //easyRange2 = (Random.Range(-2, 1) * 90) + 63;
            //easyRange3 = (Random.Range(-2, 1) * 90) + 63;
            //}
            //Quaternion quat = Quaternion.Euler(easyRange1, easyRange2, easyRange3);
            Vector3 dir = new Vector3(easyRange1, easyRange2, easyRange3);
            dir = dir.normalized;

            RaycastHit hit;

            LayerMask wallLayer = LayerMask.NameToLayer("Room");
            wallLayer = ~wallLayer;
            bool hitCheck = Physics.Raycast(newRoom.position, dir, out hit, 4f, wallLayer, QueryTriggerInteraction.Ignore);

            if (hitCheck && hit.collider.gameObject.tag != "DecalSensor") {
                Quaternion newQuat = Quaternion.LookRotation(dir, Vector3.up);

                GameObject newDecal = Instantiate(decalPrefab, hit.point, newQuat);
                newDecal.transform.Rotate(0, 0, Random.Range(0, 360));

                float newSize = Random.Range(1f, 3.2f);
                newDecal.GetComponentInChildren<DecalProjector>().size = new Vector3(newSize, newSize, newSize + .5f);

                newDecal.transform.parent = newRoom;
            }

        }
    }

    //TODO: make sure 2 monster can't spawn in the same room
    void SpawnMonsters()
    {
        GameObject[] lastRooms = new GameObject[15];
        int counter = placedRooms.Count - 20;

        for (int i = 0; i < 15; i++)
        {
            lastRooms[i] = placedRooms[counter + i].gameObject;
        }

        for (int j = 0; j < monstersToSpawn; j++)
        {
            GameObject newMonster = Instantiate(monster, lastRooms[Random.Range(0, 14)].transform.position, Quaternion.identity);
            newMonster.GetComponent<MonsterBase>().AddWaypoints(placedRooms);
        }
    }

    void ResetLevelGenerator()
    {
        StopCoroutine("LevelGenerator");

        if (startRoom)
        {
            Destroy(startRoom.gameObject);
        }

        if (endRoom)
        {
            Destroy(endRoom.gameObject);
        }

        foreach (Room room in placedRooms)
        {
            Destroy(room.gameObject);
        }

        placedRooms.Clear();
        availableDoorways.Clear();

        StartCoroutine("LevelGenerator");
    }
}
