using System;
using EntityStates.Headstompers;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F5 RID: 757
	public class FallBootsLights : MonoBehaviour
	{
		// Token: 0x06000F59 RID: 3929 RVA: 0x0000BCE9 File Offset: 0x00009EE9
		private void Start()
		{
			this.characterModel = base.GetComponentInParent<CharacterModel>();
			this.FindSourceStateMachine();
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0005C760 File Offset: 0x0005A960
		private void FindSourceStateMachine()
		{
			if (!this.characterModel || !this.characterModel.body)
			{
				return;
			}
			foreach (object obj in this.characterModel.body.transform)
			{
				EntityStateMachine component = ((Transform)obj).GetComponent<EntityStateMachine>();
				if (component && component.state is BaseHeadstompersState)
				{
					this.sourceStateMachine = component;
					break;
				}
			}
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0005C800 File Offset: 0x0005AA00
		private void Update()
		{
			bool flag = this.sourceStateMachine && !(this.sourceStateMachine.state is HeadstompersCooldown);
			if (flag != this.isReady)
			{
				if (flag)
				{
					this.readyEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.readyEffect, base.transform.position, base.transform.rotation, base.transform);
					Util.PlaySound("Play_item_proc_fallboots_activate", base.gameObject);
				}
				else if (this.readyEffectInstance)
				{
					UnityEngine.Object.Destroy(this.readyEffectInstance);
				}
				this.isReady = flag;
			}
			bool flag2 = this.sourceStateMachine && this.sourceStateMachine.state is HeadstompersFall;
			if (flag2 != this.isTriggered)
			{
				if (flag2)
				{
					this.triggerEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.triggerEffect, base.transform.position, base.transform.rotation, base.transform);
					Util.PlaySound("Play_item_proc_fallboots_activate", base.gameObject);
				}
				else if (this.triggerEffectInstance)
				{
					UnityEngine.Object.Destroy(this.triggerEffectInstance);
				}
				this.isTriggered = flag2;
			}
			bool flag3 = this.sourceStateMachine && this.sourceStateMachine.state is HeadstompersCharge;
			if (flag3 != this.isCharging)
			{
				if (flag3)
				{
					this.chargingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.chargingEffect, base.transform.position, base.transform.rotation, base.transform);
				}
				else if (this.chargingEffectInstance)
				{
					UnityEngine.Object.Destroy(this.chargingEffectInstance);
				}
				this.isCharging = flag3;
			}
		}

		// Token: 0x04001365 RID: 4965
		public GameObject readyEffect;

		// Token: 0x04001366 RID: 4966
		public GameObject triggerEffect;

		// Token: 0x04001367 RID: 4967
		public GameObject chargingEffect;

		// Token: 0x04001368 RID: 4968
		private GameObject readyEffectInstance;

		// Token: 0x04001369 RID: 4969
		private GameObject triggerEffectInstance;

		// Token: 0x0400136A RID: 4970
		private GameObject chargingEffectInstance;

		// Token: 0x0400136B RID: 4971
		private bool isReady;

		// Token: 0x0400136C RID: 4972
		private bool isTriggered;

		// Token: 0x0400136D RID: 4973
		private bool isCharging;

		// Token: 0x0400136E RID: 4974
		private CharacterModel characterModel;

		// Token: 0x0400136F RID: 4975
		private EntityStateMachine sourceStateMachine;
	}
}
