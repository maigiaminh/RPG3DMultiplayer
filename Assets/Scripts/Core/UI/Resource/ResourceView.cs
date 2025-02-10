using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceView : Singleton<ResourceView>
{

    [Header("Resource Texts")]
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _miscText;

    [SerializeField] private float duration = 1f;

    private Coroutine _updateGoldCoroutine;
    private Coroutine _updateMiscCoroutine;


    public void UpdateGoldText(int amount)
    {
        if (_updateGoldCoroutine != null)
        {
            StopCoroutine(_updateGoldCoroutine);
        }
        _updateGoldCoroutine = StartCoroutine(UpdateGoldTextCoroutine(_goldText, amount, duration));
    }


    public void UpdateMiscText(int amount)
    {
        if (_updateMiscCoroutine != null)
        {
            StopCoroutine(_updateMiscCoroutine);
        }
        _updateMiscCoroutine = StartCoroutine(UpdateMiscTextCoroutine(_miscText, amount, duration));
    }

    IEnumerator UpdateGoldTextCoroutine(TextMeshProUGUI textElement, int targetAmount, float duration)
    {
        int startAmount;
        string amountText = GetAmount(textElement.text);
        if (textElement.text == "")
        {
            startAmount = 0;
        }
        else if (!int.TryParse(amountText, out startAmount))
        {
            Debug.LogError("Không thể chuyển đổi giá trị hiện tại thành số nguyên.");
            yield break; // Dừng coroutine nếu có lỗi
        }


        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Giá trị t từ 0 đến 1

            int currentAmount = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, t));
            textElement.text = GetCurrentAmountModifyText(currentAmount.ToString());

            yield return null; // Chờ đến frame tiếp theo
        }

        textElement.text = GetCurrentAmountModifyText(targetAmount.ToString()); // Đảm bảo giá trị cuối cùng chính xác

        _updateGoldCoroutine = null;
    }


    IEnumerator UpdateMiscTextCoroutine(TextMeshProUGUI textElement, int targetAmount, float duration)
    {
        int startAmount;
        string amountText = GetAmount(textElement.text);
        if (!int.TryParse(amountText, out startAmount))
        {
            Debug.LogError("Không thể chuyển đổi giá trị hiện tại thành số nguyên.");
            yield break; // Dừng coroutine nếu có lỗi
        }


        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Giá trị t từ 0 đến 1

            int currentAmount = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, t));
            textElement.text = GetCurrentAmountModifyText(currentAmount.ToString());

            yield return null; // Chờ đến frame tiếp theo
        }

        textElement.text = GetCurrentAmountModifyText(targetAmount.ToString()); // Đảm bảo giá trị cuối cùng chính xác

        _updateMiscCoroutine = null;
    }
    private string GetCurrentAmountModifyText(string text)
    {
        string result = "";
        for (int i = text.Length - 1; i >= 0; i--)
        {
            result = text[i] + result;
            if ((text.Length - i) % 3 == 0 && i != 0)
            {
                result = "," + result;
            }
        }
        return result;
    }
    private string GetAmount(string text)
    {
        return text.Replace(",", "");
    }


}
