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
	// Token: 0x02000409 RID: 1033
	public class MPEventSystemManager : MonoBehaviour
	{
		// Token: 0x1700021A RID: 538
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000113FA File Offset: 0x0000F5FA
		// (set) Token: 0x0600170B RID: 5899 RVA: 0x00011401 File Offset: 0x0000F601
		public static MPEventSystem combinedEventSystem { get; private set; }

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600170C RID: 5900 RVA: 0x00011409 File Offset: 0x0000F609
		// (set) Token: 0x0600170D RID: 5901 RVA: 0x00011410 File Offset: 0x0000F610
		public static MPEventSystem kbmEventSystem { get; private set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600170E RID: 5902 RVA: 0x00011418 File Offset: 0x0000F618
		// (set) Token: 0x0600170F RID: 5903 RVA: 0x0001141F File Offset: 0x0000F61F
		public static MPEventSystem primaryEventSystem { get; private set; }

		// Token: 0x06001710 RID: 5904 RVA: 0x00079310 File Offset: 0x00077510
		public static MPEventSystem FindEventSystem(Player inputPlayer)
		{
			MPEventSystem result;
			MPEventSystemManager.eventSystems.TryGetValue(inputPlayer.id, out result);
			return result;
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x00079334 File Offset: 0x00077534
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

		// Token: 0x06001712 RID: 5906 RVA: 0x0007942C File Offset: 0x0007762C
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

		// Token: 0x06001713 RID: 5907 RVA: 0x00011427 File Offset: 0x0000F627
		static MPEventSystemManager()
		{
			LocalUserManager.onLocalUsersUpdated += MPEventSystemManager.RefreshEventSystems;
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(MPEventSystemManager.Initialize));
		}

		// Token: 0x04001A33 RID: 6707
		private static readonly Dictionary<int, MPEventSystem> eventSystems = new Dictionary<int, MPEventSystem>();

		// Token: 0x04001A37 RID: 6711
		public static ResourceAvailability availability;
	}
}
