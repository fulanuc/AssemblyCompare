using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000403 RID: 1027
	public class TemporaryVisualEffect : MonoBehaviour
	{
		// Token: 0x06001710 RID: 5904 RVA: 0x000115EA File Offset: 0x0000F7EA
		private void Start()
		{
			this.RebuildVisualComponents();
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x000115F2 File Offset: 0x0000F7F2
		private void FixedUpdate()
		{
			if (this.previousVisualState != this.visualState)
			{
				this.RebuildVisualComponents();
			}
			this.previousVisualState = this.visualState;
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x00078A80 File Offset: 0x00076C80
		private void RebuildVisualComponents()
		{
			TemporaryVisualEffect.VisualState visualState = this.visualState;
			MonoBehaviour[] array;
			if (visualState == TemporaryVisualEffect.VisualState.Enter)
			{
				array = this.enterComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				array = this.exitComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				return;
			}
			if (visualState != TemporaryVisualEffect.VisualState.Exit)
			{
				return;
			}
			array = this.enterComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			array = this.exitComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x00078B18 File Offset: 0x00076D18
		private void LateUpdate()
		{
			bool flag = this.healthComponent;
			if (this.parentTransform)
			{
				base.transform.position = this.parentTransform.position;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (!flag || (flag && !this.healthComponent.alive))
			{
				this.visualState = TemporaryVisualEffect.VisualState.Exit;
			}
			if (this.visualTransform)
			{
				this.visualTransform.localScale = new Vector3(this.radius, this.radius, this.radius);
			}
		}

		// Token: 0x04001A09 RID: 6665
		public float radius = 1f;

		// Token: 0x04001A0A RID: 6666
		public Transform parentTransform;

		// Token: 0x04001A0B RID: 6667
		public Transform visualTransform;

		// Token: 0x04001A0C RID: 6668
		public MonoBehaviour[] enterComponents;

		// Token: 0x04001A0D RID: 6669
		public MonoBehaviour[] exitComponents;

		// Token: 0x04001A0E RID: 6670
		public TemporaryVisualEffect.VisualState visualState;

		// Token: 0x04001A0F RID: 6671
		private TemporaryVisualEffect.VisualState previousVisualState;

		// Token: 0x04001A10 RID: 6672
		[HideInInspector]
		public HealthComponent healthComponent;

		// Token: 0x02000404 RID: 1028
		public enum VisualState
		{
			// Token: 0x04001A12 RID: 6674
			Enter,
			// Token: 0x04001A13 RID: 6675
			Exit
		}
	}
}
