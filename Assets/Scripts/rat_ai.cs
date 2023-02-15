using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rat_ai : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public float speed;
    public float distance;
    public static rat_ai Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();

        if ((transform.position.x > player.transform.position.x))
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        if (distance > 1)
        {
            if(!player_global_vars.Instance.stealthed)
            {
                transform.position += direction * speed * Time.deltaTime;
            }
            
        }
    }


    public void damagePlayer()
    {
        PlayerHealth.Instance.TakeDamage(1);
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }


}
