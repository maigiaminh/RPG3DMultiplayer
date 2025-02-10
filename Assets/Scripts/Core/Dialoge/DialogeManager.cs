using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DialogeManager : Singleton<DialogeManager>
{
    private void OnEnable()
    {
        GameEventManager.Instance.DialogeEvents.OnDialogeQuestStart += DialogeQuestStart;
        GameEventManager.Instance.DialogeEvents.OnDialogeFeatureStart += DialogeFeatureStart;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.DialogeEvents.OnDialogeQuestStart -= DialogeQuestStart;
        GameEventManager.Instance.DialogeEvents.OnDialogeFeatureStart -= DialogeFeatureStart;
    }

    private void DialogeQuestStart(QuestInfo questInfo, QuestState state, DialogeItem dialogeItem, AudioSource audioResource)
    {
        List<string> npcDialoges = new List<string>();
        List<string> playerDialoges = new List<string>();

        DialogeInfo dialogeInfo = questInfo.dialogeInfo;
        dialogeInfo.questName = questInfo.id;
        dialogeItem.actorIcon.sprite = dialogeInfo.actorIcon;
        List<AudioClip> audioClips = new List<AudioClip>();
        switch (state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                npcDialoges = dialogeInfo.requirementsNotMetText;
                audioClips = dialogeInfo.requirementsNotMetAudio;
                playerDialoges = dialogeInfo.playerRequirementsNotMetOptions;
                dialogeItem.questState = QuestState.REQUIREMENTS_NOT_MET;

                break;
            case QuestState.CAN_START:
                npcDialoges = dialogeInfo.descriptText;
                audioClips = dialogeInfo.descriptionAudio;
                playerDialoges = dialogeInfo.playerDescriptionOptions;
                dialogeItem.questState = QuestState.CAN_START;

                break;
            case QuestState.IN_PROGRESS:
                npcDialoges = dialogeInfo.inProgressText;
                audioClips = dialogeInfo.inProgressAudio;
                playerDialoges = dialogeInfo.playerInProgressOptions;
                dialogeItem.questState = QuestState.IN_PROGRESS;
                break;
            case QuestState.CAN_FINISH:
                npcDialoges = dialogeInfo.completedText;
                audioClips = dialogeInfo.completedAudio;
                playerDialoges = dialogeInfo.playerCompletedOptions;
                dialogeItem.questState = QuestState.CAN_FINISH;
                break;
            case QuestState.FINISHED:
                npcDialoges = dialogeInfo.finishedText;
                audioClips = dialogeInfo.finishedAudio;
                playerDialoges = dialogeInfo.playerFinishedOptions;
                dialogeItem.questState = QuestState.FINISHED;
                break;
        }
        dialogeItem.actorName = dialogeInfo.actorName;
        dialogeItem.questName = dialogeInfo.questName;

        DialogeUIManager.Instance.CreateQuestDialoge(npcDialoges, playerDialoges, audioClips, dialogeItem, audioResource);
    }



    private void DialogeFeatureStart(NpcFeatureType type, DialogeInfo info, FeatureItem item, AudioSource source)
    {
        List<string> npcDialoges = info.inProgressText;
        List<string> playerDialoges = info.playerInProgressOptions;
        List<AudioClip> audioClips = info.inProgressAudio;

        item.featureName = type.ToString();
        item.npcIcon.sprite = info.actorIcon;
        item.npcName = info.actorName;
        item.featureType = type;
    
        DialogeUIManager.Instance.CreateFeatureDialoge(npcDialoges, playerDialoges, audioClips, item, source);
    }


}
