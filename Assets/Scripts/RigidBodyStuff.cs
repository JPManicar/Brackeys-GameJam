using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyStuff : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rbody;
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();  
    }

    // Update is called once per frame
    void Update()
    {
        rbody.freezeRotation = true;
    }
}
