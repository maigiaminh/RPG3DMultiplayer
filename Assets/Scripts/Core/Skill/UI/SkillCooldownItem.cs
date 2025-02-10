using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownItem : MonoBehaviour
{

    [SerializeField] private Image _cooldownImage;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private Image _skillIcon;
    [SerializeField] private GameObject _blockPanel;

    public float Cooldown { get; private set; }
    public int LevelRequired { get; set; }
    private float CounterCooldown;
    private Coroutine _cooldownCoroutine;

    private void Start()
    {
        _cooldownImage.fillAmount = 0;
        Cooldown = 0;
        CounterCooldown = 0;
        _cooldownText.text = "";
    }

    private void OnEnable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += OnPlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= OnPlayerLevelChange;
    }

    public void Initialize(int levelRequired)
    {
        Debug.Log("LEVEL: " + levelRequired);
        LevelRequired = levelRequired;
    }


    public void StartCooldown(float time)
    {
        if (_cooldownCoroutine != null)
        {
            StopCoroutine(_cooldownCoroutine);
        }
        _cooldownText.text = time.ToString();
        _cooldownImage.fillAmount = 1;
        Cooldown = time;
        CounterCooldown = time;
        _cooldownCoroutine = StartCoroutine(UpdateCooldown());
    }

    private IEnumerator UpdateCooldown()
    {
        while (CounterCooldown > 0)
        {
            CounterCooldown -= Time.deltaTime;
            _cooldownImage.fillAmount = CounterCooldown / Cooldown;
            _cooldownText.text = ((int)CounterCooldown).ToString();
            yield return null;
        }
        _cooldownImage.fillAmount = 0;
        _cooldownText.text = "";
        _cooldownCoroutine = null;
    }
    public void SetSkillIcon(Sprite sprite, Color color)
    {
        _skillIcon.sprite = sprite;
        _skillIcon.color = color;
    }

    public void EnableSkillBlock(bool isActive)
    {
        _blockPanel.SetActive(isActive);
    }


    private void OnPlayerLevelChange(int level)
    {
        EnableSkillBlock(level < LevelRequired);
    }
}
