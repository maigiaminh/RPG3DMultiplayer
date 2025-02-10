using System;
using UnityEngine;

public class DialogeEvents
{
    public event Action<QuestInfo, QuestState, DialogeItem, AudioSource> OnDialogeQuestStart;
    public void DialogeQuestStart(QuestInfo dialogeInfo, QuestState questState, DialogeItem dialogeItem, AudioSource audioSource)
    {
        OnDialogeQuestStart?.Invoke(dialogeInfo, questState, dialogeItem, audioSource);
    }
    public event Action<NpcFeatureType, DialogeInfo, FeatureItem, AudioSource> OnDialogeFeatureStart;
    public void DialogeFeatureStart(NpcFeatureType featureType, DialogeInfo dialogeInfo, FeatureItem featureItem, AudioSource audioSource)
    {
        OnDialogeFeatureStart?.Invoke(featureType, dialogeInfo, featureItem, audioSource);
    }



}
