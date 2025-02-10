using System.Collections.Generic;
using UnityEngine;

public class QuestContainer : Singleton<QuestContainer>
{
    public Dictionary<string, Quest> QuestMap = new Dictionary<string, Quest>();
}
