using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Rewired;
using Rewired.Integration.UnityUI;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200040F RID: 1039
	public class MPEventSystemManager : MonoBehaviour
	{
		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600174D RID: 5965 RVA: 0x00011826 File Offset: 0x0000FA26
		// (set) Token: 0x0600174E RID: 5966 RVA: 0x0001182D File Offset: 0x0000FA2D
		public static MPEventSystem combinedEventSystem { get; private set; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x00011835 File Offset: 0x0000FA35
		// (set) Token: 0x06001750 RID: 5968 RVA: 0x0001183C File Offset: 0x0000FA3C
		public static MPEventSystem kbmEventSystem { get; private set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x00011844 File Offset: 0x0000FA44
		// (set) Token: 0x06001752 RID: 5970 RVA: 0x0001184B File Offset: 0x0000FA4B
		public static MPEventSystem primaryEventSystem { get; private set; }

		// Token: 0x06001753 RID: 5971 RVA: 0x000798D0 File Offset: 0x00077AD0
		public static MPEventSystem FindEventSystem(Player inputPlayer)
		{
			MPEventSystem result;
			MPEventSystemManager.eventSystems.TryGetValue(inputPlayer.id, out result);
			return result;
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x000798F4 File Offset: 0x00077AF4
		private static void Initialize()
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/UI/MPEventSystem");
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, RoR2Application.instance.transform);
				gameObject.name = string.Format(CultureInfo.InvariantCulture, "MPEventSystem Player{0}", i);
				MPEventSystem component = gameObject.GetComponent<MPEventSystem>();
				RewiredStandaloneInputModule component2 = gameObject.GetComponent<RewiredStandaloneInputModule>();
				Player player = players[i];
				component2.RewiredPlayerIds = new int[]
				{
					player.id
				};
				gameObject.GetComponent<MPInput>().player = player;
				if (i == 1)
				{
					MPEventSystemManager.kbmEventSystem = component;
					component.allowCursorPush = false;
				}
				component.player = players[i];
				MPEventSystemManager.eventSystems[players[i].id] = component;
			}
			MPEventSystemManager.combinedEventSystem = MPEventSystemManager.eventSystems[0];
			MPEventSystemManager.combinedEventSystem.isCombinedEventSystem = true;
			MPEventSystemManager.RefreshEventSystems();
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x000799EC File Offset: 0x00077BEC
		private static void RefreshEventSystems()
		{
			int count = LocalUserManager.readOnlyLocalUsersList.Count;
			ReadOnlyCollection<MPEventSystem> readOnlyInstancesList = MPEventSystem.readOnlyInstancesList;
			readOnlyInstancesList[0].enabled = (count <= 1);
			for (int i = 1; i < readOnlyInstancesList.Count; i++)
			{
				readOnlyInstancesList[i].enabled = (readOnlyInstancesList[i].localUser != null);
			}
			int num = 0;
			for (int j = 0; j < readOnlyInstancesList.Count; j++)
			{
				MPEventSystem mpeventSystem = readOnlyInstancesList[j];
				int playerSlot;
				if (!readOnlyInstancesList[j].enabled)
				{
					playerSlot = -1;
				}
				else
				{
					num = (playerSlot = num) + 1;
				}
				mpeventSystem.playerSlot = playerSlot;
			}
			MPEventSystemManager.primaryEventSystem = ((count > 0) ? LocalUserManager.readOnlyLocalUsersList[0].eventSystem : MPEventSystemManager.combinedEventSystem);
			MPEventSystemManager.availability.MakeAvailable();
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x00011853 File Offset: 0x0000FA53
		static MPEventSystemManager()
		{
			LocalUserManager.onLocalUsersUpdated += MPEventSystemManager.RefreshEventSystems;
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(MPEventSystemManager.Initialize));
		}

		// Token: 0x04001A5C RID: 6748
		private static readonly Dictionary<int, MPEventSystem> eventSystems = new Dictionary<int, MPEventSystem>();

		// Token: 0x04001A60 RID: 6752
		public static ResourceAvailability availability;
	}
}
