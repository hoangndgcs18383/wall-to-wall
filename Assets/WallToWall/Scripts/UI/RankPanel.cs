using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class RankPanel : BaseScreen
{
    [SerializeField] private List<RankingItem> rankingItems;
    [SerializeField] private GameObject noRanking;

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        SetRanking();
    }

    [Button("SetRanking")]
    public void SetRanking()
    {
        RankManager.Instance.SortRank();
        var rankList = RankManager.Instance.GetRankList;

        int index = 0;

        noRanking.SetActive(rankList.Any(pair => pair.Value != -1) == false);

        foreach (var rank in rankList)
        {
            //Debug.Log($"Rank: {rank.Key} - {rank.Value}");

            if (rankingItems.Count <= index) break;
            if (rank.Value == -1)
            {
                rankingItems[index].gameObject.SetActive(false);
                index++;
                continue;
            }

            
            rankingItems[index].SetRankText((index + 1).ToString(), rank.Value);
            rankingItems[index].gameObject.SetActive(true);
            index++;
        }
    }
}