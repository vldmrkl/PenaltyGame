using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShotResult
{
    Pending,
    Scored,
    Missed
}

public class PenaltyDotsUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform playerADotsContainer;
    public Transform playerBDotsContainer;
    public TextMeshProUGUI playerAScoreText;
    public TextMeshProUGUI playerBScoreText;
    public Image dotPrefab;

    [Header("Config")]
    public int shotsPerPlayer = 5;

    [Header("Colors")]
    public Color pendingColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    public Color scoredColor  = new Color(0.20f, 0.85f, 0.35f, 1f);
    public Color missedColor  = new Color(0.90f, 0.25f, 0.25f, 1f);

    private readonly List<Image> aDots = new();
    private readonly List<Image> bDots = new();

    public void Init()
    {
        BuildDots(playerADotsContainer, aDots);
        BuildDots(playerBDotsContainer, bDots);
        SetAllPending();
    }

    private void BuildDots(Transform container, List<Image> list)
    {
        // Clear existing children if any
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        list.Clear();

        for (int i = 0; i < shotsPerPlayer; i++)
        {
            Image dot = Instantiate(dotPrefab, container);
            dot.color = pendingColor;
            list.Add(dot);
        }
    }

    public void SetScore(int a, int b)
    {
        if (playerAScoreText) playerAScoreText.text = a.ToString();
        if (playerBScoreText) playerBScoreText.text = b.ToString();
    }

    public void SetShotResult(int playerIndex, int shotIndex, ShotResult result)
    {
        var dots = playerIndex == 0 ? aDots : bDots;
        if (shotIndex < 0 || shotIndex >= dots.Count) return;

        dots[shotIndex].color = result switch
        {
            ShotResult.Scored => scoredColor,
            ShotResult.Missed => missedColor,
            _ => pendingColor
        };
    }

    public void SetAllPending()
    {
        for (int i = 0; i < aDots.Count; i++) aDots[i].color = pendingColor;
        for (int i = 0; i < bDots.Count; i++) bDots[i].color = pendingColor;

        SetScore(0, 0);
    }
}