using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureItem : MonoBehaviour
{
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcNameBoxText;
    public string npcName;
    public Image npcIcon;
    public string featureName;
    public float textSpeed = .1f;
    public TextMeshProUGUI npcDescriptText;
    public GameObject playerChoicePanel;
    public List<Button> playerOptionButtons;
    public NpcFeatureType featureType;
}
