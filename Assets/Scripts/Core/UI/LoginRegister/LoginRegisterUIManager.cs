using TMPro;
using UnityEngine;

public class LoginRegisterUIManager : MonoBehaviour
{
    public static LoginRegisterUIManager Instance { get; private set; }

    public GameObject gamePanel;
    public GameObject logoutPanel;
    public GameObject backgroundPanel;
    public GameObject loginRegisterPanel;
    public GameObject emailVertificationPanel;
    public TextMeshProUGUI emailVertificationText;
    public Animator animator;

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivateLoginRegisterPanel(bool isActive)
    {
        ClearUI();
        backgroundPanel.SetActive(isActive);
        loginRegisterPanel.SetActive(isActive);
    }

    public void ClearUI()
    {
        gamePanel.SetActive(false);
        backgroundPanel.SetActive(false);
        loginRegisterPanel.SetActive(false);
        emailVertificationPanel.SetActive(false);
        logoutPanel.SetActive(false);
    }



    public void ShowGamePanel()
    {
        Debug.Log("ShowGamePanel");
        ClearUI();
        animator.Play("LoginSuccess");
        logoutPanel.SetActive(true);
        gamePanel.SetActive(true);
    }

    public void ShowEmailVertificationPanel(bool isEmailSent, string emailId, string errorMessage)
    {
        ClearUI();
        emailVertificationPanel.SetActive(true);

        if(isEmailSent)
            emailVertificationText.text = $"Please check your email address \n Vertification email has been sent to {emailId}";
        else
            emailVertificationText.text = $"Could not send vertification email: {errorMessage}";
    }
}
