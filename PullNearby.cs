using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039B RID: 923
	public class PullNearby : MonoBehaviour
	{
		// Token: 0x0600138C RID: 5004 RVA: 0x0000EEB4 File Offset: 0x0000D0B4
		private void Start()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
			if (this.pullOnStart)
			{
				this.InitializePull();
			}
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x0000EED0 File Offset: 0x0000D0D0
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.fixedAge <= this.pullDuration)
			{
				this.UpdatePull(Time.fixedDeltaTime);
			}
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x0006D3DC File Offset: 0x0006B5DC
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

		// Token: 0x0600138F RID: 5007 RVA: 0x0006D4B0 File Offset: 0x0006B6B0
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

		// Token: 0x06001390 RID: 5008 RVA: 0x0006D570 File Offset: 0x0006B770
		private void AddToList(GameObject affectedObject)
		{
			CharacterBody component = affectedObject.GetComponent<CharacterBody>();
			if (!this.victimBodyList.Contains(component))
			{
				this.victimBodyList.Add(component);
			}
		}

		// Token: 0x04001724 RID: 5924
		public float pullRadius;

		// Token: 0x04001725 RID: 5925
		public float pullDuration;

		// Token: 0x04001726 RID: 5926
		public AnimationCurve pullStrengthCurve;

		// Token: 0x04001727 RID: 5927
		public bool pullOnStart;

		// Token: 0x04001728 RID: 5928
		public int maximumPullCount = int.MaxValue;

		// Token: 0x04001729 RID: 5929
		private List<CharacterBody> victimBodyList = new List<CharacterBody>();

		// Token: 0x0400172A RID: 5930
		private bool pulling;

		// Token: 0x0400172B RID: 5931
		private TeamFilter teamFilter;

		// Token: 0x0400172C RID: 5932
		private float fixedAge;
	}
}
