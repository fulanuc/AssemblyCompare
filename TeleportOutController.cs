using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FB RID: 1019
	public class TeleportOutController : NetworkBehaviour
	{
		// Token: 0x060016BE RID: 5822 RVA: 0x00078148 File Offset: 0x00076348
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

		// Token: 0x060016BF RID: 5823 RVA: 0x000781AC File Offset: 0x000763AC
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

		// Token: 0x060016C0 RID: 5824 RVA: 0x000782B4 File Offset: 0x000764B4
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

		// Token: 0x060016C3 RID: 5827 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060016C4 RID: 5828 RVA: 0x00078340 File Offset: 0x00076540
		// (set) Token: 0x060016C5 RID: 5829 RVA: 0x000110CD File Offset: 0x0000F2CD
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

		// Token: 0x060016C6 RID: 5830 RVA: 0x00078354 File Offset: 0x00076554
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

		// Token: 0x060016C7 RID: 5831 RVA: 0x000783C0 File Offset: 0x000765C0
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

		// Token: 0x060016C8 RID: 5832 RVA: 0x000110E7 File Offset: 0x0000F2E7
		public override void PreStartClient()
		{
			if (!this.___targetNetId.IsEmpty())
			{
				this.Networktarget = ClientScene.FindLocalObject(this.___targetNetId);
			}
		}

		// Token: 0x040019CD RID: 6605
		[SyncVar]
		[NonSerialized]
		public GameObject target;

		// Token: 0x040019CE RID: 6606
		public ParticleSystem bodyGlowParticles;

		// Token: 0x040019CF RID: 6607
		public static string tpOutSoundString = "Play_UI_teleport_off_map";

		// Token: 0x040019D0 RID: 6608
		private float age;

		// Token: 0x040019D1 RID: 6609
		private const float warmupDuration = 2f;

		// Token: 0x040019D2 RID: 6610
		private NetworkInstanceId ___targetNetId;
	}
}
