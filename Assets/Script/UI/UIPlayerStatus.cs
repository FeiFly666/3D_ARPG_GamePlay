using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : MonoBehaviour
{
    [SerializeField] private Image playerHP;
    [SerializeField] private Image playerStamina;
    [SerializeField] private Image playerPoise;
    [SerializeField] private Image playerPower;

    [SerializeField] private Color staminaNormalColor;
    [SerializeField] private Color staminaExhaustedColor;

    [SerializeField] private Color powerNormalColor;
    [SerializeField] private Color powerUpColor;
    private bool exhaustedChanged = false;
    private bool powerUpChanged = false;
    private PlayerController player;

    void Start()
    {
        player = Manager.Character.player;

        if(player != null )
        {
            player.statusCtrl.OnHealthChanged += HandleHealthChanged;
            player.statusCtrl.OnStaminaChanged += HandleStaminaChanged;
            player.statusCtrl.OnPoiseChanged += HandlePoiseChanged;
            player.statusCtrl.OnPowerChanged += HandlePowerChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = Manager.Character.player;
            if (player != null)
            {
                player.statusCtrl.OnHealthChanged += HandleHealthChanged;
                player.statusCtrl.OnStaminaChanged += HandleStaminaChanged;
                player.statusCtrl.OnPoiseChanged += HandlePoiseChanged;
                player.statusCtrl.OnPowerChanged += HandlePowerChanged;
            }
        }
    }

    IEnumerator SmoothChanged(Image img, float target)
    {
        float start = img.fillAmount;
        float time = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - time < 0.15f)
        {
            img.fillAmount = Mathf.Lerp(start, target, (Time.realtimeSinceStartup - time) / 0.15f);
            yield return null;
        }

        img.fillAmount = target;
    }
    void HandleHealthChanged(float currentHP, float maxHP)
    {
        StartCoroutine(SmoothChanged(playerHP, currentHP / maxHP));
    }

    void HandleStaminaChanged(float curretnStamina, float maxStamina)
    {
        if(player.statusCtrl.isExhausted && !exhaustedChanged)
        {
            exhaustedChanged = true;
            playerStamina.color = staminaExhaustedColor;
        }
        else if(!player.statusCtrl.isExhausted && exhaustedChanged)
        {
            exhaustedChanged = false;
            playerStamina.color = staminaNormalColor;
        }
        StartCoroutine(SmoothChanged(playerStamina, curretnStamina /  maxStamina));
    }

    void HandlePoiseChanged(float currentPoise, float maxPoise)
    {
        StartCoroutine(SmoothChanged(playerPoise, currentPoise / maxPoise));
    }

    void HandlePowerChanged(float currentPower, float maxPower)
    {
        if(player.statusCtrl.IsPowerUp && !powerUpChanged)
        {
            powerUpChanged = true;
            playerPower.color = powerUpColor;
        }
        else if(!player.statusCtrl.IsPowerUp &&  powerUpChanged)
        {
            powerUpChanged = false;
            playerPower.color = powerNormalColor;
        }
        StartCoroutine(SmoothChanged(playerPower, currentPower / maxPower));
    }

    private void OnDestroy()
    {
        if(player != null)
        {
            player.statusCtrl.OnHealthChanged -= HandleHealthChanged;
            player.statusCtrl.OnStaminaChanged -= HandleStaminaChanged;
            player.statusCtrl.OnPoiseChanged -= HandlePoiseChanged;
            player.statusCtrl.OnPowerChanged -= HandlePowerChanged;
        }
    }
}
