using TMPro;
using UnityEngine;

public class RankingItem : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text pointText;

    public void SetRankText(string value, int point)
    {
        rankText.SetText($"{value}\t Player{Random.Range(0, 100)}");
        pointText.SetText(point.ToString());
    }
}