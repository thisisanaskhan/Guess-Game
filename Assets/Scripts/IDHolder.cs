using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDHolder : MonoBehaviour
{
    [SerializeField] private int _matchingID;
    [SerializeField] private int _cardID;

    public int MatchingID
    {
        get => _matchingID;
        private set => _matchingID = value;
    }

    public int CardID
    {
        get => _cardID;
        private set => _cardID = value;
    }

    
}
