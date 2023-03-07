using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float health, stamina;
    public int currency;
    public static GameManager _instance;
    public List<ScriptObj_InvItem> allSavedItems;

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
