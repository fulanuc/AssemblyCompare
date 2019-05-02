using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000362 RID: 866
	public class ModelLocator : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x060011DD RID: 4573 RVA: 0x0000D98A File Offset: 0x0000BB8A
		public void Start()
		{
			if (this.modelTransform)
			{
				this.modelParentTransform = this.modelTransform.parent;
				if (!this.dontDetatchFromParent)
				{
					this.modelTransform.parent = null;
				}
			}
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x000675B0 File Offset: 0x000657B0
		private void UpdateModelTransform(float deltaTime)
		{
			if (this.modelTransform && this.modelParentTransform)
			{
				Vector3 position = this.modelParentTransform.position;
				Quaternion quaternion = this.modelParentTransform.rotation;
				this.UpdateTargetNormal();
				this.SmoothNormals(deltaTime);
				quaternion = Quaternion.FromToRotation(Vector3.up, this.currentNormal) * quaternion;
				this.modelTransform.SetPositionAndRotation(position, quaternion);
			}
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x0000D9BE File Offset: 0x0000BBBE
		private void SmoothNormals(float deltaTime)
		{
			this.currentNormal = Vector3.SmoothDamp(this.currentNormal, this.targetNormal, ref this.normalSmoothdampVelocity, 0.1f, float.PositiveInfinity, deltaTime);
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00067620 File Offset: 0x00065820
		private void UpdateTargetNormal()
		{
			if (!this.normalizeToFloor)
			{
				this.targetNormal = Vector3.up;
				return;
			}
			if (this.characterMotor)
			{
				this.targetNormal = this.characterMotor.estimatedFloorNormal;
				return;
			}
			this.characterMotor = base.GetComponent<CharacterMotor>();
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x0000D9E8 File Offset: 0x0000BBE8
		public void LateUpdate()
		{
			if (this.autoUpdateModelTransform)
			{
				this.UpdateModelTransform(Time.deltaTime);
			}
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x0006766C File Offset: 0x0006586C
		private void OnDestroy()
		{
			if (this.modelTransform)
			{
				if (this.preserveModel)
				{
					if (!this.noCorpse)
					{
						this.modelTransform.gameObject.AddComponent<Corpse>();
					}
					this.modelTransform = null;
					return;
				}
				UnityEngine.Object.Destroy(this.modelTransform.gameObject);
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x0000D9FD File Offset: 0x0000BBFD
		public void OnDeathStart()
		{
			if (!this.dontReleaseModelOnDeath)
			{
				this.preserveModel = true;
			}
		}

		// Token: 0x040015E5 RID: 5605
		[Tooltip("The transform of the child gameobject which acts as the model for this entity.")]
		public Transform modelTransform;

		// Token: 0x040015E6 RID: 5606
		[Tooltip("The transform of the child gameobject which acts as the base for this entity's model. If provided, this will be detached from the hierarchy and positioned to match this object's position.")]
		public Transform modelBaseTransform;

		// Token: 0x040015E7 RID: 5607
		[Tooltip("Whether or not the model reference should be released upon the death of this character.")]
		public bool dontReleaseModelOnDeath;

		// Token: 0x040015E8 RID: 5608
		[Tooltip("Whether or not to update the model transforms automatically.")]
		public bool autoUpdateModelTransform = true;

		// Token: 0x040015E9 RID: 5609
		public bool dontDetatchFromParent;

		// Token: 0x040015EA RID: 5610
		private Transform modelParentTransform;

		// Token: 0x040015EB RID: 5611
		public bool noCorpse;

		// Token: 0x040015EC RID: 5612
		public bool normalizeToFloor;

		// Token: 0x040015ED RID: 5613
		private const float normalSmoothdampTime = 0.1f;

		// Token: 0x040015EE RID: 5614
		private Vector3 normalSmoothdampVelocity;

		// Token: 0x040015EF RID: 5615
		private Vector3 targetNormal = Vector3.up;

		// Token: 0x040015F0 RID: 5616
		private Vector3 currentNormal = Vector3.up;

		// Token: 0x040015F1 RID: 5617
		private CharacterMotor characterMotor;

		// Token: 0x040015F2 RID: 5618
		public bool preserveModel;
	}
}
