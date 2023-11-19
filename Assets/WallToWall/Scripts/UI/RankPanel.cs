using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RankPanel : BaseScreen
{
    [SerializeField] private List<RankingItem> rankingItems;

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        SetRanking();
    }

    [Button("SetRanking")]
    public void SetRanking()
    {
        var rankList = RankManager.Instance.GetRankList;

        int index = 0;
        
        foreach (var rank in rankList)
        {
            if (rankingItems.Count <= index) break;
            if (rank.Value == -1)
            {
                rankingItems[index].gameObject.SetActive(false);
                index++;
                continue;
            }
            rankingItems[index].SetRankText(rank.Key, rank.Value);
            rankingItems[index].gameObject.SetActive(true);
            index++;
        }
    }
}