using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ClipInfo
{
	public AudioClip clip;
	public float destroySpeed;
	public int transitionPlatform;
	public MusicTransitionType transitionType;
	public int order;
}
