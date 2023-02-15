using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsDisplay : MonoBehaviour
{
    public Sprite fullHeart, halfHeart, emptyHeart;
    Image heartImage;
    void Awake()
    {
         heartImage = GetComponent<Image>();
    }

    //methods that handles the UI display of hearts
    public void SetHearthStatus(HeartStatus status)
    {
        switch(status)
        {
            case HeartStatus.empty:
                heartImage.sprite = emptyHeart;
                break;
            case HeartStatus.half:
                heartImage.sprite = halfHeart;
                break;
            case HeartStatus.full:
                heartImage.sprite = fullHeart;
                break;
        }
    }


    
    public enum HeartStatus
    {
        empty = 0,
        half = 1,
        full = 2
    }

}
