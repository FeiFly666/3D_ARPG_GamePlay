using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossStatus : MonoBehaviour
{
    [SerializeField] private Image playerHP;
    [SerializeField] private Image playerPoise;
    [SerializeField] private Image playerPower;

    [SerializeField] private Color powerNormalColor;
    [SerializeField] private Color powerUpColor;
    //[SerializeField] private TextMeshPro bossName;
    [SerializeField] private TextMeshProUGUI bossName;
    private bool powerUpChanged = false;
    private EnemyController boss;

    private void Start()
    {
        Manager.Event.Subscribe(EventManager.Event_Type.Boss_Find_Player,AddBoss);
        Manager.Event.Subscribe(EventManager.Event_Type.Boss_Lose_Player, RemoveBoss);

        this.gameObject.SetActive(false);
    }

    private void AddBoss(object args)
    {
        this.gameObject.SetActive(true);
        EnemyController newBoss = args as EnemyController;

        if (newBoss != null)
        {
            UnRegisteEvent();
            boss = newBoss;
            RegisteEvent();
            newBoss.statusCtrl.InvokeUIChange();
        }
    }
    private void RemoveBoss(object args)
    {
        this.gameObject.SetActive(false);

        UnRegisteEvent();
        boss = null;
    }
    private void RegisteEvent()
    {
        if (boss != null)
        {
            boss.statusCtrl.OnHealthChanged += HandleHealthChanged;
            boss.statusCtrl.OnPoiseChanged += HandlePoiseChanged;
            boss.statusCtrl.OnPowerChanged += HandlePowerChanged;
        }
        bossName.text = boss.data.Name;
    }
    private void UnRegisteEvent()
    {
        if (boss != null)
        {
            boss.statusCtrl.OnHealthChanged -= HandleHealthChanged;
            boss.statusCtrl.OnPoiseChanged -= HandlePoiseChanged;
            boss.statusCtrl.OnPowerChanged -= HandlePowerChanged;
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

    void HandlePoiseChanged(float currentPoise, float maxPoise)
    {
        StartCoroutine(SmoothChanged(playerPoise, currentPoise / maxPoise));
    }

    void HandlePowerChanged(float currentPower, float maxPower)
    {
        if (boss.statusCtrl.IsPowerUp && !powerUpChanged)
        {
            powerUpChanged = true;
            playerPower.color = powerUpColor;
        }
        else if (!boss.statusCtrl.IsPowerUp && powerUpChanged)
        {
            powerUpChanged = false;
            playerPower.color = powerNormalColor;
        }
        StartCoroutine(SmoothChanged(playerPower, currentPower / maxPower));
    }
    private void OnDestroy()
    {
        UnRegisteEvent();
        Manager.Event.UnSubscribe(EventManager.Event_Type.Boss_Find_Player, AddBoss);
        Manager.Event.UnSubscribe(EventManager.Event_Type.Boss_Lose_Player, RemoveBoss);
    }
}
