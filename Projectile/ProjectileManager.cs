using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000562 RID: 1378
	public class ProjectileManager : MonoBehaviour
	{
		// Token: 0x06001ED8 RID: 7896 RVA: 0x00096C98 File Offset: 0x00094E98
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

		// Token: 0x06001ED9 RID: 7897 RVA: 0x000168ED File Offset: 0x00014AED
		private void Awake()
		{
			this.predictionManager = new ProjectileManager.PredictionManager();
		}

		// Token: 0x06001EDA RID: 7898 RVA: 0x000168FA File Offset: 0x00014AFA
		private void OnDisable()
		{
			if (ProjectileManager.instance == this)
			{
				ProjectileManager.instance = null;
			}
		}

		// Token: 0x06001EDB RID: 7899 RVA: 0x0001690F File Offset: 0x00014B0F
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

		// Token: 0x06001EDC RID: 7900 RVA: 0x00016944 File Offset: 0x00014B44
		[NetworkMessageHandler(msgType = 49, server = true)]
		private static void HandlePlayerFireProjectile(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandlePlayerFireProjectileInternal(netMsg);
			}
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x0001695D File Offset: 0x00014B5D
		[NetworkMessageHandler(msgType = 50, client = true)]
		private static void HandleReleaseProjectilePredictionId(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandleReleaseProjectilePredictionIdInternal(netMsg);
			}
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x00016976 File Offset: 0x00014B76
		private int FindProjectilePrefabIndex(GameObject prefab)
		{
			return Array.IndexOf<GameObject>(ProjectileManager.projectilePrefabs, prefab);
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x00016983 File Offset: 0x00014B83
		private GameObject FindProjectilePrefabFromIndex(int projectilePrefabIndex)
		{
			if (projectilePrefabIndex < ProjectileManager.projectilePrefabs.Length)
			{
				return ProjectileManager.projectilePrefabs[projectilePrefabIndex];
			}
			return null;
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x00096D90 File Offset: 0x00094F90
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

		// Token: 0x06001EE1 RID: 7905 RVA: 0x00016998 File Offset: 0x00014B98
		public void FireProjectile(FireProjectileInfo fireProjectileInfo)
		{
			if (NetworkServer.active)
			{
				this.FireProjectileServer(fireProjectileInfo, null, 0, 0.0);
				return;
			}
			this.FireProjectileClient(fireProjectileInfo, NetworkManager.singleton.client);
		}

		// Token: 0x06001EE2 RID: 7906 RVA: 0x00096E14 File Offset: 0x00095014
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

		// Token: 0x06001EE3 RID: 7907 RVA: 0x00096FA4 File Offset: 0x000951A4
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

		// Token: 0x06001EE4 RID: 7908 RVA: 0x000970E4 File Offset: 0x000952E4
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

		// Token: 0x06001EE5 RID: 7909 RVA: 0x00097130 File Offset: 0x00095330
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

		// Token: 0x06001EE6 RID: 7910 RVA: 0x000169C5 File Offset: 0x00014BC5
		public void OnClientProjectileReceived(ProjectileController projectile)
		{
			if (projectile.predictionId != 0 && projectile.hasAuthority)
			{
				this.predictionManager.OnAuthorityProjectileReceived(projectile);
			}
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x0009715C File Offset: 0x0009535C
		private void ReleasePredictionId(NetworkConnection owner, ushort predictionId)
		{
			this.releasePredictionIdMsg.predictionId = predictionId;
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(50);
			networkWriter.Write(this.releasePredictionIdMsg);
			networkWriter.FinishMessage();
			owner.SendWriter(networkWriter, 0);
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x000971A0 File Offset: 0x000953A0
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

		// Token: 0x06001EE9 RID: 7913 RVA: 0x000169E3 File Offset: 0x00014BE3
		private void HandleReleaseProjectilePredictionIdInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.ReleasePredictionIdMessage>(this.releasePredictionIdMsg);
			this.predictionManager.ReleasePredictionId(this.releasePredictionIdMsg.predictionId);
		}

		// Token: 0x06001EEA RID: 7914 RVA: 0x000972F0 File Offset: 0x000954F0
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

		// Token: 0x04002144 RID: 8516
		public static ProjectileManager instance;

		// Token: 0x04002145 RID: 8517
		private static GameObject[] projectilePrefabs;

		// Token: 0x04002146 RID: 8518
		private static ProjectileController[] projectilePrefabProjectileControllers;

		// Token: 0x04002147 RID: 8519
		private ProjectileManager.PredictionManager predictionManager;

		// Token: 0x04002148 RID: 8520
		private ProjectileManager.PlayerFireProjectileMessage fireMsg = new ProjectileManager.PlayerFireProjectileMessage();

		// Token: 0x04002149 RID: 8521
		private ProjectileManager.ReleasePredictionIdMessage releasePredictionIdMsg = new ProjectileManager.ReleasePredictionIdMessage();

		// Token: 0x02000563 RID: 1379
		private class PlayerFireProjectileMessage : MessageBase
		{
			// Token: 0x06001EEE RID: 7918 RVA: 0x0009734C File Offset: 0x0009554C
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

			// Token: 0x06001EEF RID: 7919 RVA: 0x000973F8 File Offset: 0x000955F8
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

			// Token: 0x0400214A RID: 8522
			public double sendTime;

			// Token: 0x0400214B RID: 8523
			public byte prefabIndex;

			// Token: 0x0400214C RID: 8524
			public Vector3 position;

			// Token: 0x0400214D RID: 8525
			public Quaternion rotation;

			// Token: 0x0400214E RID: 8526
			public GameObject owner;

			// Token: 0x0400214F RID: 8527
			public HurtBoxReference target;

			// Token: 0x04002150 RID: 8528
			public float damage;

			// Token: 0x04002151 RID: 8529
			public float force;

			// Token: 0x04002152 RID: 8530
			public bool crit;

			// Token: 0x04002153 RID: 8531
			public ushort predictionId;

			// Token: 0x04002154 RID: 8532
			public DamageColorIndex damageColorIndex;

			// Token: 0x04002155 RID: 8533
			public float speedOverride;

			// Token: 0x04002156 RID: 8534
			public float fuseOverride;
		}

		// Token: 0x02000564 RID: 1380
		private class ReleasePredictionIdMessage : MessageBase
		{
			// Token: 0x06001EF1 RID: 7921 RVA: 0x00016A25 File Offset: 0x00014C25
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
			}

			// Token: 0x06001EF2 RID: 7922 RVA: 0x00016A33 File Offset: 0x00014C33
			public override void Deserialize(NetworkReader reader)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
			}

			// Token: 0x04002157 RID: 8535
			public ushort predictionId;
		}

		// Token: 0x02000565 RID: 1381
		private class PredictionManager
		{
			// Token: 0x06001EF3 RID: 7923 RVA: 0x00016A41 File Offset: 0x00014C41
			public ProjectileController FindPredictedProjectileController(ushort predictionId)
			{
				return this.predictions[predictionId];
			}

			// Token: 0x06001EF4 RID: 7924 RVA: 0x000974A4 File Offset: 0x000956A4
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

			// Token: 0x06001EF5 RID: 7925 RVA: 0x00097500 File Offset: 0x00095700
			public void ReleasePredictionId(ushort predictionId)
			{
				ProjectileController projectileController = this.predictions[predictionId];
				this.predictions.Remove(predictionId);
				if (projectileController && projectileController.gameObject)
				{
					UnityEngine.Object.Destroy(projectileController.gameObject);
				}
			}

			// Token: 0x06001EF6 RID: 7926 RVA: 0x00016A4F File Offset: 0x00014C4F
			public void RegisterPrediction(ProjectileController predictedProjectile)
			{
				predictedProjectile.NetworkpredictionId = this.RequestPredictionId();
				this.predictions[predictedProjectile.predictionId] = predictedProjectile;
				predictedProjectile.isPrediction = true;
			}

			// Token: 0x06001EF7 RID: 7927 RVA: 0x00097548 File Offset: 0x00095748
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

			// Token: 0x04002158 RID: 8536
			private Dictionary<ushort, ProjectileController> predictions = new Dictionary<ushort, ProjectileController>();
		}
	}
}
