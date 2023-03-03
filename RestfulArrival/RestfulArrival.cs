﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace RestfulArrival
{
	// Keep the version up-to-date with AssemblyInfo.cs, manifest.json, and README.md!
	[BepInPlugin( "com.riintouge.restfularrival" , "Restful Arrival" , "1.0.0" )]
	[BepInProcess( "valheim.exe" )]
	public partial class RestfulArrival : BaseUnityPlugin
	{
		// 0 - Core
		public static ConfigEntry< bool > IsEnabled;
		public static ConfigEntry< bool > LoadOnStart;

		private readonly Harmony Harmony = new Harmony( "com.riintouge.restfularrival" );

		private void Awake()
		{
			IsEnabled = Config.Bind(
				"0 - Core",
				"Enable",
				true,
				"Determines if this mod has any effect when loaded." );

			LoadOnStart = Config.Bind(
				"0 - Core",
				"LoadOnStart",
				true,
				"Determines if this mod loads on game start." );

			if( LoadOnStart.Value )
				Harmony.PatchAll();
		}
	}
}
