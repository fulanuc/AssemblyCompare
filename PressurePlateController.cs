using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x02000399 RID: 921
	public class PressurePlateController : MonoBehaviour
	{
		// Token: 0x0600137C RID: 4988 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Start()
		{
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x0006CEE0 File Offset: 0x0006B0E0
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

		// Token: 0x0600137E RID: 4990 RVA: 0x0000EE5F File Offset: 0x0000D05F
		public void EnableOverlapSphere(bool input)
		{
			this.enableOverlapSphere = input;
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x0006CF80 File Offset: 0x0006B180
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

		// Token: 0x06001380 RID: 4992 RVA: 0x0006D000 File Offset: 0x0006B200
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

		// Token: 0x06001381 RID: 4993 RVA: 0x0000EE68 File Offset: 0x0000D068
		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(base.transform.position, this.overlapSphereRadius);
		}

		// Token: 0x04001705 RID: 5893
		public bool enableOverlapSphere = true;

		// Token: 0x04001706 RID: 5894
		public float overlapSphereRadius;

		// Token: 0x04001707 RID: 5895
		public float overlapSphereFrequency;

		// Token: 0x04001708 RID: 5896
		public string switchDownSoundString;

		// Token: 0x04001709 RID: 5897
		public string switchUpSoundString;

		// Token: 0x0400170A RID: 5898
		public UnityEvent OnSwitchDown;

		// Token: 0x0400170B RID: 5899
		public UnityEvent OnSwitchUp;

		// Token: 0x0400170C RID: 5900
		public AnimationCurve switchVisualPositionFromUpToDown;

		// Token: 0x0400170D RID: 5901
		public AnimationCurve switchVisualPositionFromDownToUp;

		// Token: 0x0400170E RID: 5902
		public Transform switchVisualTransform;

		// Token: 0x0400170F RID: 5903
		private float overlapSphereStopwatch;

		// Token: 0x04001710 RID: 5904
		private float animationStopwatch;

		// Token: 0x04001711 RID: 5905
		private bool switchDown;
	}
}
