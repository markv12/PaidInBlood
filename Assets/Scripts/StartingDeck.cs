using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingDeck : ScriptableObject
{
    [Reorderable]
    public CardData[] masterList;
}
