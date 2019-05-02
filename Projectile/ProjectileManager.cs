using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000553 RID: 1363
	public class ProjectileManager : MonoBehaviour
	{
		// Token: 0x06001E6E RID: 7790 RVA: 0x00095F7C File Offset: 0x0009417C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			ProjectileManager.projectilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Projectiles/");
			Array.Sort<GameObject>(ProjectileManager.projectilePrefabs, (GameObject a, GameObject b) => string.CompareOrdinal(a.name, b.name));
			ProjectileManager.projectilePrefabProjectileControllers = (from prefab in ProjectileManager.projectilePrefabs
			select prefab.GetComponent<ProjectileController>()).ToArray<ProjectileController>();
			int num = 256;
			if (ProjectileManager.projectilePrefabs.Length > num)
			{
				Debug.LogErrorFormat("Cannot have more than {0} projectile prefabs defined, which is over the limit for {1}. Check comments at error source for details.", new object[]
				{
					num,
					typeof(byte).Name
				});
				for (int i = num; i < ProjectileManager.projectilePrefabs.Length; i++)
				{
					Debug.LogErrorFormat("Could not register projectile [{0}/{1}]=\"{2}\"", new object[]
					{
						i,
						num - 1,
						ProjectileManager.projectilePrefabs[i].name
					});
				}
			}
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x0001640E File Offset: 0x0001460E
		private void Awake()
		{
			this.predictionManager = new ProjectileManager.PredictionManager();
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0001641B File Offset: 0x0001461B
		private void OnDisable()
		{
			if (ProjectileManager.instance == this)
			{
				ProjectileManager.instance = null;
			}
		}

		// Token: 0x06001E71 RID: 7793 RVA: 0x00016430 File Offset: 0x00014630
		private void OnEnable()
		{
			if (ProjectileManager.instance == null)
			{
				ProjectileManager.instance = this;
				return;
			}
			Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time", new object[]
			{
				base.GetType().Name
			});
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x00016465 File Offset: 0x00014665
		[NetworkMessageHandler(msgType = 49, server = true)]
		private static void HandlePlayerFireProjectile(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandlePlayerFireProjectileInternal(netMsg);
			}
		}

		// Token: 0x06001E73 RID: 7795 RVA: 0x0001647E File Offset: 0x0001467E
		[NetworkMessageHandler(msgType = 50, client = true)]
		private static void HandleReleaseProjectilePredictionId(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandleReleaseProjectilePredictionIdInternal(netMsg);
			}
		}

		// Token: 0x06001E74 RID: 7796 RVA: 0x00016497 File Offset: 0x00014697
		private int FindProjectilePrefabIndex(GameObject prefab)
		{
			return Array.IndexOf<GameObject>(ProjectileManager.projectilePrefabs, prefab);
		}

		// Token: 0x06001E75 RID: 7797 RVA: 0x000164A4 File Offset: 0x000146A4
		private GameObject FindProjectilePrefabFromIndex(int projectilePrefabIndex)
		{
			if (projectilePrefabIndex < ProjectileManager.projectilePrefabs.Length)
			{
				return ProjectileManager.projectilePrefabs[projectilePrefabIndex];
			}
			return null;
		}

		// Token: 0x06001E76 RID: 7798 RVA: 0x00096074 File Offset: 0x00094274
		[Obsolete("Use the FireProjectileInfo overload of FireProjectile instead.")]
		public void FireProjectile(GameObject prefab, Vector3 position, Quaternion rotation, GameObject owner, float damage, float force, bool crit, DamageColorIndex damageColorIndex = DamageColorIndex.Default, GameObject target = null, float speedOverride = -1f)
		{
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = prefab,
				position = position,
				rotation = rotation,
				owner = owner,
				damage = damage,
				force = force,
				crit = crit,
				damageColorIndex = damageColorIndex,
				target = target,
				speedOverride = speedOverride,
				fuseOverride = -1f
			};
			this.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x06001E77 RID: 7799 RVA: 0x000164B9 File Offset: 0x000146B9
		public void FireProjectile(FireProjectileInfo fireProjectileInfo)
		{
			if (NetworkServer.active)
			{
				this.FireProjectileServer(fireProjectileInfo, null, 0, 0.0);
				return;
			}
			this.FireProjectileClient(fireProjectileInfo, NetworkManager.singleton.client);
		}

		// Token: 0x06001E78 RID: 7800 RVA: 0x000960F8 File Offset: 0x000942F8
		private void FireProjectileClient(FireProjectileInfo fireProjectileInfo, NetworkClient client)
		{
			int num = this.FindProjectilePrefabIndex(fireProjectileInfo.projectilePrefab);
			if (num == -1)
			{
				Debug.LogErrorFormat(fireProjectileInfo.projectilePrefab, "Prefab {0} is not a registered projectile prefab.", new object[]
				{
					fireProjectileInfo.projectilePrefab
				});
				return;
			}
			bool allowPrediction = ProjectileManager.projectilePrefabProjectileControllers[num].allowPrediction;
			ushort predictionId = 0;
			if (allowPrediction)
			{
				ProjectileController component = UnityEngine.Object.Instantiate<GameObject>(fireProjectileInfo.projectilePrefab, fireProjectileInfo.position, fireProjectileInfo.rotation).GetComponent<ProjectileController>();
				ProjectileManager.InitializeProjectile(component, fireProjectileInfo);
				this.predictionManager.RegisterPrediction(component);
				predictionId = component.predictionId;
			}
			this.fireMsg.sendTime = (double)Run.instance.time;
			this.fireMsg.prefabIndex = (byte)num;
			this.fireMsg.position = fireProjectileInfo.position;
			this.fireMsg.rotation = fireProjectileInfo.rotation;
			this.fireMsg.owner = fireProjectileInfo.owner;
			this.fireMsg.predictionId = predictionId;
			this.fireMsg.damage = fireProjectileInfo.damage;
			this.fireMsg.force = fireProjectileInfo.force;
			this.fireMsg.crit = fireProjectileInfo.crit;
			this.fireMsg.damageColorIndex = fireProjectileInfo.damageColorIndex;
			this.fireMsg.speedOverride = fireProjectileInfo.speedOverride;
			this.fireMsg.fuseOverride = fireProjectileInfo.fuseOverride;
			this.fireMsg.target = HurtBoxReference.FromRootObject(fireProjectileInfo.target);
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(49);
			networkWriter.Write(this.fireMsg);
			networkWriter.FinishMessage();
			client.SendWriter(networkWriter, 0);
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x00096288 File Offset: 0x00094488
		private static void InitializeProjectile(ProjectileController projectileController, FireProjectileInfo fireProjectileInfo)
		{
			GameObject gameObject = projectileController.gameObject;
			ProjectileDamage component = gameObject.GetComponent<ProjectileDamage>();
			TeamFilter component2 = gameObject.GetComponent<TeamFilter>();
			ProjectileNetworkTransform component3 = gameObject.GetComponent<ProjectileNetworkTransform>();
			MissileController component4 = gameObject.GetComponent<MissileController>();
			ProjectileSimple component5 = gameObject.GetComponent<ProjectileSimple>();
			projectileController.Networkowner = fireProjectileInfo.owner;
			projectileController.procChainMask = fireProjectileInfo.procChainMask;
			if (component2)
			{
				component2.teamIndex = TeamComponent.GetObjectTeam(fireProjectileInfo.owner);
			}
			if (component3)
			{
				component3.SetValuesFromTransform();
			}
			if (component4)
			{
				component4.target = (fireProjectileInfo.target ? fireProjectileInfo.target.transform : null);
			}
			if (fireProjectileInfo.useSpeedOverride && component5)
			{
				component5.velocity = fireProjectileInfo.speedOverride;
			}
			if (fireProjectileInfo.useFuseOverride)
			{
				ProjectileImpactExplosion component6 = gameObject.GetComponent<ProjectileImpactExplosion>();
				if (component6)
				{
					component6.lifetime = fireProjectileInfo.fuseOverride;
				}
				ProjectileFuse component7 = gameObject.GetComponent<ProjectileFuse>();
				if (component7)
				{
					component7.fuse = fireProjectileInfo.fuseOverride;
				}
			}
			if (component)
			{
				component.damage = fireProjectileInfo.damage;
				component.force = fireProjectileInfo.force;
				component.crit = fireProjectileInfo.crit;
				component.damageColorIndex = fireProjectileInfo.damageColorIndex;
			}
		}

		// Token: 0x06001E7A RID: 7802 RVA: 0x000963C8 File Offset: 0x000945C8
		private void FireProjectileServer(FireProjectileInfo fireProjectileInfo, NetworkConnection clientAuthorityOwner = null, ushort predictionId = 0, double fastForwardTime = 0.0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fireProjectileInfo.projectilePrefab, fireProjectileInfo.position, fireProjectileInfo.rotation);
			ProjectileController component = gameObject.GetComponent<ProjectileController>();
			component.NetworkpredictionId = predictionId;
			ProjectileManager.InitializeProjectile(component, fireProjectileInfo);
			if (clientAuthorityOwner != null)
			{
				NetworkServer.SpawnWithClientAuthority(gameObject, clientAuthorityOwner);
				return;
			}
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001E7B RID: 7803 RVA: 0x00096414 File Offset: 0x00094614
		public void OnServerProjectileDestroyed(ProjectileController projectile)
		{
			if (projectile.predictionId != 0)
			{
				NetworkConnection clientAuthorityOwner = projectile.clientAuthorityOwner;
				if (clientAuthorityOwner != null)
				{
					this.ReleasePredictionId(clientAuthorityOwner, projectile.predictionId);
				}
			}
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x000164E6 File Offset: 0x000146E6
		public void OnClientProjectileReceived(ProjectileController projectile)
		{
			if (projectile.predictionId != 0 && projectile.hasAuthority)
			{
				this.predictionManager.OnAuthorityProjectileReceived(projectile);
			}
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x00096440 File Offset: 0x00094640
		private void ReleasePredictionId(NetworkConnection owner, ushort predictionId)
		{
			this.releasePredictionIdMsg.predictionId = predictionId;
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(50);
			networkWriter.Write(this.releasePredictionIdMsg);
			networkWriter.FinishMessage();
			owner.SendWriter(networkWriter, 0);
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x00096484 File Offset: 0x00094684
		private void HandlePlayerFireProjectileInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.PlayerFireProjectileMessage>(this.fireMsg);
			GameObject gameObject = this.FindProjectilePrefabFromIndex((int)this.fireMsg.prefabIndex);
			if (gameObject == null)
			{
				this.ReleasePredictionId(netMsg.conn, this.fireMsg.predictionId);
				return;
			}
			FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
			fireProjectileInfo.projectilePrefab = gameObject;
			fireProjectileInfo.position = this.fireMsg.position;
			fireProjectileInfo.rotation = this.fireMsg.rotation;
			fireProjectileInfo.owner = this.fireMsg.owner;
			fireProjectileInfo.damage = this.fireMsg.damage;
			fireProjectileInfo.force = this.fireMsg.force;
			fireProjectileInfo.crit = this.fireMsg.crit;
			GameObject gameObject2 = this.fireMsg.target.ResolveGameObject();
			fireProjectileInfo.target = ((gameObject2 != null) ? gameObject2.gameObject : null);
			fireProjectileInfo.damageColorIndex = this.fireMsg.damageColorIndex;
			fireProjectileInfo.speedOverride = this.fireMsg.speedOverride;
			fireProjectileInfo.fuseOverride = this.fireMsg.fuseOverride;
			this.FireProjectileServer(fireProjectileInfo, netMsg.conn, this.fireMsg.predictionId, (double)Run.instance.time - this.fireMsg.sendTime);
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x00016504 File Offset: 0x00014704
		private void HandleReleaseProjectilePredictionIdInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.ReleasePredictionIdMessage>(this.releasePredictionIdMsg);
			this.predictionManager.ReleasePredictionId(this.releasePredictionIdMsg.predictionId);
		}

		// Token: 0x06001E80 RID: 7808 RVA: 0x000965D4 File Offset: 0x000947D4
		[ConCommand(commandName = "dump_projectile_map", flags = ConVarFlags.None, helpText = "Dumps the map between indices and projectile prefabs.")]
		private static void DumpProjectileMap(ConCommandArgs args)
		{
			string[] array = new string[ProjectileManager.projectilePrefabs.Length];
			for (int i = 0; i < ProjectileManager.projectilePrefabs.Length; i++)
			{
				array[i] = string.Format("[{0}] = {1}", i, ProjectileManager.projectilePrefabs[i].name);
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x04002106 RID: 8454
		public static ProjectileManager instance;

		// Token: 0x04002107 RID: 8455
		private static GameObject[] projectilePrefabs;

		// Token: 0x04002108 RID: 8456
		private static ProjectileController[] projectilePrefabProjectileControllers;

		// Token: 0x04002109 RID: 8457
		private ProjectileManager.PredictionManager predictionManager;

		// Token: 0x0400210A RID: 8458
		private ProjectileManager.PlayerFireProjectileMessage fireMsg = new ProjectileManager.PlayerFireProjectileMessage();

		// Token: 0x0400210B RID: 8459
		private ProjectileManager.ReleasePredictionIdMessage releasePredictionIdMsg = new ProjectileManager.ReleasePredictionIdMessage();

		// Token: 0x02000554 RID: 1364
		private class PlayerFireProjectileMessage : MessageBase
		{
			// Token: 0x06001E84 RID: 7812 RVA: 0x00096630 File Offset: 0x00094830
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sendTime);
				writer.WritePackedUInt32((uint)this.prefabIndex);
				writer.Write(this.position);
				writer.Write(this.rotation);
				writer.Write(this.owner);
				GeneratedNetworkCode._WriteHurtBoxReference_None(writer, this.target);
				writer.Write(this.damage);
				writer.Write(this.force);
				writer.Write(this.crit);
				writer.WritePackedUInt32((uint)this.predictionId);
				writer.Write((int)this.damageColorIndex);
				writer.Write(this.speedOverride);
				writer.Write(this.fuseOverride);
			}

			// Token: 0x06001E85 RID: 7813 RVA: 0x000966DC File Offset: 0x000948DC
			public override void Deserialize(NetworkReader reader)
			{
				this.sendTime = reader.ReadDouble();
				this.prefabIndex = (byte)reader.ReadPackedUInt32();
				this.position = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
				this.owner = reader.ReadGameObject();
				this.target = GeneratedNetworkCode._ReadHurtBoxReference_None(reader);
				this.damage = reader.ReadSingle();
				this.force = reader.ReadSingle();
				this.crit = reader.ReadBoolean();
				this.predictionId = (ushort)reader.ReadPackedUInt32();
				this.damageColorIndex = (DamageColorIndex)reader.ReadInt32();
				this.speedOverride = reader.ReadSingle();
				this.fuseOverride = reader.ReadSingle();
			}

			// Token: 0x0400210C RID: 8460
			public double sendTime;

			// Token: 0x0400210D RID: 8461
			public byte prefabIndex;

			// Token: 0x0400210E RID: 8462
			public Vector3 position;

			// Token: 0x0400210F RID: 8463
			public Quaternion rotation;

			// Token: 0x04002110 RID: 8464
			public GameObject owner;

			// Token: 0x04002111 RID: 8465
			public HurtBoxReference target;

			// Token: 0x04002112 RID: 8466
			public float damage;

			// Token: 0x04002113 RID: 8467
			public float force;

			// Token: 0x04002114 RID: 8468
			public bool crit;

			// Token: 0x04002115 RID: 8469
			public ushort predictionId;

			// Token: 0x04002116 RID: 8470
			public DamageColorIndex damageColorIndex;

			// Token: 0x04002117 RID: 8471
			public float speedOverride;

			// Token: 0x04002118 RID: 8472
			public float fuseOverride;
		}

		// Token: 0x02000555 RID: 1365
		private class ReleasePredictionIdMessage : MessageBase
		{
			// Token: 0x06001E87 RID: 7815 RVA: 0x00016546 File Offset: 0x00014746
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
			}

			// Token: 0x06001E88 RID: 7816 RVA: 0x00016554 File Offset: 0x00014754
			public override void Deserialize(NetworkReader reader)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
			}

			// Token: 0x04002119 RID: 8473
			public ushort predictionId;
		}

		// Token: 0x02000556 RID: 1366
		private class PredictionManager
		{
			// Token: 0x06001E89 RID: 7817 RVA: 0x00016562 File Offset: 0x00014762
			public ProjectileController FindPredictedProjectileController(ushort predictionId)
			{
				return this.predictions[predictionId];
			}

			// Token: 0x06001E8A RID: 7818 RVA: 0x00096788 File Offset: 0x00094988
			public void OnAuthorityProjectileReceived(ProjectileController authoritativeProjectile)
			{
				ProjectileController projectileController;
				if (authoritativeProjectile.hasAuthority && authoritativeProjectile.predictionId != 0 && this.predictions.TryGetValue(authoritativeProjectile.predictionId, out projectileController))
				{
					authoritativeProjectile.ghost = projectileController.ghost;
					if (authoritativeProjectile.ghost)
					{
						authoritativeProjectile.ghost.authorityTransform = authoritativeProjectile.transform;
					}
				}
			}

			// Token: 0x06001E8B RID: 7819 RVA: 0x000967E4 File Offset: 0x000949E4
			public void ReleasePredictionId(ushort predictionId)
			{
				ProjectileController projectileController = this.predictions[predictionId];
				this.predictions.Remove(predictionId);
				if (projectileController && projectileController.gameObject)
				{
					UnityEngine.Object.Destroy(projectileController.gameObject);
				}
			}

			// Token: 0x06001E8C RID: 7820 RVA: 0x00016570 File Offset: 0x00014770
			public void RegisterPrediction(ProjectileController predictedProjectile)
			{
				predictedProjectile.NetworkpredictionId = this.RequestPredictionId();
				this.predictions[predictedProjectile.predictionId] = predictedProjectile;
				predictedProjectile.isPrediction = true;
			}

			// Token: 0x06001E8D RID: 7821 RVA: 0x0009682C File Offset: 0x00094A2C
			private ushort RequestPredictionId()
			{
				for (ushort num = 1; num < 32767; num += 1)
				{
					if (!this.predictions.ContainsKey(num))
					{
						return num;
					}
				}
				return 0;
			}

			// Token: 0x0400211A RID: 8474
			private Dictionary<ushort, ProjectileController> predictions = new Dictionary<ushort, ProjectileController>();
		}
	}
}
