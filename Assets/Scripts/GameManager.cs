using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Start,
        WaitingRound,
        Playing,
        GameOver
    }

    [Serializable]
    public struct Round
    {
        public List<Combination> combinations;
    }

    [Serializable]
    public struct Combination
    {
        public List<int> houses;
    }
    
    public int numberOfRounds;
    public float secondsPerRound;

    public RectTransform startPanel;
    public TextMeshProUGUI titleText;
    public RectTransform finishPanel;
    public TextMeshProUGUI playerWonText;
    public RectTransform roundPanel;
    public TextMeshProUGUI roundMessage;
    public RectTransform timerPanel;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI combinationText;

    public List<Round> rounds;
    
    public List<GameObject> houses;

    public List<KeyCode> player1Keycodes;
    public List<KeyCode> player2Keycodes;

    [HideInInspector]
    public GameState gameState;

    private int currentRound;
    private float currentCountdown;
    private int player1Wins, player2Wins;
    private Combination currentCombination;

    protected void Start()
    {
        ShowStartPanel();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (gameState)
            {
                case GameState.Start:
                    HideStartPanel();
                    break;
                case GameState.GameOver:
                    HideGameOverPanel();
                    ShowStartPanel();
                    break;
                case GameState.WaitingRound:
                    HideRoundPanel();
                    break;
            }
        }

        if (gameState == GameState.Playing)
        {
            currentCountdown -= Time.deltaTime;
            timer.text = ((int) currentCountdown).ToString();

            if (currentCountdown <= 0)
            {
                gameState = GameState.WaitingRound;
                CheckRounds();
            }
            
            CheckCombinations();
        }
    }

    private void CheckCombinations()
    {
        int player1matches = 0;
        for (int i = 0; i < currentCombination.houses.Count; i++)
        {
            if (Input.GetKey(player1Keycodes[currentCombination.houses[i]]))
            {
                player1matches++;
            }
        }
        
        int player2matches = 0;
        for (int i = 0; i < currentCombination.houses.Count; i++)
        {
            if (Input.GetKey(player2Keycodes[currentCombination.houses[i]]))
            {
                player2matches++;
            }
        }

        if (player1matches == currentCombination.houses.Count)
        {
            CheckRounds(1);
        }
        else if (player2matches == currentCombination.houses.Count)
        {
            CheckRounds(2);
        }
    }

    private void CheckRounds(int winPlayer = 0)
    {
        foreach (var house in houses)
        {
            house.GetComponent<SpriteRenderer>().color = Color.black;
        }
        
        if (winPlayer == 1)
        {
            player1Wins++;
        }else if (winPlayer == 2)
        {
            player2Wins++;
        }
        
        if (currentRound >= numberOfRounds - 1)
        {
            if (player1Wins > player2Wins)
            {
                ShowGameOverPanel(1);
            }
            else if (player2Wins > player1Wins)
            {
                ShowGameOverPanel(2);
            }
            else
            {
                ShowGameOverPanel();
            }
        }
        else
        {
            ShowRoundPanel(winPlayer);
        }
    }

    private void ShowStartPanel()
    {
        gameState = GameState.Start;
        startPanel.GetComponent<CanvasGroup>().alpha = 1f;
        titleText.transform.DOMove(titleText.transform.position + Vector3.up * 10f, 2f).SetLoops(-1, LoopType.Yoyo);
        currentRound = 0;
        currentCountdown = secondsPerRound;
        player1Wins = 0;
        player2Wins = 0;
    }

    private void HideStartPanel()
    {
        gameState = GameState.Playing;
        SelectCombinationForRound();
        startPanel.GetComponent<CanvasGroup>().alpha = 0f;
        timerPanel.GetComponent<CanvasGroup>().alpha = 1f;
        titleText.transform.DOKill();
    }
    
    private void ShowGameOverPanel(int winPlayer = 0)
    {
        finishPanel.GetComponent<CanvasGroup>().alpha = 1f;
        timerPanel.GetComponent<CanvasGroup>().alpha = 0f;
        gameState = GameState.GameOver;
        playerWonText.transform.DOMove(titleText.transform.position + Vector3.up * 10f, 2f).SetLoops(-1, LoopType.Yoyo);
        
        switch (winPlayer)
        {
            case 0:
                playerWonText.text = "It's a tie! :(";
                break;
            case 1:
                playerWonText.text = "Player 1 Won!";
                break;
            case 2:
                playerWonText.text = "Player 2 Won!";
                break;
        }
    }

    private void HideGameOverPanel()
    {
        finishPanel.GetComponent<CanvasGroup>().alpha = 0f;
        playerWonText.transform.DOKill();
    }

    private void ShowRoundPanel(int winPlayer = 0)
    {
        roundPanel.GetComponent<CanvasGroup>().alpha = 1f;
        timerPanel.GetComponent<CanvasGroup>().alpha = 0f;
        gameState = GameState.WaitingRound;
        switch (winPlayer)
        {
            case 0:
                roundMessage.text = "It's a tie! :(";
                break;
            case 1:
                roundMessage.text = "Player 1 won the round!";
                break;
            case 2:
                roundMessage.text = "Player 2 won the round!";
                break;
        }
    }

    private void HideRoundPanel()
    {
        roundPanel.GetComponent<CanvasGroup>().alpha = 0f;
        currentRound++;
        SelectCombinationForRound();
        timerPanel.GetComponent<CanvasGroup>().alpha = 1f;
        gameState = GameState.Playing;
    }

    private void SelectCombinationForRound()
    {
        var selectedRound = rounds[currentRound];
        currentCombination = selectedRound.combinations[UnityEngine.Random.Range(0, selectedRound.combinations.Count)];
        var combiText = "Connect houses: ";
        foreach (var house in currentCombination.houses)
        {
            combiText += " " + house + 1;
            houses[house].GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        combinationText.text = combiText;
    }
}
