using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000425 RID: 1061
	public class WispAI : MonoBehaviour
	{
		// Token: 0x060017B3 RID: 6067 RVA: 0x00011BBF File Offset: 0x0000FDBF
		private void Awake()
		{
			this.bodyDirectionComponent = this.body.GetComponent<CharacterDirection>();
			this.bodyMotorComponent = this.body.GetComponent<CharacterMotor>();
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x0007B4D4 File Offset: 0x000796D4
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

		// Token: 0x060017B5 RID: 6069 RVA: 0x0007B594 File Offset: 0x00079794
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

		// Token: 0x04001ACC RID: 6860
		[Tooltip("The character to control.")]
		public GameObject body;

		// Token: 0x04001ACD RID: 6861
		[Tooltip("The enemy to target.")]
		public Transform targetTransform;

		// Token: 0x04001ACE RID: 6862
		[Tooltip("The skill to activate for a ranged attack.")]
		public GenericSkill fireSkill;

		// Token: 0x04001ACF RID: 6863
		[Tooltip("How close the character must be to the enemy to use a ranged attack.")]
		public float fireRange;

		// Token: 0x04001AD0 RID: 6864
		private CharacterDirection bodyDirectionComponent;

		// Token: 0x04001AD1 RID: 6865
		private CharacterMotor bodyMotorComponent;

		// Token: 0x04001AD2 RID: 6866
		private static List<WispAI.TargetSearchCandidate> candidateList = new List<WispAI.TargetSearchCandidate>();

		// Token: 0x02000426 RID: 1062
		private struct TargetSearchCandidate
		{
			// Token: 0x04001AD3 RID: 6867
			public Transform transform;

			// Token: 0x04001AD4 RID: 6868
			public Vector3 positionDiff;

			// Token: 0x04001AD5 RID: 6869
			public float sqrDistance;
		}
	}
}
