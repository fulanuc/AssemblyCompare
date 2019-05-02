using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000274 RID: 628
	public class BurnEffectController : MonoBehaviour
	{
		// Token: 0x06000BD0 RID: 3024 RVA: 0x0004D100 File Offset: 0x0004B300
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			BurnEffectController.normalEffect = new BurnEffectController.EffectParams
			{
				startSound = "Play_item_proc_igniteOnKill_Loop",
				stopSound = "Stop_item_proc_igniteOnKill_Loop",
				overlayMaterial = Resources.Load<Material>("Materials/matOnFire"),
				fireEffectPrefab = Resources.Load<GameObject>("Prefabs/FireEffect")
			};
			BurnEffectController.helfireEffect = new BurnEffectController.EffectParams
			{
				startSound = "Play_item_proc_igniteOnKill_Loop",
				stopSound = "Stop_item_proc_igniteOnKill_Loop",
				overlayMaterial = Resources.Load<Material>("Materials/matOnHelfire"),
				fireEffectPrefab = Resources.Load<GameObject>("Prefabs/HelfireEffect")
			};
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0004D190 File Offset: 0x0004B390
		private void Start()
		{
			Util.PlaySound(this.effectType.startSound, base.gameObject);
			this.particles = new List<GameObject>();
			this.temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
			this.temporaryOverlay.originalMaterial = this.effectType.overlayMaterial;
			CharacterModel component = this.target.GetComponent<CharacterModel>();
			if (component)
			{
				if (this.temporaryOverlay)
				{
					this.temporaryOverlay.AddToCharacerModel(component);
				}
				CharacterModel.RendererInfo[] rendererInfos = component.rendererInfos;
				for (int i = 0; i < rendererInfos.Length; i++)
				{
					if (!rendererInfos[i].ignoreOverlays)
					{
						GameObject gameObject = this.AddFireParticles(rendererInfos[i].renderer, this.target);
						if (gameObject)
						{
							this.particles.Add(gameObject);
						}
					}
				}
			}
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0004D264 File Offset: 0x0004B464
		private void OnDestroy()
		{
			Util.PlaySound(this.effectType.stopSound, base.gameObject);
			if (this.temporaryOverlay)
			{
				UnityEngine.Object.Destroy(this.temporaryOverlay);
			}
			for (int i = 0; i < this.particles.Count; i++)
			{
				if (this.particles[i])
				{
					this.particles[i].GetComponent<ParticleSystem>().emission.enabled = false;
					this.particles[i].GetComponent<DestroyOnTimer>().enabled = true;
					this.particles[i].GetComponentInChildren<LightIntensityCurve>().enabled = true;
				}
			}
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0004D318 File Offset: 0x0004B518
		private GameObject AddFireParticles(Renderer modelRenderer, GameObject target)
		{
			if (modelRenderer is MeshRenderer || modelRenderer is SkinnedMeshRenderer)
			{
				GameObject fireEffectPrefab = this.effectType.fireEffectPrefab;
				fireEffectPrefab.GetComponent<ParticleSystem>();
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fireEffectPrefab, modelRenderer.transform);
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				ParticleSystem.ShapeModule shape = component.shape;
				if (modelRenderer)
				{
					if (modelRenderer is MeshRenderer)
					{
						shape.shapeType = ParticleSystemShapeType.MeshRenderer;
						shape.meshRenderer = (MeshRenderer)modelRenderer;
					}
					else if (modelRenderer is SkinnedMeshRenderer)
					{
						shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
						shape.skinnedMeshRenderer = (SkinnedMeshRenderer)modelRenderer;
					}
				}
				ParticleSystem.MainModule main = component.main;
				Vector3 lossyScale = modelRenderer.transform.lossyScale;
				component.gameObject.SetActive(true);
				BoneParticleController component2 = gameObject.GetComponent<BoneParticleController>();
				if (component2 && modelRenderer is SkinnedMeshRenderer)
				{
					component2.skinnedMeshRenderer = (SkinnedMeshRenderer)modelRenderer;
				}
				return gameObject;
			}
			return null;
		}

		// Token: 0x04000FBD RID: 4029
		private List<GameObject> particles;

		// Token: 0x04000FBE RID: 4030
		public GameObject target;

		// Token: 0x04000FBF RID: 4031
		private TemporaryOverlay temporaryOverlay;

		// Token: 0x04000FC0 RID: 4032
		private int soundID;

		// Token: 0x04000FC1 RID: 4033
		public BurnEffectController.EffectParams effectType = BurnEffectController.normalEffect;

		// Token: 0x04000FC2 RID: 4034
		public static BurnEffectController.EffectParams normalEffect;

		// Token: 0x04000FC3 RID: 4035
		public static BurnEffectController.EffectParams helfireEffect;

		// Token: 0x04000FC4 RID: 4036
		public float fireParticleSize = 5f;

		// Token: 0x02000275 RID: 629
		public class EffectParams
		{
			// Token: 0x04000FC5 RID: 4037
			public string startSound;

			// Token: 0x04000FC6 RID: 4038
			public string stopSound;

			// Token: 0x04000FC7 RID: 4039
			public Material overlayMaterial;

			// Token: 0x04000FC8 RID: 4040
			public GameObject fireEffectPrefab;
		}
	}
}
