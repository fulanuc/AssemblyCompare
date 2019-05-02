using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A0 RID: 928
	public class PullNearby : MonoBehaviour
	{
		// Token: 0x060013A9 RID: 5033 RVA: 0x0000F07E File Offset: 0x0000D27E
		private void Start()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
			if (this.pullOnStart)
			{
				this.InitializePull();
			}
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0000F09A File Offset: 0x0000D29A
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.fixedAge <= this.pullDuration)
			{
				this.UpdatePull(Time.fixedDeltaTime);
			}
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x0006D5E4 File Offset: 0x0006B7E4
		private void UpdatePull(float deltaTime)
		{
			if (!this.pulling)
			{
				return;
			}
			for (int i = 0; i < this.victimBodyList.Count; i++)
			{
				CharacterBody characterBody = this.victimBodyList[i];
				Vector3 vector = base.transform.position - characterBody.corePosition;
				float d = this.pullStrengthCurve.Evaluate(vector.magnitude / this.pullRadius);
				Vector3 b = vector.normalized * d * deltaTime;
				CharacterMotor component = characterBody.GetComponent<CharacterMotor>();
				if (component)
				{
					component.rootMotion += b;
				}
				else
				{
					Rigidbody component2 = characterBody.GetComponent<Rigidbody>();
					if (component2)
					{
						component2.velocity += b;
					}
				}
			}
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x0006D6B8 File Offset: 0x0006B8B8
		public void InitializePull()
		{
			if (this.pulling)
			{
				return;
			}
			this.pulling = true;
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.pullRadius, LayerIndex.defaultLayer.mask);
			int num = 0;
			int num2 = 0;
			while (num < array.Length && num2 < this.maximumPullCount)
			{
				HealthComponent component = array[num].GetComponent<HealthComponent>();
				if (component)
				{
					TeamComponent component2 = component.GetComponent<TeamComponent>();
					bool flag = false;
					if (component2 && this.teamFilter)
					{
						flag = (component2.teamIndex == this.teamFilter.teamIndex);
					}
					if (!flag)
					{
						this.AddToList(component.gameObject);
						num2++;
					}
				}
				num++;
			}
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x0006D778 File Offset: 0x0006B978
		private void AddToList(GameObject affectedObject)
		{
			CharacterBody component = affectedObject.GetComponent<CharacterBody>();
			if (!this.victimBodyList.Contains(component))
			{
				this.victimBodyList.Add(component);
			}
		}

		// Token: 0x04001740 RID: 5952
		public float pullRadius;

		// Token: 0x04001741 RID: 5953
		public float pullDuration;

		// Token: 0x04001742 RID: 5954
		public AnimationCurve pullStrengthCurve;

		// Token: 0x04001743 RID: 5955
		public bool pullOnStart;

		// Token: 0x04001744 RID: 5956
		public int maximumPullCount = int.MaxValue;

		// Token: 0x04001745 RID: 5957
		private List<CharacterBody> victimBodyList = new List<CharacterBody>();

		// Token: 0x04001746 RID: 5958
		private bool pulling;

		// Token: 0x04001747 RID: 5959
		private TeamFilter teamFilter;

		// Token: 0x04001748 RID: 5960
		private float fixedAge;
	}
}
