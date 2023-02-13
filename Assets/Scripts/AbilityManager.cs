using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    //ability images
    public Image a_image_1;
    public Image a_image_2;
    public Image a_bgr_image_1;
    public Image a_bgr_image_2;

    //cooldowns
    public float a_cd_1 = 3f;
    public float a_cd_2 = 3f;
    public float a_nextUseTime_1 = 0f;
    public float a_nextUseTime_2 = 0f;
    public float target_time;



    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        
    }


    void Update()
    {
        //image fill
        float fillAmount1 = 1 - (a_nextUseTime_1 - Time.time) / a_cd_1;
        a_image_1.fillAmount = fillAmount1;

        float fillAmount2 = 1 - (a_nextUseTime_2 - Time.time) / a_cd_2;
        a_image_2.fillAmount = fillAmount2;


        //check for input
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= a_nextUseTime_1)
        {
            StartCoroutine(test(a_bgr_image_1));
            a_nextUseTime_1 = Time.time + a_cd_1;
        }
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= a_nextUseTime_2)
        {
            StartCoroutine(test(a_bgr_image_2));
            a_nextUseTime_2 = Time.time + a_cd_2;
        }

    }

    IEnumerator test(Image img)
    {
        img.rectTransform.sizeDelta = new Vector2(60, 70);
        yield return new WaitForSeconds(0.1f);
        img.rectTransform.sizeDelta = new Vector2(77, 80);
    }
    

}
