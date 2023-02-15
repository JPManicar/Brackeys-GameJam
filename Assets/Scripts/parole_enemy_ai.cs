using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class parole_enemy_ai : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float smoothTime = 0.3f;
    public bool followPlayer = false;
    public static parole_enemy_ai Instance;
    public Collider2D followCollider;
    private float startTime;
    private float journeyLength;
    private bool movingToEnd = true;
    private Transform player;
    private Vector2 velocity = Vector2.zero;
    Vector2 targetPosition;
    public bool movingToNearestPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        startTime = Time.time;
        journeyLength = Vector2.Distance(pointA.position, pointB.position);
    }

    void Update()
    {
        if (followPlayer && player != null && player_global_vars.Instance.stealthed == false)
        {
            // Follow player smoothly using SmoothDamp
            targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, speed);
            speed = 5f;
        }
        else
        {
            speed = 3f;
            if (movingToNearestPoint)
            {
                speed = 7f;
                // Move to nearest point
                targetPosition = FindNearestPoint().position;
                transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, speed);
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    movingToNearestPoint = false;
                    startTime = Time.time;
                    journeyLength = Vector2.Distance(pointA.position, pointB.position);
                }
            }
            else
            {
                // Move between points A and B
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                if (movingToEnd)
                {
                    transform.position = Vector2.Lerp(pointA.position, pointB.position, fracJourney);
                    if (fracJourney >= 1f)
                    {
                        movingToEnd = false;
                        startTime = Time.time;
                    }
                }
                else
                {
                    transform.position = Vector2.Lerp(pointB.position, pointA.position, fracJourney);
                    if (fracJourney >= 1f)
                    {
                        movingToEnd = true;
                        startTime = Time.time;
                    }
                }
            }
        }
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == followCollider && other.CompareTag("player"))
        {
            followPlayer = true;
            player = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == followCollider && other.CompareTag("player"))
        {
            followPlayer = false;
            player = null;
            movingToNearestPoint = true;
            startTime = Time.time;
            journeyLength = Vector2.Distance(transform.position, FindNearestPoint().position);
        }
    }

    Transform FindNearestPoint()
    {
        float distanceToA = Vector2.Distance(transform.position, pointA.position);
        float distanceToB = Vector2.Distance(transform.position, pointB.position);
        return distanceToA < distanceToB ? pointA : pointB;
    }

}



