using System;
using UnityEngine;
using System.Collections;

namespace LegendPeak.Player
{
	[Serializable]
	public class PlayerLevel 
	{
		public int totalPlatforms;
		public float maxEnergy;
		public float rotationCostPerSecond;
		public float stabilizeCost;
		public float stabilizeTime;
		public float deathCost;
	}
}
