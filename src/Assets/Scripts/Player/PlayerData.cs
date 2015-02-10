﻿using System;
using UnityEngine;
using System.Collections;

namespace LegendPeak.Player
{
    [Serializable]
    public class PlayerData
    {
        public int highestPlatform { get; set; }
        public int totalPlatforms { get; set; }
        public MonetizedState monetizedState { get; set; }
		public ControlMode controlMode { get; set; }
		public bool playMusic { get; set; }
		public int playerLevel { get; set; }
		public int gameOverContinues { get; set;}
		public bool viewedTutorialGeneral {get; set;}
		public bool purchasedAlternateMusic { get; set; }
		public int soundTrackSelection { get; set; }
    }
}