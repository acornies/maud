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
		public ControlMode controlMode { get; set; }
		public bool playMusic { get; set; }
    }
}