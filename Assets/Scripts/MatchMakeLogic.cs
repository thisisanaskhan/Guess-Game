using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;

// Type of Card Matching Condition
public enum MatchType
{
    Match,
    Mismatch
}

public class MatchMakeLogic : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private List<CanvasGroup> allCards = new List<CanvasGroup>();
    [SerializeField] private int totalPairs = 8; // Total pairs to match (configurable in Inspector)
    [SerializeField] private float showCardDuration = 1f; // Duration to show cards at start
    [SerializeField] private float matchAnimationDuration = 0.5f; // Animation duration for
    [SerializeField] private GameObject startPanel; // This panel is only to prevent from click card in very starting  

    [Header("Audio Settings")]
    [SerializeField] private AudioSource correctMatchSound; // Audio for correct Card Match
    [SerializeField] private AudioSource inCorrectMatchSound; // Audio for incorrect Card match
    [SerializeField] private AudioSource cardFlipSound; // Audio for Card Flip
    [SerializeField] private AudioSource gameOverSound; // Audio for GameOver
    

    [Header("Game State")]
    private int clickCount; // Click Count, After 2 click rested to 0
    private int firstCardMatchID; // After Clicking Card Getting ID of Card
    private int secondCardMatchID; // After Clicking Card Getting ID of Card
    private int matchesCount; // Count of how many Card Matches
    private int turnCount; // Count of how many turns
    private GameObject clickedObject = null; // Current Clicked Object Reference
    private CanvasGroup firstCardPressedRef = null; // After Clicking on Card Getting Card reference to perform Animation on card
    private CanvasGroup secondCardPressRef = null; // After Clicking on Card Getting Card reference to perform Animation on card
    private MatchType matchResult;
    private UIManager uiManager; // Cached UIManager reference

    private void Start()
    {
        InitializeGame();
        uiManager = UIManager.instance;
    }

    /// <summary>
    /// Initializes the game state and starts the card reveal animation.
    /// </summary>
    private void InitializeGame()
    {
        clickCount = 0;
        firstCardMatchID = 0;
        secondCardMatchID = 0;
        matchesCount = 0;
        turnCount = 0;
        firstCardPressedRef = null;
        secondCardPressRef = null;

        StartCoroutine(ShowCard(false, 1));
    }

    /// <summary>
    /// Shows or hides all cards with a delay.
    /// </summary>
    private IEnumerator ShowCard(bool status, float time)
    { 
        yield return new WaitForSeconds(time);
        for (int i = 0; i < allCards.Count; i++)
        {
            allCards[i].transform.GetChild(0).gameObject.SetActive(status);
        }
        SetAllCardsInteractable(false);
    }

    /// <summary>
    /// All Card Interactable Disable in Start
    /// </summary>
    private void SetAllCardsInteractable(bool interactable)
    {
        startPanel.SetActive(interactable);
    }


    /// <summary>
    /// Handles card click events, triggered by UI buttons.
    /// </summary>
    public void OnClickCard()
    {
        clickedObject = EventSystem.current.currentSelectedGameObject;
        var canvasGroup = clickedObject.GetComponent<CanvasGroup>();
        var idHolder = clickedObject.GetComponent<IDHolder>();
        var button = clickedObject.GetComponent<UnityEngine.UI.Button>();
        var cardFace = clickedObject.transform.GetChild(0);

        clickCount++;
        cardFlipSound.Play();

        if (clickCount == 1)
        { 
            
            if(clickedObject != null)
            {
                firstCardMatchID = idHolder.MatchingID;
                firstCardPressedRef = canvasGroup;
                cardFace.gameObject.SetActive(true);

                // Preventing from double click
                button.interactable = false;

            }
        }
        else if (clickCount == 2)
        { 
            
            if (clickedObject != null)
            {

                secondCardMatchID = idHolder.MatchingID;
                secondCardPressRef = canvasGroup;
                cardFace.gameObject.SetActive(true);
                button.interactable = false;
            }
            StartCoroutine(ProcessTurn());
        }
    }


    /// <summary>
    /// Processes a turn after two cards are clicked, handling match or mismatch logic.
    /// </summary>
    IEnumerator ProcessTurn()
    {
        turnCount++;
        uiManager.UpdateTurnsUI(turnCount);

        matchResult = (firstCardMatchID == secondCardMatchID) ? MatchType.Match : MatchType.Mismatch;

        if (matchResult == MatchType.Match)
        {
            matchesCount++;
            uiManager.UpdateMatchesUI(matchesCount);

            yield return new WaitForSeconds(0.2f);
            // Play Sound for Card Match
            correctMatchSound.Play();
            // Card Animation
            PlayShakeAnimation(firstCardPressedRef, secondCardPressRef);

            yield return new WaitForSeconds(1);
            // Hide Card Match Sound
            firstCardPressedRef.alpha = 0;
            secondCardPressRef.alpha = 0;

            CheckGameOver();
        }
        else if (matchResult == MatchType.Mismatch)
        {
            yield return new WaitForSeconds(0.2f);
            // Play Sound for Card MisMatch
            inCorrectMatchSound.Play();
            // Card Animation
            PlayShakeAnimation(firstCardPressedRef, secondCardPressRef);

            yield return new WaitForSeconds(1);
            // Flip MisMatch Card Again 
            firstCardPressedRef.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            secondCardPressRef.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            // Resrt Card to Clickable Again
            firstCardPressedRef.GetComponent<UnityEngine.UI.Button>().interactable = true;
            secondCardPressRef.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }

        
        ResetTurn();
    }

    /// <summary>
    /// Animation for Card Match and Mismatch.
    /// </summary>
    private void PlayShakeAnimation(CanvasGroup card1, CanvasGroup card2)
    {
        card1.transform.DOShakeScale(showCardDuration, matchAnimationDuration).SetEase(Ease.OutExpo);
        card2.transform.DOShakeScale(showCardDuration, matchAnimationDuration).SetEase(Ease.OutExpo);
    }

    /// <summary>
    /// Resets the turn state after processing a match or mismatch.
    /// </summary>
    private void ResetTurn()
    {
        clickCount = 0;
        firstCardPressedRef = null;
        secondCardPressRef = null;
    }


    /// <summary>
    /// Checks if the game is over and triggers the win condition.
    /// </summary>
    private void CheckGameOver()
    {
        if (matchesCount >= totalPairs)
        {
            uiManager.ShowGameOverScreen();
            gameOverSound.Play();
        }
    }
}