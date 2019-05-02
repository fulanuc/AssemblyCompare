using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000401 RID: 1025
	public class TeleportOutController : NetworkBehaviour
	{
		// Token: 0x060016FE RID: 5886 RVA: 0x000786D4 File Offset: 0x000768D4
		public static void AddTPOutEffect(CharacterModel characterModel, float beginAlpha, float endAlpha, float duration)
		{
			if (characterModel)
			{
				TemporaryOverlay temporaryOverlay = characterModel.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = duration;
				temporaryOverlay.animateShaderAlpha = true;
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, beginAlpha, 1f, endAlpha);
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matTPInOut");
				temporaryOverlay.AddToCharacerModel(characterModel);
			}
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x00078738 File Offset: 0x00076938
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (this.target)
			{
				ModelLocator component = this.target.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
						if (component2)
						{
							TeleportOutController.AddTPOutEffect(component2, 0f, 1f, 2f);
							if (component2.rendererInfos.Length != 0)
							{
								Renderer renderer = component2.rendererInfos[component2.rendererInfos.Length - 1].renderer;
								if (renderer)
								{
									ParticleSystem.ShapeModule shape = this.bodyGlowParticles.shape;
									if (renderer is MeshRenderer)
									{
										shape.shapeType = ParticleSystemShapeType.MeshRenderer;
										shape.meshRenderer = (renderer as MeshRenderer);
									}
									else if (renderer is SkinnedMeshRenderer)
									{
										shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
										shape.skinnedMeshRenderer = (renderer as SkinnedMeshRenderer);
									}
								}
							}
						}
					}
				}
			}
			this.bodyGlowParticles.Play();
			Util.PlaySound(TeleportOutController.tpOutSoundString, base.gameObject);
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x00078840 File Offset: 0x00076A40
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.age += Time.fixedDeltaTime;
				if (this.age >= 2f && this.target)
				{
					GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(this.target);
					if (teleportEffectPrefab)
					{
						EffectManager.instance.SpawnEffect(teleportEffectPrefab, new EffectData
						{
							origin = this.target.transform.position
						}, true);
					}
					UnityEngine.Object.Destroy(this.target);
				}
			}
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06001704 RID: 5892 RVA: 0x000788CC File Offset: 0x00076ACC
		// (set) Token: 0x06001705 RID: 5893 RVA: 0x000114F2 File Offset: 0x0000F6F2
		public GameObject Networktarget
		{
			get
			{
				return this.target;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.target, 1u, ref this.___targetNetId);
			}
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x000788E0 File Offset: 0x00076AE0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.target);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.target);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x0007894C File Offset: 0x00076B4C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___targetNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.target = reader.ReadGameObject();
			}
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x0001150C File Offset: 0x0000F70C
		public override void PreStartClient()
		{
			if (!this.___targetNetId.IsEmpty())
			{
				this.Networktarget = ClientScene.FindLocalObject(this.___targetNetId);
			}
		}

		// Token: 0x040019F6 RID: 6646
		[SyncVar]
		[NonSerialized]
		public GameObject target;

		// Token: 0x040019F7 RID: 6647
		public ParticleSystem bodyGlowParticles;

		// Token: 0x040019F8 RID: 6648
		public static string tpOutSoundString = "Play_UI_teleport_off_map";

		// Token: 0x040019F9 RID: 6649
		private float age;

		// Token: 0x040019FA RID: 6650
		private const float warmupDuration = 2f;

		// Token: 0x040019FB RID: 6651
		private NetworkInstanceId ___targetNetId;
	}
}
