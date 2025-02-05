using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int terrainIndex;
    private TerrainDetector _terrainDetector;
    private SoundManager _soundManager;
    
    private Coroutine refresh;
    


    public float health;
    public float maxHealth;
    
    public float stamina;
    public float maxStamina;
    
    public Slider sliderH;
    public Slider sliderS;
    
    public bool isDead = false;
    
    public void Awake()
    {
        health = maxHealth;
        sliderH.maxValue = maxHealth;
        sliderH.value = maxHealth;

        stamina = maxStamina;
        sliderS.maxValue = maxStamina;
        sliderS.value = maxStamina;
    }
    
    private void Start()
    {
        _soundManager = FindObjectOfType<SoundManager>();
        _soundManager.SetFootstepsTarget(gameObject.transform.Find("Footsteps").GetComponent<AudioSource>());
        _terrainDetector = new TerrainDetector();
    }

    public int GetTerrainIndex()
    {
        terrainIndex = _terrainDetector.GetActiveTerrainTextureIdx(transform.position);
        return terrainIndex;
    }

    private void Step()
    {
        _soundManager.PlayFootstepsSound();
    }
    
    public void GetDamage(float damage)
    {
        if ((health - damage) >= 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;
            isDead = true;
        }
        sliderH.value = health;
    }

    public void UseStamina(float amount)
    {
        if ((stamina - amount) >= 0)
        {
            stamina -= amount;
            sliderS.value = stamina;

            if (refresh != null)
            {
                StopCoroutine(refresh);
            }

            refresh = StartCoroutine(RefreshStamina());
        }
        else
        {
            stamina = 0;
        }
    }
    
    private IEnumerator RefreshStamina()
    {
        yield return new WaitForSeconds(2); //wait 2 secs to refresh stamina

        while (stamina < maxStamina)
        {
            stamina += maxStamina / 100; //refreshing stamina as the same amount of spending it
            sliderS.value = stamina;
            yield return new WaitForSeconds(0.2f);
        }
        refresh = null;
    }
}
