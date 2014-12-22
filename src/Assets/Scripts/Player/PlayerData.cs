using System;
using Assets.Scripts.GameState;
using UnityEngine;
using System.Collections;

namespace LegendPeak.Player
{
    [Serializable]
    public class PlayerData
    {
        public int highestPoint { get; set; }
        public int totalHeight { get; set; }
        public MonetizedState monetizedState { get; set; }
    }
}
