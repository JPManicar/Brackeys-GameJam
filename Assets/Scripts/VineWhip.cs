using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineWhip : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lr;
    GameObject player;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attack()
    {
        // Find the closest transform to the attacker
        Transform closestTransform = FindClosestTransform(player);

        // Draw a line between the attacker and the closest transform

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPositions(new Vector3[] { player.transform.position, closestTransform.position });
    }


    public Transform FindClosestTransform(GameObject target)
    {
        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;
        Vector3 targetPosition = target.transform.position;

        foreach (Transform transform in GameObject.FindObjectsOfType<Transform>())
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance < closestDistance && transform != player.transform)
            {
                closestDistance = distance;
                closestTransform = transform;
            }
        }

        return closestTransform;
    }
}
