using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{

    public Animator playerAnim;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
            playerAnim.SetBool("is_attacking", true);
        }
        else
        {
            playerAnim.SetBool("is_attacking", false);
        }
    }

}
