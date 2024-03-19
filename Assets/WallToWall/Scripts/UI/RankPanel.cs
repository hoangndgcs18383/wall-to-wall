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

        List<int> rankSet = new List<int>();
        foreach (var rank in rankList)
        {
            if (rank.Value != -1)
            {
                if (!rankSet.Contains(rank.Value))
                {
                    rankSet.Add(rank.Value);
                }
            }
        }

        noRanking.SetActive(rankSet.Count == 0);
        for (int i = 0; i < rankSet.Count; i++)
        {
            rankingItems[i].SetRankText((i + 1).ToString(), rankSet[i]);
            rankingItems[i].gameObject.SetActive(true);
        }

        /*int index = 0;

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
        }*/
    }
}