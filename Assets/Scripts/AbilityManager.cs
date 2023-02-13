using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class AbilityManager : MonoBehaviour
{
    public class Ability
    {
        public float damage;
        public float cooldown;
        public event Action OnUse;
        //public static event onuse OnUse;
        public Ability(float dmg, float cd, Action onuse)
        {
            damage = dmg;
            cooldown = cd;
            OnUse = onuse;

        }
        public void use()
        {
            OnUse?.Invoke();
        }
    }


    public static AbilityManager Instance;

    //ability images
    public Image a_image_1;
    public Image a_image_2;
    public Image a_bgr_image_1;
    public Image a_bgr_image_2;



    //cooldowns
    public float a_nextUseTime_1 = 0f;
    public float a_nextUseTime_2 = 0f;
    public float target_time;


    //current abilities
    private Ability current_ability_1;
    private Ability current_ability_2;

    //others
    public SpriteRenderer sr;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        //get compoenents



        //define all abilities
        Ability fireTiger = new Ability(40f, 5f, fireTigerAction);
        Ability InvChameleon = new Ability(40f, 8f, InvChameleonAction);
        current_ability_1 = fireTiger;
        current_ability_2 = InvChameleon;
    }


    void Update()
    {
        //image fill
        float fillAmount1 = 1 - (a_nextUseTime_1 - Time.time) / current_ability_1.cooldown;
        a_image_1.fillAmount = fillAmount1;

        float fillAmount2 = 1 - (a_nextUseTime_2 - Time.time) / current_ability_1.cooldown;
        a_image_2.fillAmount = fillAmount2;


        //check for input
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= a_nextUseTime_1)
        {
            StartCoroutine(Haptic(a_bgr_image_1));
            current_ability_1.use();
            a_nextUseTime_1 = Time.time + current_ability_1.cooldown;

        }
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= a_nextUseTime_2)
        {
            StartCoroutine(Haptic(a_bgr_image_2));
            current_ability_2.use();
            a_nextUseTime_2 = Time.time + current_ability_1.cooldown;
        }

    }





    IEnumerator Haptic(Image img)
    {
        img.rectTransform.sizeDelta = new Vector2(60, 70);
        yield return new WaitForSeconds(0.1f);
        img.rectTransform.sizeDelta = new Vector2(77, 80);
    }



    //why isnt any of this shit working bruh it worked before i closed the project what the actual fuck
    void fireTigerAction()
    {
        //for when movement code is done: make attack range and attack damage higher whilst this is active
        Debug.Log("Firetiger!");
    }

    void InvChameleonAction()
    {
        StartCoroutine(stealth());
    }

    IEnumerator stealth()
    {
        sr.color = new Color(255, 255, 255, 100);
        yield return new WaitForSeconds(4f);
        sr.color = new Color(255, 255, 255, 255);
    }
}
