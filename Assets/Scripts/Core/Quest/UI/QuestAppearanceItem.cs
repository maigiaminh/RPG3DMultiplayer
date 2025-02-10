using TMPro;
using UnityEngine;

public class QuestAppearanceItem : MonoBehaviour
{
    public TextMeshProUGUI questContentText;
    public string QuestName {get; private set;}
    public string QuestContent {get; private set;}


    public void UpdateQuestNameText(string questName, string content){
        QuestName = questName;
        QuestContent = QuestContent;
        questContentText.text = content;
    }
}
