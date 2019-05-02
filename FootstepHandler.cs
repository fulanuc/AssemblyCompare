using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002FA RID: 762
	[RequireComponent(typeof(ChildLocator))]
	public class FootstepHandler : MonoBehaviour
	{
		// Token: 0x06000F6E RID: 3950 RVA: 0x0005CEB4 File Offset: 0x0005B0B4
		private void Start()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			this.body = base.GetComponent<CharacterModel>().body;
			CharacterMaster master = this.body.master;
			this.bodyInventory = ((master != null) ? master.inventory : null);
			this.animator = base.GetComponent<Animator>();
			if (this.enableFootstepDust)
			{
				this.footstepDustInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(this.footstepDustPrefab, base.transform).transform;
				this.footstepDustInstanceParticleSystem = this.footstepDustInstanceTransform.GetComponent<ParticleSystem>();
				this.footstepDustInstanceShakeEmitter = this.footstepDustInstanceTransform.GetComponent<ShakeEmitter>();
			}
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0005CF50 File Offset: 0x0005B150
		public void Footstep(AnimationEvent animationEvent)
		{
			if ((double)animationEvent.animatorClipInfo.weight > 0.5)
			{
				this.Footstep(animationEvent.stringParameter, (GameObject)animationEvent.objectReferenceParameter);
			}
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0005CF90 File Offset: 0x0005B190
		public void Footstep(string childName, GameObject footstepEffect)
		{
			Transform transform = this.childLocator.FindChild(childName);
			if (transform)
			{
				Color color = Color.gray;
				RaycastHit raycastHit = default(RaycastHit);
				Vector3 position = transform.position;
				position.y += 1.5f;
				Debug.DrawRay(position, Vector3.down);
				if (Physics.Raycast(new Ray(position, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask | LayerIndex.water.mask, QueryTriggerInteraction.Collide))
				{
					if (this.bodyInventory && this.bodyInventory.GetItemCount(ItemIndex.Hoof) > 0 && childName == "FootR")
					{
						Util.PlaySound("Play_item_proc_hoof", transform.gameObject);
					}
					if (footstepEffect)
					{
						EffectManager.instance.SimpleImpactEffect(footstepEffect, raycastHit.point, raycastHit.normal, false);
					}
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(raycastHit.collider, raycastHit.point);
					bool flag = false;
					if (objectSurfaceDef)
					{
						color = objectSurfaceDef.approximateColor;
						if (objectSurfaceDef.footstepEffectPrefab)
						{
							EffectManager.instance.SpawnEffect(objectSurfaceDef.footstepEffectPrefab, new EffectData
							{
								origin = raycastHit.point,
								scale = this.body.radius
							}, false);
							flag = true;
						}
						if (!string.IsNullOrEmpty(objectSurfaceDef.materialSwitchString))
						{
							AkSoundEngine.SetSwitch("material", objectSurfaceDef.materialSwitchString, transform.gameObject);
						}
					}
					else
					{
						Debug.LogFormat("{0} is missing surface def", new object[]
						{
							raycastHit.collider.gameObject
						});
					}
					if (this.footstepDustInstanceTransform && !flag)
					{
						this.footstepDustInstanceTransform.position = raycastHit.point;
						this.footstepDustInstanceParticleSystem.main.startColor = color;
						this.footstepDustInstanceParticleSystem.Play();
						if (this.footstepDustInstanceShakeEmitter)
						{
							this.footstepDustInstanceShakeEmitter.StartShake();
						}
					}
				}
				Util.PlaySound(this.baseFootstepString, transform.gameObject);
				return;
			}
			Debug.LogWarningFormat("Object {0} lacks ChildLocator entry \"{1}\" to handle Footstep event!", new object[]
			{
				base.gameObject.name,
				childName
			});
		}

		// Token: 0x0400138D RID: 5005
		public string baseFootstepString;

		// Token: 0x0400138E RID: 5006
		public bool enableFootstepDust;

		// Token: 0x0400138F RID: 5007
		public GameObject footstepDustPrefab;

		// Token: 0x04001390 RID: 5008
		private ChildLocator childLocator;

		// Token: 0x04001391 RID: 5009
		private Inventory bodyInventory;

		// Token: 0x04001392 RID: 5010
		private Animator animator;

		// Token: 0x04001393 RID: 5011
		private Transform footstepDustInstanceTransform;

		// Token: 0x04001394 RID: 5012
		private ParticleSystem footstepDustInstanceParticleSystem;

		// Token: 0x04001395 RID: 5013
		private ShakeEmitter footstepDustInstanceShakeEmitter;

		// Token: 0x04001396 RID: 5014
		private CharacterBody body;
	}
}
