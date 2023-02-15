using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kill_me : MonoBehaviour
{
    // Start is called before the first frame update
    public static kill_me Instance;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
