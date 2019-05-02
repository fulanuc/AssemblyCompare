using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.CharacterAI
{
	// Token: 0x020005A5 RID: 1445
	public class AISkillDriver : MonoBehaviour
	{
		// Token: 0x170002DE RID: 734
		// (get) Token: 0x060020B4 RID: 8372 RVA: 0x00017DFB File Offset: 0x00015FFB
		public float minDistanceSqr
		{
			get
			{
				return this.minDistance * this.minDistance;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x060020B5 RID: 8373 RVA: 0x00017E0A File Offset: 0x0001600A
		public float maxDistanceSqr
		{
			get
			{
				return this.maxDistance * this.maxDistance;
			}
		}

		// Token: 0x040022A5 RID: 8869
		[Tooltip("The name of this skill driver for reference purposes.")]
		public string customName;

		// Token: 0x040022A6 RID: 8870
		[Tooltip("The slot of the associated skill. Set to None to allow this behavior to run regardless of skill availability.")]
		public SkillSlot skillSlot;

		// Token: 0x040022A7 RID: 8871
		[Tooltip("If set, this cannot be the dominant driver while the skill is on cooldown or out of stock.")]
		public bool requireSkillReady;

		// Token: 0x040022A8 RID: 8872
		[Tooltip("The type of object targeted for movement.")]
		[FormerlySerializedAs("targetType")]
		public AISkillDriver.TargetType moveTargetType;

		// Token: 0x040022A9 RID: 8873
		[Tooltip("The minimum health fraction required of the user for this behavior.")]
		public float minUserHealthFraction = float.NegativeInfinity;

		// Token: 0x040022AA RID: 8874
		[Tooltip("The maximum health fraction required of the user for this behavior.")]
		public float maxUserHealthFraction = float.PositiveInfinity;

		// Token: 0x040022AB RID: 8875
		[Tooltip("The minimum health fraction required of the target for this behavior.")]
		public float minTargetHealthFraction = float.NegativeInfinity;

		// Token: 0x040022AC RID: 8876
		[Tooltip("The maximum health fraction required of the target for this behavior.")]
		public float maxTargetHealthFraction = float.PositiveInfinity;

		// Token: 0x040022AD RID: 8877
		[Tooltip("The minimum distance from the target required for this behavior.")]
		public float minDistance;

		// Token: 0x040022AE RID: 8878
		[Tooltip("The maximum distance from the target required for this behavior.")]
		public float maxDistance = float.PositiveInfinity;

		// Token: 0x040022AF RID: 8879
		public bool selectionRequiresTargetLoS;

		// Token: 0x040022B0 RID: 8880
		[Tooltip("If set, this skill will not be activated unless there is LoS to the target.")]
		public bool activationRequiresTargetLoS;

		// Token: 0x040022B1 RID: 8881
		[Tooltip("If set, this skill will not be activated unless the aim vector is pointing close to the target.")]
		public bool activationRequiresAimConfirmation;

		// Token: 0x040022B2 RID: 8882
		[Tooltip("The movement type to use while this is the dominant skill driver.")]
		public AISkillDriver.MovementType movementType = AISkillDriver.MovementType.ChaseMoveTarget;

		// Token: 0x040022B3 RID: 8883
		public float moveInputScale = 1f;

		// Token: 0x040022B4 RID: 8884
		[Tooltip("Where to look while this is the dominant skill driver")]
		public AISkillDriver.AimType aimType = AISkillDriver.AimType.AtMoveTarget;

		// Token: 0x040022B5 RID: 8885
		[Tooltip("If set, the nodegraph will not be used to direct the local navigator while this is the dominant skill driver. Direction toward the target will be used instead.")]
		public bool ignoreNodeGraph;

		// Token: 0x040022B6 RID: 8886
		[Tooltip("If non-negative, this value will be used for the driver evaluation timer while this is the dominant skill driver.")]
		public float driverUpdateTimerOverride = -1f;

		// Token: 0x040022B7 RID: 8887
		[Tooltip("If set and this is the dominant skill driver, the current enemy will be reset at the time of the next evaluation.")]
		public bool resetCurrentEnemyOnNextDriverSelection;

		// Token: 0x040022B8 RID: 8888
		[Tooltip("If true, this skill driver cannot be chosen twice in a row.")]
		public bool noRepeat;

		// Token: 0x040022B9 RID: 8889
		[Tooltip("If true, the AI will attempt to sprint while this is the dominant skill driver.")]
		public bool shouldSprint;

		// Token: 0x020005A6 RID: 1446
		public enum TargetType
		{
			// Token: 0x040022BB RID: 8891
			CurrentEnemy,
			// Token: 0x040022BC RID: 8892
			NearestFriendlyInSkillRange,
			// Token: 0x040022BD RID: 8893
			CurrentLeader
		}

		// Token: 0x020005A7 RID: 1447
		public enum AimType
		{
			// Token: 0x040022BF RID: 8895
			None,
			// Token: 0x040022C0 RID: 8896
			AtMoveTarget,
			// Token: 0x040022C1 RID: 8897
			AtCurrentEnemy,
			// Token: 0x040022C2 RID: 8898
			AtCurrentLeader,
			// Token: 0x040022C3 RID: 8899
			MoveDirection
		}

		// Token: 0x020005A8 RID: 1448
		public enum MovementType
		{
			// Token: 0x040022C5 RID: 8901
			Stop,
			// Token: 0x040022C6 RID: 8902
			ChaseMoveTarget,
			// Token: 0x040022C7 RID: 8903
			StrafeMovetarget,
			// Token: 0x040022C8 RID: 8904
			FleeMoveTarget
		}
	}
}
