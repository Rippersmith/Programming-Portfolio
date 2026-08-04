using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixStatusInfo : MonoBehaviour
{
    public GameObject[] allCubes;

    //public 

    // Start is called before the first frame update
    void Start()
    {
        GetAllCubesInOrderFromChildren();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetAllCubesInOrderFromChildren()
    {
        List<GameObject> allCubesList = new List<GameObject>();

        Transform currXYRow;

        for(int i = 0; i < transform.childCount; i++)
        {
            currXYRow = transform.GetChild(i);
            //xyRows.Add(transform.GetChild(i).gameObject);

            for (int j = 0; j < currXYRow.childCount; j++)
            {
                //xyRows.Add(transform.GetChild(i).gameObject);
                allCubesList.Add(currXYRow.GetChild(j).gameObject);
            }
        }

        allCubes = allCubesList.ToArray();

        //Debug.Log(xyRows[1].name) ;
    }
}
