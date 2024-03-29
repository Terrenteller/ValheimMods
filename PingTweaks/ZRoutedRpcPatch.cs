﻿using HarmonyLib;
using UnityEngine;

namespace PingTweaks
{
	public partial class PingTweaks
	{
		[HarmonyPatch( typeof( ZRoutedRpc ) )]
		private class ZRoutedRpcPatch
		{
			[HarmonyPatch( "InvokeRoutedRPC" , new[] { typeof( long ) , typeof( string ) , typeof( object[] ) } )]
			[HarmonyPrefix]
			private static bool InvokeRoutedRPCPrefix( ref string methodName , ref object[] parameters )
			{
				if( !IsEnabled.Value || methodName.CompareTo( "ChatMessage" ) != 0 )
					return true;

				int? typeRaw = parameters[ 1 ] as int?;
				if( !typeRaw.HasValue || (Talker.Type)typeRaw.Value != Talker.Type.Ping )
					return true;

				Vector3? position = parameters[ 0 ] as Vector3?;
				UserInfo userInfo = parameters[ 2 ] as UserInfo;
				string text = parameters[ 3 ] as string;
				string senderAccountId = parameters[ 4 ] as string;

				if( ShowMapMarkerTextWhenPinged.Value )
				{
					Minimap minimap = Minimap.instance;
					float zoomFactor = Traverse.Create( minimap )
						.Field( "m_largeZoom" )
						.GetValue< float >();
					float radius = minimap.m_removeRadius * zoomFactor * 2.0f;
					Minimap.PinData pinData = Traverse.Create( minimap )
						.Method( "GetClosestPin" , new[] { typeof( Vector3 ) , typeof( float ) } )
						.GetValue< Minimap.PinData >( position , radius );

					if( pinData != null )
					{
						parameters[ 3 ] = pinData.m_name; // Unlocalized
						text = pinData.m_NamePinData.PinNameText.text; // Localized
					}
				}

				if( PingBroadcastModifier.Value != KeyCode.None
					&& !Input.GetKey( PingBroadcastModifier.Value )
					&& position.HasValue
					&& userInfo != null
					&& text != null
					&& senderAccountId != null )
				{
					Chat.instance.OnNewChatMessage(
						null,
						ZNet.instance.GetUID(),
						position.Value,
						Talker.Type.Ping,
						userInfo,
						text,
						senderAccountId );

					return false;
				}

				return true;
			}
		}
	}
}
