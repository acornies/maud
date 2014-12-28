using System;
using Assets.Scripts.GameState;
using UnityEngine;
using System.Collections;

namespace LegendPeak.Player
{
    [Serializable]
    public class PlayerData
    {
        public float highestPoint { get; set; }
        public float totalHeight { get; set; }
        public MonetizedState monetizedState { get; set; }
    }
}