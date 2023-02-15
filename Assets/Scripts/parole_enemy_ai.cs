using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class parole_enemy_ai : MonoBehaviour
{
    public float speed = 0.1f;
    public bool followPlayer = false;
    public static parole_enemy_ai Instance;
    public Transform player;
    public Animator anim;
    public float distance;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();
    }

    void Update()
    {
        Vector2 myPos, targetPos;
        myPos = transform.position;
        targetPos.x = player.position.x;
        targetPos.y = 0;
       
        distance = Vector2.Distance(transform.position, player.position);
        if(!followPlayer)
        {
            anim.SetFloat("isWalking", 0);
            anim.SetBool("isAttacking", false);
        }
        if(followPlayer)
        {
            if(player.transform.localScale.x > 0)
            {
                transform.localScale = new Vector2(3, transform.localScale.y);
            }
            if (player.transform.localScale.x < 0)
            {
                transform.localScale = new Vector2(-3, transform.localScale.y);
            }
            if (distance >= 3)
            {
                anim.SetBool("isAttacking", false);
                anim.SetFloat("isWalking", 1);
                transform.position = Vector2.Lerp(myPos, targetPos, speed);
            }
            if(distance <= 3)
            {
                anim.SetFloat("isWalking", 0);
                anim.SetBool("isAttacking", true);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("player"))
        {
            followPlayer = true;
        }
    }





}



