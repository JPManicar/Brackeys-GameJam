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
    public Image a_image_3;
    public Image a_bgr_image_1;
    public Image a_bgr_image_2;
    public Image a_bgr_image_3;

    //cooldowns
    public float a_nextUseTime_1 = 0f;
    public float a_nextUseTime_2 = 0f;
    public float a_nextUseTime_3 = 0f;
    public float target_time;

    //current abilities
    private Ability current_ability_1;
    private Ability current_ability_2;
    private Ability current_ability_3;

    //others
    public SpriteRenderer sr;
    public PlayerMovement player_movement;
    public GameObject player;
    public Rigidbody2D r;
    public SpriteRenderer[] srs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        //get compoenents
        player = GameObject.FindGameObjectWithTag("player");
        

        //define all abilities
        Ability fireTiger = new Ability(40f, 15f, fireTigerAction);
        Ability InvChameleon = new Ability(40f, 20f, InvChameleonAction);
        Ability vineWhip = new Ability(40f, 2f, vineWhipAction);

        current_ability_1 = vineWhip;
        current_ability_2 = InvChameleon;
        current_ability_3 = vineWhip;
        Debug.Log("Current Abilities: " + current_ability_1 + ", " + current_ability_2 + ", " + current_ability_3);
    }


    void Update()
    {
        //image fill
        float fillAmount1 = 1 - (a_nextUseTime_1 - Time.time) / current_ability_1.cooldown;
        a_image_1.fillAmount = fillAmount1;

        float fillAmount2 = 1 - (a_nextUseTime_2 - Time.time) / current_ability_2.cooldown;
        a_image_2.fillAmount = fillAmount2;

        float fillAmount3 = 1 - (a_nextUseTime_3 - Time.time) / current_ability_3.cooldown;
        a_image_3.fillAmount = fillAmount2;

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
            a_nextUseTime_2 = Time.time + current_ability_2.cooldown;
        }

        if (Input.GetKeyDown(KeyCode.R) && Time.time >= a_nextUseTime_3)
        {
            StartCoroutine(Haptic(a_bgr_image_3));
            current_ability_3.use();
            a_nextUseTime_3 = Time.time + current_ability_3.cooldown;
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

    void vineWhipAction()
    {



    }

    IEnumerator stealth()
    {
        player_global_vars.Instance.stealthed = true;
        Debug.Log("startign stealth");
        Color initialColor;
        initialColor = sr.color;
        foreach(var x in srs)
        {
            x.color = Color.gray;
            player_movement.overrideMaxSpeed = 11;
            player_movement.ifOverrideMaxSpeed = true;
        
        }
        yield return new WaitForSeconds(5f);
        foreach (var x in srs)
        {
            x.color = initialColor;
            player_movement.ifOverrideMaxSpeed = false;
            //player_movement.overrideMaxSpeed = 9;

        }
        player_global_vars.Instance.stealthed = false;

    }
}
