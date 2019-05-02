using System;
using System.Text;
using EntityStates.TimedChest;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000407 RID: 1031
	public sealed class TimedChestController : NetworkBehaviour, IInteractable
	{
		// Token: 0x0600171B RID: 5915 RVA: 0x0001165C File Offset: 0x0000F85C
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextString);
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x00011669 File Offset: 0x0000F869
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.purchased)
			{
				return Interactability.Disabled;
			}
			if (!this.locked)
			{
				return Interactability.Available;
			}
			return Interactability.ConditionsNotMet;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x00011680 File Offset: 0x0000F880
		public void OnInteractionBegin(Interactor activator)
		{
			base.GetComponent<EntityStateMachine>().SetNextState(new Opening());
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x0600171F RID: 5919 RVA: 0x00078C60 File Offset: 0x00076E60
		private int remainingTime
		{
			get
			{
				float num = 0f;
				if (Run.instance)
				{
					num = Run.instance.GetRunStopwatch();
				}
				return (int)(this.lockTime - num);
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06001720 RID: 5920 RVA: 0x00011692 File Offset: 0x0000F892
		private bool locked
		{
			get
			{
				return this.remainingTime <= 0;
			}
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x00078C94 File Offset: 0x00076E94
		public void FixedUpdate()
		{
			if (NetworkClient.active)
			{
				if (!this.purchased)
				{
					int num = this.remainingTime;
					bool flag = num >= 0;
					bool flag2 = true;
					if (num < -599)
					{
						flag2 = ((num & 1) != 0);
						num = -599;
					}
					int num2 = flag ? num : (-num);
					uint num3 = (uint)(num2 / 60);
					uint value = (uint)(num2 - (int)(num3 * 60u));
					TimedChestController.sharedStringBuilder.Clear();
					TimedChestController.sharedStringBuilder.Append("<mspace=2.5em>");
					if (flag2)
					{
						uint num4 = 2u;
						if (!flag)
						{
							TimedChestController.sharedStringBuilder.Append("-");
							num4 = 1u;
						}
						TimedChestController.sharedStringBuilder.AppendUint(num3, num4, num4);
						TimedChestController.sharedStringBuilder.Append(":");
						TimedChestController.sharedStringBuilder.AppendUint(value, 2u, 2u);
					}
					else
					{
						TimedChestController.sharedStringBuilder.Append("--:--");
					}
					TimedChestController.sharedStringBuilder.Append("</mspace>");
					this.displayTimer.SetText(TimedChestController.sharedStringBuilder);
					this.displayTimer.color = (this.locked ? this.displayIsLockedColor : this.displayIsAvailableColor);
					this.displayTimer.SetText(TimedChestController.sharedStringBuilder);
					this.displayScaleCurve.enabled = false;
					return;
				}
				this.displayScaleCurve.enabled = true;
			}
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x000116A0 File Offset: 0x0000F8A0
		private void OnEnable()
		{
			InstanceTracker.Add<TimedChestController>(this);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x000116A8 File Offset: 0x0000F8A8
		private void OnDisable()
		{
			InstanceTracker.Remove<TimedChestController>(this);
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x000116B0 File Offset: 0x0000F8B0
		public bool ShouldShowOnScanner()
		{
			return !this.purchased;
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001A1A RID: 6682
		public float lockTime = 600f;

		// Token: 0x04001A1B RID: 6683
		public TextMeshPro displayTimer;

		// Token: 0x04001A1C RID: 6684
		public ObjectScaleCurve displayScaleCurve;

		// Token: 0x04001A1D RID: 6685
		public string contextString;

		// Token: 0x04001A1E RID: 6686
		public Color displayIsAvailableColor;

		// Token: 0x04001A1F RID: 6687
		public Color displayIsLockedColor;

		// Token: 0x04001A20 RID: 6688
		public bool purchased;

		// Token: 0x04001A21 RID: 6689
		private const int minTime = -599;

		// Token: 0x04001A22 RID: 6690
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
