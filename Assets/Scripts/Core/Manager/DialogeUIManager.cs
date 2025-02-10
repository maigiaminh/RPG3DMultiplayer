using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class DialogeUIManager : Singleton<DialogeUIManager>
{
    // contact Press E Board
    [Header("Dialoge UI")]
    [SerializeField] private GameObject _actorNameBoard;
    private Animator _actorNameBoardAnimator;
    private Image _actorIcon;
    private TextMeshProUGUI _actorNameText;
    // Dialoge Section;
    private TextMeshProUGUI _actorDialogeNameText;
    public bool IsDialogOpen { get; set; }
    private Image _actorSprite;
    private Coroutine _typeLineCoroutine;
    private int _currentDialogeIndex = 0;
    private int _dialogeAmount = 0;
    private List<string> _descriptDialoges;
    private Dictionary<int, List<string>> _playerDialoges;
    private List<AudioClip> _audioClips;
    public DialogeItem dialogeItem;
    public FeatureItem featureItem;
    private bool _choice = false;


    private AudioSource _currentAudioResource;
    private int _currentAudioIndex = 0;
    //

    protected override void Awake()
    {
        base.Awake();
        if (_actorNameBoard)
        {
            _actorNameBoardAnimator = _actorNameBoard.GetComponent<Animator>();
            _actorNameText = _actorNameBoard.GetComponentInChildren<TextMeshProUGUI>();
            _actorIcon = _actorNameBoard.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        }
    }


    #region Dialoge Section


    private void DeactiveAllChilds()
    {
        // GameObject[] childs = transform.GetComponentsInChildren<GameObject>();
        // foreach (GameObject child in childs)
        // {
        //     child.SetActive(false);
        // }
    }

    public void CreateQuestDialoge(List<string> dialoges, List<string> playerDialoges, List<AudioClip> audioClips, DialogeItem dialogeItem, AudioSource audioResource)
    {
        if (IsDialogOpen) return;
        this.dialogeItem = dialogeItem;
        Debug.Log("Create dialoge: " + dialogeItem.actorName);
        Debug.Log("Dialoges count: " + dialogeItem.questName);
        _actorDialogeNameText = dialogeItem.npcNameText;
        _currentDialogeIndex = 0;
        _descriptDialoges = dialoges;
        _actorSprite = dialogeItem.actorIcon;
        _playerDialoges = GetPlayerDialogeOptions(playerDialoges);
        _currentAudioIndex = 0;
        _currentAudioResource = audioResource;
        _audioClips = audioClips;
        IsDialogOpen = true;
        dialogeItem.npcNameText.text = dialogeItem.actorName;
        dialogeItem.npcNameBoxText.text = dialogeItem.actorName;
        // _actorDialogeNameText.text = dialogeItem.actorName;
        GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenDialogeQuest(this.dialogeItem);

        DeactiveAllChilds();
        CreateDialogeStretchCanvas(dialogeItem);
        this.dialogeItem.playerChoicePanel.SetActive(false);



        if (dialoges.Count != playerDialoges.Count)
        {
            Debug.LogError("Dialoges and player dialoges count not equal at " + dialogeItem.name + " dialoge item.");
        }
        // if (dialoges.Count != audioClips.Count)
        // {
        //     Debug.LogError("Dialoges and audio clips count not equal at " + dialogeItem.name + " dialoge item.");
        // }
        Debug.Log("Dialoges count: " + dialoges.Count);
        Debug.Log("Player dialoges count: " + playerDialoges.Count);

        SettingPlayerOptions(_currentDialogeIndex);
        _typeLineCoroutine = StartCoroutine(TypeLine(dialoges[_currentDialogeIndex]));

        _dialogeAmount = dialoges.Count;

        SetListenerToPlayerOptions();
    }

    public void CreateFeatureDialoge(List<string> npcDialoges, List<string> playerDialoges, List<AudioClip> audioClips, FeatureItem item, AudioSource source)
    {
        if (IsDialogOpen) return;
        featureItem = item;
        Debug.Log("Create dialoge: " + featureItem.npcName);
        _actorDialogeNameText = featureItem.npcNameText;
        _currentDialogeIndex = 0;
        _descriptDialoges = npcDialoges;
        _actorSprite = featureItem.npcIcon;
        _playerDialoges = GetPlayerDialogeOptions(playerDialoges);
        _currentAudioIndex = 0;
        _currentAudioResource = source;
        _audioClips = audioClips;
        IsDialogOpen = true;
        featureItem.npcNameText.text = featureItem.featureName;
        featureItem.npcNameBoxText.text = featureItem.npcName;

        // _actorDialogeNameText.text = .actorName;
        GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenDialogeFeature(this.featureItem);

        DeactiveAllChilds();
        CreateDialogeStretchCanvas(featureItem);
        this.featureItem.playerChoicePanel.SetActive(false);


        Debug.Log("Dialoges count: " + npcDialoges.Count);
        Debug.Log("Player npcDialoges count: " + playerDialoges.Count);
        SettingPlayerOptions(_currentDialogeIndex);
        _typeLineCoroutine = StartCoroutine(TypeLine(npcDialoges[_currentDialogeIndex]));
        _dialogeAmount = npcDialoges.Count;
        SetListenerToPlayerOptions();
    }

    private void PlayDialogeAudio(AudioSource audioResource, AudioClip audioClip)
    {
        if (_currentAudioIndex >= _audioClips.Count) return;
        if (audioResource.isPlaying) audioResource.Stop();
        audioResource.PlayOneShot(audioClip);
        _currentAudioIndex++;
    }


    private void SetListenerToPlayerOptions()
    {
        SetButtonToDefaultState();

        if (dialogeItem)
        {
            for (int i = 0; i < dialogeItem.playerOptionButtons.Count; i++)
            {
                int index = i;
                Button button = dialogeItem.playerOptionButtons[index];
                Debug.Log("Button Name: " + button.gameObject.name);
                button.onClick.AddListener(() => ChooseOptions(index));
            }
        }
        else if (featureItem)
        {
            for (int i = 0; i < featureItem.playerOptionButtons.Count; i++)
            {
                int index = i;
                Button button = featureItem.playerOptionButtons[index];
                Debug.Log("Button Name: " + button.gameObject.name);
                button.onClick.AddListener(() => ChooseOptions(index));
            }
        }

    }

    private void CreateDialogeStretchCanvas(DialogeItem dialogeItem)
    {
        Debug.Log("Create Dialoge Stretch Canvas + dialogeItem");
        this.dialogeItem = Instantiate(dialogeItem);
        this.dialogeItem.transform.SetParent(transform);
        RectTransform rectTransform = this.dialogeItem.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.dialogeItem.npcDescriptText.text = "";
    }

    private void CreateDialogeStretchCanvas(FeatureItem featureItem)
    {
        Debug.Log("Create Dialoge Stretch Canvas + featureItem");
        this.featureItem = Instantiate(featureItem);
        this.featureItem.transform.SetParent(transform);
        RectTransform rectTransform = this.featureItem.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.featureItem.npcDescriptText.text = "";
    }

    private void ChooseOptions(int index)
    {
        _choice = index == 0 ? true : false;

        // NEEDED TO CONTROL LAST STATE
        if (_currentDialogeIndex >= _dialogeAmount - 1)
        {
            if (dialogeItem)
            {
                var state = dialogeItem.questState;
                var item = dialogeItem;
                CloseDialoge(dialogeItem);
                HandlePlayerChooseDecide(item,state, _choice);
            }
            if (featureItem)
            {
                var type = featureItem.featureType;
                CloseDialoge(featureItem);
                HandlePlayerChooseDecide(type, _choice);
            }


            return;
        }


        ChooseDescripOption();

        Debug.Log("Choose option: " + _choice);

    }


    private void SetButtonToDefaultState()
    {
        if (dialogeItem)
        {
            foreach (Button button in dialogeItem.playerOptionButtons)
            {
                if (button.transition == Selectable.Transition.Animation)
                {
                    button.animator.SetTrigger("Normal");
                    button.animator.ResetTrigger("Highlighted");
                    button.animator.ResetTrigger("Pressed");
                    button.animator.ResetTrigger("Selected");
                    button.animator.ResetTrigger("Disabled");

                }
                button.OnDeselect(null);
            }
        }
        else if (featureItem)
        {
            foreach (Button button in featureItem.playerOptionButtons)
            {
                if (button.transition == Selectable.Transition.Animation)
                {
                    button.animator.SetTrigger("Normal");
                    button.animator.ResetTrigger("Highlighted");
                    button.animator.ResetTrigger("Pressed");
                    button.animator.ResetTrigger("Selected");
                    button.animator.ResetTrigger("Disabled");

                }
                button.OnDeselect(null);
            }
        }
    }

    private void ChooseDescripOption()
    {
        _currentDialogeIndex++;
        Debug.Log("Choose option: " + _currentDialogeIndex);


        StartCoroutine(DelayToActivePlayerChoicePanel());


    }

    IEnumerator DelayToActivePlayerChoicePanel()
    {
        yield return new WaitForSeconds(.2f);
        SetActivePlayerChoicePanel(false);

        if (_typeLineCoroutine != null)
        {
            StopCoroutine(_typeLineCoroutine);
        }
        _typeLineCoroutine = StartCoroutine(TypeLine(_descriptDialoges[_currentDialogeIndex]));
        SettingPlayerOptions(_currentDialogeIndex);
    }

    private void SettingPlayerOptions(int currentDialogeIndex)
    {
        List<string> playerOptions = _playerDialoges[currentDialogeIndex];
        for (int i = 0; i < playerOptions.Count; i++)
        {
            if (dialogeItem)
            {
                if (i >= dialogeItem.playerOptionButtons.Count)
                {
                    Debug.LogWarning("Player option buttons count is less than player options count at " + dialogeItem.name + " dialoge item.");
                    continue;
                }
                dialogeItem.playerOptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerOptions[i];
            }
            else if (featureItem)
            {
                if (i >= featureItem.playerOptionButtons.Count)
                {
                    Debug.LogWarning("Player option buttons count is less than player options count at " + featureItem.name + " dialoge item.");
                    continue;
                }
                featureItem.playerOptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerOptions[i];
            }
        }
        if (dialogeItem)
        {
            if (playerOptions.Count < dialogeItem.playerOptionButtons.Count)
            {
                for (int i = playerOptions.Count; i < dialogeItem.playerOptionButtons.Count; i++)
                {
                    dialogeItem.playerOptionButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else if (featureItem)
        {
            if (playerOptions.Count < featureItem.playerOptionButtons.Count)
            {
                for (int i = playerOptions.Count; i < featureItem.playerOptionButtons.Count; i++)
                {
                    featureItem.playerOptionButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator TypeLine(string line)
    {
        PlayDialogeAudio(_currentAudioResource, _audioClips[_currentAudioIndex]);

        if (dialogeItem)
        {
            dialogeItem.npcDescriptText.text = "";
            Debug.Log("Typing line: " + line);
            foreach (char c in line.ToCharArray())
            {
                dialogeItem.npcDescriptText.text += c;
                yield return new WaitForSeconds(dialogeItem.textSpeed);
            }
        }
        else if (featureItem)
        {
            featureItem.npcDescriptText.text = "";
            Debug.Log("Typing line: " + line);
            foreach (char c in line.ToCharArray())
            {
                featureItem.npcDescriptText.text += c;
                yield return new WaitForSeconds(featureItem.textSpeed);
            }
        }
        SetActivePlayerChoicePanel(true);

        SetButtonToDefaultState();
        _typeLineCoroutine = null;
    }


    private void NextLine()
    {
        if (_typeLineCoroutine != null)
        {
            StopCoroutine(_typeLineCoroutine);
        }
    }

    private void SetActivePlayerChoicePanel(bool isActive)
    {
        if (dialogeItem)
            dialogeItem.playerChoicePanel.SetActive(isActive);
        else if (featureItem)
            featureItem.playerChoicePanel.SetActive(isActive);
    }

    private Dictionary<int, List<string>> GetPlayerDialogeOptions(List<string> dialoges)
    {
        Dictionary<int, List<string>> playerDialoges = new Dictionary<int, List<string>>();
        for (int i = 0; i < dialoges.Count; i++)
        {
            string[] str = dialoges[i].Split("@ ");
            foreach (string s in str)
            {
                if (playerDialoges.ContainsKey(i))
                {
                    playerDialoges[i].Add(s);
                }
                else
                {
                    playerDialoges.Add(i, new List<string>() { s });
                }
            }
        }
        return playerDialoges;
    }

    public void CloseDialoge(DialogeItem item)
    {
        if (_typeLineCoroutine != null)
        {
            StopCoroutine(_typeLineCoroutine);
        }
        _currentAudioResource.Stop();
        IsDialogOpen = false;
        GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseDialogeQuest(item);
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        if (dialogeItem) Destroy(dialogeItem.gameObject);
        if (featureItem) Destroy(featureItem.gameObject);
        dialogeItem = null;
        featureItem = null;
    }

    public void CloseDialoge(FeatureItem item)
    {
        if (_typeLineCoroutine != null)
        {
            StopCoroutine(_typeLineCoroutine);
        }
        _currentAudioResource.Stop();
        IsDialogOpen = false;
        GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseDialogeFeature(item);
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();

        Destroy(featureItem.gameObject);
        featureItem = null;
    }
    private void HandleWhenPlayerChooseYes()
    {
    }


    #endregion

    private void HandlePlayerChooseDecide(DialogeItem item, QuestState state, bool choice)
    {
        if (!choice) return;
        switch (state)
        {
            case QuestState.CAN_START:
                GameEventManager.Instance.QuestEvents.StartQuest(item.questName);
                break;
            case QuestState.CAN_FINISH:
                GameEventManager.Instance.QuestEvents.FinishQuest(item.questName);
                break;
        }

    }

    private void HandlePlayerChooseDecide(NpcFeatureType featureType, bool choice)
    {
        if(!choice) return;
        switch (featureType)
        {
            case NpcFeatureType.Shop:
                ShopUIManager.Instance.OpenStore();
                break;
        }
    }



    public void ActivateContactPressEBoard(string actorName, Sprite sprite, bool isActive)
    {
        if (isActive)
        {
            _actorNameText.text = actorName;
            _actorIcon.sprite = sprite;
            _actorNameBoard.SetActive(isActive);
        }
        else
        {
            _actorNameBoardAnimator.SetTrigger("Disappear");
            StartCoroutine(DelayToDeactiveContactPressEBoard(.5f));
        }
    }

    private IEnumerator DelayToDeactiveContactPressEBoard(float time)
    {
        yield return new WaitForSeconds(time);
        _actorNameBoard.SetActive(false);
    }


}
