[System.Serializable]
public class QuestData 
{
    public QuestState questState;
    public int currentQuestStepIndex;

    public QuestStepState[] questStepStates;

    public QuestData(QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        this.questState = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
    }
}
