using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200042B RID: 1067
	public class WispAI : MonoBehaviour
	{
		// Token: 0x060017F7 RID: 6135 RVA: 0x00011FF1 File Offset: 0x000101F1
		private void Awake()
		{
			this.bodyDirectionComponent = this.body.GetComponent<CharacterDirection>();
			this.bodyMotorComponent = this.body.GetComponent<CharacterMotor>();
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x0007BA94 File Offset: 0x00079C94
		private void FixedUpdate()
		{
			if (!this.body)
			{
				return;
			}
			if (!this.targetTransform)
			{
				this.targetTransform = this.SearchForTarget();
			}
			if (this.targetTransform)
			{
				Vector3 vector = this.targetTransform.position - this.body.transform.position;
				this.bodyMotorComponent.moveDirection = vector;
				this.bodyDirectionComponent.moveVector = Vector3.Lerp(this.bodyDirectionComponent.moveVector, vector, Time.deltaTime);
				if (this.fireSkill && vector.sqrMagnitude < this.fireRange * this.fireRange)
				{
					this.fireSkill.ExecuteIfReady();
				}
			}
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x0007BB54 File Offset: 0x00079D54
		private Transform SearchForTarget()
		{
			Vector3 position = this.body.transform.position;
			Vector3 forward = this.bodyDirectionComponent.forward;
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				Transform transform = teamMembers[i].transform;
				Vector3 vector = transform.position - position;
				if (Vector3.Dot(forward, vector) > 0f)
				{
					WispAI.candidateList.Add(new WispAI.TargetSearchCandidate
					{
						transform = transform,
						positionDiff = vector,
						sqrDistance = vector.sqrMagnitude
					});
				}
			}
			WispAI.candidateList.Sort(delegate(WispAI.TargetSearchCandidate a, WispAI.TargetSearchCandidate b)
			{
				if (a.sqrDistance < b.sqrDistance)
				{
					return -1;
				}
				if (a.sqrDistance != b.sqrDistance)
				{
					return 1;
				}
				return 0;
			});
			Transform result = null;
			for (int j = 0; j < WispAI.candidateList.Count; j++)
			{
				if (!Physics.Raycast(position, WispAI.candidateList[j].positionDiff, Mathf.Sqrt(WispAI.candidateList[j].sqrDistance), LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					result = WispAI.candidateList[j].transform;
					break;
				}
			}
			WispAI.candidateList.Clear();
			return result;
		}

		// Token: 0x04001AF5 RID: 6901
		[Tooltip("The character to control.")]
		public GameObject body;

		// Token: 0x04001AF6 RID: 6902
		[Tooltip("The enemy to target.")]
		public Transform targetTransform;

		// Token: 0x04001AF7 RID: 6903
		[Tooltip("The skill to activate for a ranged attack.")]
		public GenericSkill fireSkill;

		// Token: 0x04001AF8 RID: 6904
		[Tooltip("How close the character must be to the enemy to use a ranged attack.")]
		public float fireRange;

		// Token: 0x04001AF9 RID: 6905
		private CharacterDirection bodyDirectionComponent;

		// Token: 0x04001AFA RID: 6906
		private CharacterMotor bodyMotorComponent;

		// Token: 0x04001AFB RID: 6907
		private static List<WispAI.TargetSearchCandidate> candidateList = new List<WispAI.TargetSearchCandidate>();

		// Token: 0x0200042C RID: 1068
		private struct TargetSearchCandidate
		{
			// Token: 0x04001AFC RID: 6908
			public Transform transform;

			// Token: 0x04001AFD RID: 6909
			public Vector3 positionDiff;

			// Token: 0x04001AFE RID: 6910
			public float sqrDistance;
		}
	}
}
