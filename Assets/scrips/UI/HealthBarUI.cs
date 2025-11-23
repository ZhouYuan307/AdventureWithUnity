using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Entity entity;
    private RectTransform rectTransform;
    private Slider slider;
    private CharacterStats stats;
    private Canvas canvas;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        stats = GetComponentInParent<CharacterStats>();
        canvas = GetComponent<Canvas>();

        entity.onFlipped += FlipUI;
        stats.onHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = stats.GetMaxHealthValue();
        slider.value = stats.currentHealth;
    }

    private void FlipUI() => rectTransform.Rotate(0, 180, 0);

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        stats.onHealthChanged -= UpdateHealthUI;
    }


    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
        {
            canvas.enabled = false;
        }
        else
        {
            canvas.enabled = true;
        }
    }

}
