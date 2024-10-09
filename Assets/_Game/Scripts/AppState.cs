using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerProfileSystem;

public static class AppState
{
	public static PlayerInfo HumanPlayerInfo { get; set; }
	public static GameConfig GameConfig { get; set; }
	public static TimelineConfig Timeline { get; set; }
	public static void EnsureAppState()
	{

	}
}
