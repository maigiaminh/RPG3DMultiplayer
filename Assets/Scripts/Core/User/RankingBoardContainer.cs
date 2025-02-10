using System.Collections.Generic;
using UnityEngine;

public class RankingBoardContainer : Singleton<RankingBoardContainer>
{
    [HideInInspector] public List<RankingLevelData> RankingLevelItems = new List<RankingLevelData>();

    public void SetRankingLevelItems(List<RankingLevelData> rankingLevelItems) {
        RankingLevelItems = rankingLevelItems;
    }
}
