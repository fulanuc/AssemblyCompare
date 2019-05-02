using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000609 RID: 1545
	public class LocalUserSignInController : MonoBehaviour
	{
		// Token: 0x060022D1 RID: 8913 RVA: 0x000A6EDC File Offset: 0x000A50DC
		private void Start()
		{
			LocalUserSignInCardController[] componentsInChildren = base.GetComponentsInChildren<LocalUserSignInCardController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.cards.Add(componentsInChildren[i]);
			}
		}

		// Token: 0x060022D2 RID: 8914 RVA: 0x00019652 File Offset: 0x00017852
		public bool AreAllCardsReady()
		{
			return this.cards.Any((LocalUserSignInCardController v) => v.rewiredPlayer != null && v.requestedUserProfile == null);
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x000A6F0C File Offset: 0x000A510C
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

		// Token: 0x060022D4 RID: 8916 RVA: 0x000A6FC0 File Offset: 0x000A51C0
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

		// Token: 0x060022D5 RID: 8917 RVA: 0x000A7008 File Offset: 0x000A5208
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

		// Token: 0x060022D6 RID: 8918 RVA: 0x000A704C File Offset: 0x000A524C
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

		// Token: 0x060022D7 RID: 8919 RVA: 0x000A716C File Offset: 0x000A536C
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

		// Token: 0x040025A9 RID: 9641
		public GameObject localUserCardPrefab;

		// Token: 0x040025AA RID: 9642
		private readonly List<LocalUserSignInCardController> cards = new List<LocalUserSignInCardController>();
	}
}
