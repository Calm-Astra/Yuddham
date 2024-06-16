using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yuddham
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Yuddham/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Card graphics")]
        public Sprite cardImage;

        [Header("List of Placeables")]
        public PlaceableData[] placeablesData; //link to all the Placeables that this card spawns
        public Vector3[] relativeOffsets; //the relative offsets (from cursor) where the placeables will be dropped
    }
    
}