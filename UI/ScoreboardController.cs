using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063D RID: 1597
	public class ScoreboardController : MonoBehaviour
	{
		// Token: 0x0600240B RID: 9227 RVA: 0x0001A4CC File Offset: 0x000186CC
		private void Awake()
		{
			this.stripAllocator = new UIElementAllocator<ScoreboardStrip>(this.container, this.stripPrefab);
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x0001A4E5 File Offset: 0x000186E5
		private void SetStripCount(int newCount)
		{
			this.stripAllocator.AllocateElements(newCount);
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x000AB354 File Offset: 0x000A9554
		private void Rebuild()
		{
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			int count = instances.Count;
			this.SetStripCount(count);
			for (int i = 0; i < count; i++)
			{
				this.stripAllocator.elements[i].SetMaster(instances[i].master);
			}
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x0001A4F3 File Offset: 0x000186F3
		private void PlayerEventToRebuild(PlayerCharacterMasterController playerCharacterMasterController)
		{
			this.Rebuild();
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x0001A4FB File Offset: 0x000186FB
		private void OnEnable()
		{
			PlayerCharacterMasterController.onPlayerAdded += this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerRemoved += this.PlayerEventToRebuild;
			this.Rebuild();
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x0001A525 File Offset: 0x00018725
		private void OnDisable()
		{
			PlayerCharacterMasterController.onPlayerRemoved -= this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerAdded -= this.PlayerEventToRebuild;
		}

		// Token: 0x040026BB RID: 9915
		public GameObject stripPrefab;

		// Token: 0x040026BC RID: 9916
		public RectTransform container;

		// Token: 0x040026BD RID: 9917
		private UIElementAllocator<ScoreboardStrip> stripAllocator;
	}
}
