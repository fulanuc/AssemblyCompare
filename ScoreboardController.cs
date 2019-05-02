using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200062B RID: 1579
	public class ScoreboardController : MonoBehaviour
	{
		// Token: 0x0600237B RID: 9083 RVA: 0x00019DFE File Offset: 0x00017FFE
		private void Awake()
		{
			this.stripAllocator = new UIElementAllocator<ScoreboardStrip>(this.container, this.stripPrefab);
		}

		// Token: 0x0600237C RID: 9084 RVA: 0x00019E17 File Offset: 0x00018017
		private void SetStripCount(int newCount)
		{
			this.stripAllocator.AllocateElements(newCount);
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x000A9CD8 File Offset: 0x000A7ED8
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

		// Token: 0x0600237E RID: 9086 RVA: 0x00019E25 File Offset: 0x00018025
		private void PlayerEventToRebuild(PlayerCharacterMasterController playerCharacterMasterController)
		{
			this.Rebuild();
		}

		// Token: 0x0600237F RID: 9087 RVA: 0x00019E2D File Offset: 0x0001802D
		private void OnEnable()
		{
			PlayerCharacterMasterController.onPlayerAdded += this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerRemoved += this.PlayerEventToRebuild;
			this.Rebuild();
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x00019E57 File Offset: 0x00018057
		private void OnDisable()
		{
			PlayerCharacterMasterController.onPlayerRemoved -= this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerAdded -= this.PlayerEventToRebuild;
		}

		// Token: 0x04002660 RID: 9824
		public GameObject stripPrefab;

		// Token: 0x04002661 RID: 9825
		public RectTransform container;

		// Token: 0x04002662 RID: 9826
		private UIElementAllocator<ScoreboardStrip> stripAllocator;
	}
}
