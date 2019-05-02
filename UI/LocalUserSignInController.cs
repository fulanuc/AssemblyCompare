using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F7 RID: 1527
	public class LocalUserSignInController : MonoBehaviour
	{
		// Token: 0x06002241 RID: 8769 RVA: 0x000A5860 File Offset: 0x000A3A60
		private void Start()
		{
			LocalUserSignInCardController[] componentsInChildren = base.GetComponentsInChildren<LocalUserSignInCardController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.cards.Add(componentsInChildren[i]);
			}
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x00018FA5 File Offset: 0x000171A5
		public bool AreAllCardsReady()
		{
			return this.cards.Any((LocalUserSignInCardController v) => v.rewiredPlayer != null && v.requestedUserProfile == null);
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x000A5890 File Offset: 0x000A3A90
		private void DoSignIn()
		{
			LocalUserManager.LocalUserInitializationInfo[] array = new LocalUserManager.LocalUserInitializationInfo[this.cards.Count((LocalUserSignInCardController v) => v.rewiredPlayer != null)];
			int index = 0;
			for (int i = 0; i < this.cards.Count; i++)
			{
				if (this.cards[i].rewiredPlayer != null)
				{
					array[index++] = new LocalUserManager.LocalUserInitializationInfo
					{
						player = this.cards[index].rewiredPlayer,
						profile = this.cards[index].requestedUserProfile
					};
				}
			}
			LocalUserManager.SetLocalUsers(array);
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x000A5944 File Offset: 0x000A3B44
		private LocalUserSignInCardController FindCardAssociatedWithRewiredPlayer(Player rewiredPlayer)
		{
			for (int i = 0; i < this.cards.Count; i++)
			{
				if (this.cards[i].rewiredPlayer == rewiredPlayer)
				{
					return this.cards[i];
				}
			}
			return null;
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x000A598C File Offset: 0x000A3B8C
		private LocalUserSignInCardController FindCardWithoutRewiredPlayer()
		{
			for (int i = 0; i < this.cards.Count; i++)
			{
				if (this.cards[i].rewiredPlayer == null)
				{
					return this.cards[i];
				}
			}
			return null;
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x000A59D0 File Offset: 0x000A3BD0
		private void Update()
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (!(player.name == "PlayerMain"))
				{
					LocalUserSignInCardController localUserSignInCardController = this.FindCardAssociatedWithRewiredPlayer(player);
					if (localUserSignInCardController == null)
					{
						if (player.GetButtonDown("Start"))
						{
							LocalUserSignInCardController localUserSignInCardController2 = this.FindCardWithoutRewiredPlayer();
							if (localUserSignInCardController2 != null)
							{
								localUserSignInCardController2.rewiredPlayer = player;
							}
						}
					}
					else if (player.GetButtonDown("UICancel") || !LocalUserSignInController.PlayerHasControllerConnected(player))
					{
						localUserSignInCardController.rewiredPlayer = null;
					}
				}
			}
			ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.readOnlyLocalUsersList;
			int num = 4;
			while (this.cards.Count < num)
			{
				this.cards.Add(UnityEngine.Object.Instantiate<GameObject>(this.localUserCardPrefab, base.transform).GetComponent<LocalUserSignInCardController>());
			}
			while (this.cards.Count > num)
			{
				UnityEngine.Object.Destroy(this.cards[this.cards.Count - 1].gameObject);
				this.cards.RemoveAt(this.cards.Count - 1);
			}
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x000A5AF0 File Offset: 0x000A3CF0
		private static bool PlayerHasControllerConnected(Player player)
		{
			using (IEnumerator<Controller> enumerator = player.controllers.Controllers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Controller controller = enumerator.Current;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400254E RID: 9550
		public GameObject localUserCardPrefab;

		// Token: 0x0400254F RID: 9551
		private readonly List<LocalUserSignInCardController> cards = new List<LocalUserSignInCardController>();
	}
}
