using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    [SerializeField] private Image timeImage;
    [SerializeField] private float totalTime;
    [SerializeField] private PlayerMovement player;
    
    private float time;
    private bool isTime = false;
    
    private void Start()
    {
        time = totalTime;
    }

    private void Update()
    {
        if (isTime) return;
        
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeImage.fillAmount = time / totalTime;
        }

        else
        {
            isTime = true;
            player.SetCanMove();
            HudManager.Instance.Lose();
        }
    }
}
