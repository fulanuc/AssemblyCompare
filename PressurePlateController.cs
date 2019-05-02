using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x0200039E RID: 926
	public class PressurePlateController : MonoBehaviour
	{
		// Token: 0x06001399 RID: 5017 RVA: 0x000025DA File Offset: 0x000007DA
		private void Start()
		{
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x0006D0F8 File Offset: 0x0006B2F8
		private void FixedUpdate()
		{
			if (this.enableOverlapSphere)
			{
				this.overlapSphereStopwatch += Time.fixedDeltaTime;
				if (this.overlapSphereStopwatch >= 1f / this.overlapSphereFrequency)
				{
					this.overlapSphereStopwatch -= 1f / this.overlapSphereFrequency;
					bool @switch = Physics.OverlapSphere(base.transform.position, this.overlapSphereRadius, LayerIndex.defaultLayer.mask | LayerIndex.fakeActor.mask, QueryTriggerInteraction.UseGlobal).Length != 0;
					this.SetSwitch(@switch);
				}
			}
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x0000F01C File Offset: 0x0000D21C
		public void EnableOverlapSphere(bool input)
		{
			this.enableOverlapSphere = input;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x0006D198 File Offset: 0x0006B398
		public void SetSwitch(bool switchIsDown)
		{
			if (switchIsDown != this.switchDown)
			{
				if (switchIsDown)
				{
					this.animationStopwatch = 0f;
					Util.PlaySound(this.switchDownSoundString, base.gameObject);
					UnityEvent onSwitchDown = this.OnSwitchDown;
					if (onSwitchDown != null)
					{
						onSwitchDown.Invoke();
					}
				}
				else
				{
					this.animationStopwatch = 0f;
					Util.PlaySound(this.switchUpSoundString, base.gameObject);
					UnityEvent onSwitchUp = this.OnSwitchUp;
					if (onSwitchUp != null)
					{
						onSwitchUp.Invoke();
					}
				}
				this.switchDown = switchIsDown;
			}
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0006D218 File Offset: 0x0006B418
		private void Update()
		{
			this.animationStopwatch += Time.deltaTime;
			if (this.switchVisualTransform)
			{
				Vector3 localPosition = this.switchVisualTransform.transform.localPosition;
				bool flag = this.switchDown;
				if (flag)
				{
					if (flag)
					{
						localPosition.z = this.switchVisualPositionFromUpToDown.Evaluate(this.animationStopwatch);
					}
				}
				else
				{
					localPosition.z = this.switchVisualPositionFromDownToUp.Evaluate(this.animationStopwatch);
				}
				this.switchVisualTransform.localPosition = localPosition;
			}
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x0000F025 File Offset: 0x0000D225
		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(base.transform.position, this.overlapSphereRadius);
		}

		// Token: 0x04001721 RID: 5921
		public bool enableOverlapSphere = true;

		// Token: 0x04001722 RID: 5922
		public float overlapSphereRadius;

		// Token: 0x04001723 RID: 5923
		public float overlapSphereFrequency;

		// Token: 0x04001724 RID: 5924
		public string switchDownSoundString;

		// Token: 0x04001725 RID: 5925
		public string switchUpSoundString;

		// Token: 0x04001726 RID: 5926
		public UnityEvent OnSwitchDown;

		// Token: 0x04001727 RID: 5927
		public UnityEvent OnSwitchUp;

		// Token: 0x04001728 RID: 5928
		public AnimationCurve switchVisualPositionFromUpToDown;

		// Token: 0x04001729 RID: 5929
		public AnimationCurve switchVisualPositionFromDownToUp;

		// Token: 0x0400172A RID: 5930
		public Transform switchVisualTransform;

		// Token: 0x0400172B RID: 5931
		private float overlapSphereStopwatch;

		// Token: 0x0400172C RID: 5932
		private float animationStopwatch;

		// Token: 0x0400172D RID: 5933
		private bool switchDown;
	}
}
