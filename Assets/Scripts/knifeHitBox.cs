using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knifeHitBox : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer enemy_sr;
    private SpriteRenderer[] enemy_srs;
    private Color originalColor;
    private Color hit_color = new Color(229, 0, 0);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {

            enemy_srs = collision.GetComponentInParent<parole_enemy_ai>().srs;
            foreach (var x in enemy_srs)
            {
                x.color = hit_color;
            }
            Invoke("ResetSpritesColor", 0.1f);
            collision.gameObject.GetComponent<Enemy_Health>().health -= 35;
            
        }
        if (collision.CompareTag("rat"))
        {
            enemy_sr = collision.gameObject.GetComponentInParent<SpriteRenderer>();
            enemy_sr.color = hit_color;
            Invoke("ResetSpriteColor", 0.1f);
            collision.gameObject.GetComponentInParent<Enemy_Health>().health -= 100;

        }
    }


    void ResetSpritesColor()
    {
        foreach(var x in enemy_srs)
        {
            x.color = Color.white;
        }

    }
    void ResetSpriteColor()
    {
        enemy_sr.color = Color.white;
    }
}
