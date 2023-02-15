using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_global_vars : MonoBehaviour
{
    // Start is called before the first frame update
    public static player_global_vars Instance;
    public bool stealthed = false;
    public SpriteRenderer[] srs;
    public Color hit_color = new Color(229, 0, 0);

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
        srs = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
