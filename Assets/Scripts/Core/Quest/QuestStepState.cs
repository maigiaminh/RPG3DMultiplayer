
[System.Serializable]
public class QuestStepState
{
    public string state;

    public QuestStepState()
    {
        state = "";
    }
    public QuestStepState(string state)
    {
        this.state = state;
    }

    public void SetState(string state)
    {
        this.state = state;
    }

    
}
