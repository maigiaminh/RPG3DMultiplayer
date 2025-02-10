using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : Singleton<LevelView>
{
    [SerializeField] private Slider experienceBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float _lerpSpeed = 5f; // Speed of the lerp


    private float _targetFillAmount; // Store the target fill amount
    private Coroutine _lerpCoroutine;

    public void UpdateExperience(int maxExp, int curExp)
    {
        _targetFillAmount = (float)curExp / maxExp; // Update target fill amount

        // Start lerp coroutine (or restart if already running)
        if (_lerpCoroutine != null)
        {
            StopCoroutine(_lerpCoroutine);
        }
        _lerpCoroutine = StartCoroutine(LerpExperienceBar());
    }

    private IEnumerator LerpExperienceBar()
    {
        while (experienceBar.value != _targetFillAmount)
        {
            experienceBar.value = Mathf.Lerp(experienceBar.value, _targetFillAmount, Time.deltaTime * _lerpSpeed);
            yield return null;
        }
        _lerpCoroutine = null;  // Clear the coroutine reference when finished
    }


    public void UpdateLevel(int level)
    {
        levelText.text = level + ""; // Or any other format you prefer
    }
}