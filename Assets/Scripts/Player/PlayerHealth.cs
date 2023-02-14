using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    
    public int playerHealth;
    public int maxHealth;
    public Slider healthSlider;
    
    void Start()
    {

        setMaxHealth(maxHealth);
        setHealth(maxHealth);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(2);
        }
    }
    public void setHealth(int value)
    {
        playerHealth = value;
        healthSlider.value = playerHealth;
    }
    public void setMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        healthSlider.maxValue = newMaxHealth;
        healthSlider.value = newMaxHealth;
    }
    public void heal(int healAmount)
    {
        int temp = playerHealth + healAmount; 
        if(temp > maxHealth)
            playerHealth = maxHealth;
        else
            playerHealth = temp;
    }
    public void TakeDamage(int damageTaken)
    {
        playerHealth-= damageTaken;
        setHealth(playerHealth);
    }

}
