using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stationary_enemy_ai : MonoBehaviour
{
    // Start is called before the first frame update

    public bool attack;
    public static stationary_enemy_ai Instance;

    public GameObject projectilePrefab; 
    private GameObject player; 
    public int numProjectiles; 
    public float delayBetweenProjectiles;
    private Vector2 direction;
    private float angle;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
    }

    // Update is called once per frame
    void Update()
    {
        // calculate the angle between the spawner and the player
        direction = (player.transform.position - transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public IEnumerator StartShooting()
    {
        if (attack)
        {
            yield return SpawnProjectiles();
            yield return new WaitForSeconds(2);
        }
    }

    public IEnumerator SpawnProjectiles()
    {


        // spawn the projectiles in a burst
        for (int i = 0; i < numProjectiles; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
            projectileRigidbody.velocity = direction * 10f; // adjust the velocity to control the speed of the projectiles

            angle += 360f / numProjectiles;
            yield return new WaitForSeconds(delayBetweenProjectiles);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            attack = true;
            StartCoroutine(StartShooting());
            Debug.Log("player detected");
            
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("player") && attack == true)
        {
            
        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            Debug.Log("player left");
            attack = false;
            StopAllCoroutines();
        }
    }
}
