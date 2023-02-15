using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignore_enemy_collision : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] all_pirates;
    public GameObject[] all_rats;
    public GameObject[] all_turrets;
    public static ignore_enemy_collision Instance;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        all_pirates = GameObject.FindGameObjectsWithTag("Enemy");
        all_rats = GameObject.FindGameObjectsWithTag("rat");
        all_turrets = GameObject.FindGameObjectsWithTag("turret");

        foreach (var x in all_pirates)
        {
            foreach(var y in all_rats)
            {
                Physics2D.IgnoreCollision(x.GetComponent<Collider2D>(), y.GetComponent<Collider2D>());
            }
            
        }
    }
}
