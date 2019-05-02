using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.CharacterAI
{
	// Token: 0x02000592 RID: 1426
	public class AISkillDriver : MonoBehaviour
	{
		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06002023 RID: 8227 RVA: 0x000176CC File Offset: 0x000158CC
		public float minDistanceSqr
		{
			get
			{
				return this.minDistance * this.minDistance;
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06002024 RID: 8228 RVA: 0x000176DB File Offset: 0x000158DB
		public float maxDistanceSqr
		{
			get
			{
				return this.maxDistance * this.maxDistance;
			}
		}

		// Token: 0x0400224D RID: 8781
		[Tooltip("The name of this skill driver for reference purposes.")]
		public string customName;

		// Token: 0x0400224E RID: 8782
		[Tooltip("The slot of the associated skill. Set to None to allow this behavior to run regardless of skill availability.")]
		public SkillSlot skillSlot;

		// Token: 0x0400224F RID: 8783
		[Tooltip("If set, this cannot be the dominant driver while the skill is on cooldown or out of stock.")]
		public bool requireSkillReady;

		// Token: 0x04002250 RID: 8784
		[Tooltip("The type of object targeted for movement.")]
		[FormerlySerializedAs("targetType")]
		public AISkillDriver.TargetType moveTargetType;

		// Token: 0x04002251 RID: 8785
		[Tooltip("The minimum health fraction required of the user for this behavior.")]
		public float minUserHealthFraction = float.NegativeInfinity;

		// Token: 0x04002252 RID: 8786
		[Tooltip("The maximum health fraction required of the user for this behavior.")]
		public float maxUserHealthFraction = float.PositiveInfinity;

		// Token: 0x04002253 RID: 8787
		[Tooltip("The minimum health fraction required of the target for this behavior.")]
		public float minTargetHealthFraction = float.NegativeInfinity;

		// Token: 0x04002254 RID: 8788
		[Tooltip("The maximum health fraction required of the target for this behavior.")]
		public float maxTargetHealthFraction = float.PositiveInfinity;

		// Token: 0x04002255 RID: 8789
		[Tooltip("The minimum distance from the target required for this behavior.")]
		public float minDistance;

		// Token: 0x04002256 RID: 8790
		[Tooltip("The maximum distance from the target required for this behavior.")]
		public float maxDistance = float.PositiveInfinity;

		// Token: 0x04002257 RID: 8791
		public bool selectionRequiresTargetLoS;

		// Token: 0x04002258 RID: 8792
		[Tooltip("If set, this skill will not be activated unless there is LoS to the target.")]
		public bool activationRequiresTargetLoS;

		// Token: 0x04002259 RID: 8793
		[Tooltip("If set, this skill will not be activated unless the aim vector is pointing close to the target.")]
		public bool activationRequiresAimConfirmation;

		// Token: 0x0400225A RID: 8794
		[Tooltip("The movement type to use while this is the dominant skill driver.")]
		public AISkillDriver.MovementType movementType = AISkillDriver.MovementType.ChaseMoveTarget;

		// Token: 0x0400225B RID: 8795
		public float moveInputScale = 1f;

		// Token: 0x0400225C RID: 8796
		[Tooltip("Where to look while this is the dominant skill driver")]
		public AISkillDriver.AimType aimType = AISkillDriver.AimType.AtMoveTarget;

		// Token: 0x0400225D RID: 8797
		[Tooltip("If set, the nodegraph will not be used to direct the local navigator while this is the dominant skill driver. Direction toward the target will be used instead.")]
		public bool ignoreNodeGraph;

		// Token: 0x0400225E RID: 8798
		[Tooltip("If non-negative, this value will be used for the driver evaluation timer while this is the dominant skill driver.")]
		public float driverUpdateTimerOverride = -1f;

		// Token: 0x0400225F RID: 8799
		[Tooltip("If set and this is the dominant skill driver, the current enemy will be reset at the time of the next evaluation.")]
		public bool resetCurrentEnemyOnNextDriverSelection;

		// Token: 0x04002260 RID: 8800
		[Tooltip("If true, this skill driver cannot be chosen twice in a row.")]
		public bool noRepeat;

		// Token: 0x04002261 RID: 8801
		[Tooltip("If true, the AI will attempt to sprint while this is the dominant skill driver.")]
		public bool shouldSprint;

		// Token: 0x02000593 RID: 1427
		public enum TargetType
		{
			// Token: 0x04002263 RID: 8803
			CurrentEnemy,
			// Token: 0x04002264 RID: 8804
			NearestFriendlyInSkillRange,
			// Token: 0x04002265 RID: 8805
			CurrentLeader
		}

		// Token: 0x02000594 RID: 1428
		public enum AimType
		{
			// Token: 0x04002267 RID: 8807
			None,
			// Token: 0x04002268 RID: 8808
			AtMoveTarget,
			// Token: 0x04002269 RID: 8809
			AtCurrentEnemy,
			// Token: 0x0400226A RID: 8810
			AtCurrentLeader,
			// Token: 0x0400226B RID: 8811
			MoveDirection
		}

		// Token: 0x02000595 RID: 1429
		public enum MovementType
		{
			// Token: 0x0400226D RID: 8813
			Stop,
			// Token: 0x0400226E RID: 8814
			ChaseMoveTarget,
			// Token: 0x0400226F RID: 8815
			StrafeMovetarget,
			// Token: 0x04002270 RID: 8816
			FleeMoveTarget
		}
	}
}
