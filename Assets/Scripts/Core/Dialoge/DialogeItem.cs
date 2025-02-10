using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogeItem : MonoBehaviour
{
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcNameBoxText;
    public string actorName;
    public Image actorIcon;
    public string questName;
    public float textSpeed = .1f;
    public QuestState questState;
    public TextMeshProUGUI npcDescriptText;
    public GameObject playerChoicePanel;
    public List<Button> playerOptionButtons;

    
}
