using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacterManager : MonoBehaviour
{
    public static ChooseCharacterManager Instance;

    [Header("Choose Class & Choose Oufit Places")]
    public GameObject ChooseClassPlace;
    public GameObject ChooseOutfitPlace;

    [Header("Character Description Panel")]
    public GameObject CharacterDescriptionPanel;
    public GameObject CharacterIconGameObject;
    [Header("Character Description Object")]
    public TextMeshProUGUI CharacterName;
    public List<TextMeshProUGUI> CharacterDescriptionsList = new List<TextMeshProUGUI>();
    public List<Image> SkillBackgroundColors = new List<Image>();
    public Image IconBackgroundImage;
    public Image CharacterIconImage;
    public Image Skill1Icon;
    public Image Skill2Icon;
    public Image Skill3Icon;
    public Image Skill4Icon;


    private List<Animator> animators = new List<Animator>();
    public string CurrentCharacterName {get; private set;}
    private bool _canChoose = false;
    Coroutine disablePanelCoroutine;


    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Choose Character Manager in the scene.");
        }
        Instance = this;
        GetAllAnimators();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _canChoose)
        {
            if(CharacterDescriptionPanel.activeSelf){
                SwitchToChooseGender();
                CursorManager.Instance.ChangeCursorMode(CursorLockMode.None, true);
            }
        }

    }

    private void SwitchToChooseGender()
    {
        ChooseClassPlace.SetActive(false);
        ChooseOutfitPlace.SetActive(true);
    }

    private void GetAllAnimators()
    {
        animators = CharacterDescriptionPanel.GetComponentsInChildren<Animator>().ToList();
    }

    public void UpdateDescription(CharacterDescript characterDescription)
    {
        CurrentCharacterName = characterDescription.CharacterName;
        CharacterName.text = characterDescription.CharacterName;
        UpdateText(characterDescription.CharacterDescriptionsList);

        CharacterIconImage.sprite = characterDescription.CharacterIcon;
        Skill1Icon.sprite = characterDescription.Skill1Icon;
        Skill2Icon.sprite = characterDescription.Skill2Icon;
        Skill3Icon.sprite = characterDescription.Skill3Icon;
        Skill4Icon.sprite = characterDescription.Skill4Icon;

        IconBackgroundImage.color = characterDescription.IconBackgroundColor;
        SkillBackgroundColors.ForEach(image => image.color = characterDescription.SkillBackgroundColor);

        ActivateCharacterDescriptionPanel(true);
    }

    private void UpdateText(List<string> characterDescriptionsList)
    {
        int i = 0;
        for (i = 0; i < characterDescriptionsList.Count; i++)
        {
            CharacterDescriptionsList[i].text = characterDescriptionsList[i];
            if (CharacterDescriptionsList[i]) CharacterDescriptionsList[i].gameObject.SetActive(true);
        }
        for (; i < CharacterDescriptionsList.Count; i++)
        {
            CharacterDescriptionsList[i].text = "";
            if (CharacterDescriptionsList[i]) CharacterDescriptionsList[i].gameObject.SetActive(false);
        }
    }

    public void ActivateCharacterDescriptionPanel(bool isActive)
    {
        if (isActive)
        {
            _canChoose = true;
            CharacterDescriptionPanel.SetActive(true);
            SetAnimation("Appear");
            if (disablePanelCoroutine != null) StopCoroutine(disablePanelCoroutine);
        }
        else
        {
            _canChoose = false;
            SetAnimation("Disappear");
            disablePanelCoroutine = StartCoroutine(DisablePanel());
        }
    }

    private void SetAnimation(string trigger)
    {
        if (trigger == "Appear") animators.ForEach(animator => animator.ResetTrigger("Disappear"));
        else animators.ForEach(animator => animator.ResetTrigger("Appear"));
        foreach (var animator in animators)
        {
            animator.SetTrigger(trigger);
        }
        animators.ForEach(animator => animator.SetTrigger(trigger));
    }

    IEnumerator DisablePanel()
    {
        yield return new WaitForSeconds(1f);
        CharacterDescriptionPanel.SetActive(false);
        disablePanelCoroutine = null;
    }



}
