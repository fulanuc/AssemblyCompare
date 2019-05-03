using System;
using System.Text;
using EntityStates.TimedChest;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000401 RID: 1025
	public class TimedChestController : NetworkBehaviour, IInteractable
	{
		// Token: 0x060016DB RID: 5851 RVA: 0x00011237 File Offset: 0x0000F437
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextString);
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00011244 File Offset: 0x0000F444
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

		// Token: 0x060016DD RID: 5853 RVA: 0x0001125B File Offset: 0x0000F45B
		public void OnInteractionBegin(Interactor activator)
		{
			base.GetComponent<EntityStateMachine>().SetNextState(new Opening());
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060016DF RID: 5855 RVA: 0x0001126D File Offset: 0x0000F46D
		private int remainingTime
		{
			get
			{
				return (int)(this.lockTime - Run.instance.time);
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060016E0 RID: 5856 RVA: 0x00011281 File Offset: 0x0000F481
		private bool locked
		{
			get
			{
				return this.remainingTime <= 0;
			}
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x000786D4 File Offset: 0x000768D4
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

		// Token: 0x060016E4 RID: 5860 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040019F1 RID: 6641
		public float lockTime = 600f;

		// Token: 0x040019F2 RID: 6642
		public TextMeshPro displayTimer;

		// Token: 0x040019F3 RID: 6643
		public ObjectScaleCurve displayScaleCurve;

		// Token: 0x040019F4 RID: 6644
		public string contextString;

		// Token: 0x040019F5 RID: 6645
		public Color displayIsAvailableColor;

		// Token: 0x040019F6 RID: 6646
		public Color displayIsLockedColor;

		// Token: 0x040019F7 RID: 6647
		public bool purchased;

		// Token: 0x040019F8 RID: 6648
		private const int minTime = -599;

		// Token: 0x040019F9 RID: 6649
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
