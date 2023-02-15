using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_global_vars : MonoBehaviour
{
    // Start is called before the first frame update
    public static player_global_vars Instance;
    public bool stealthed = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        stealthed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
