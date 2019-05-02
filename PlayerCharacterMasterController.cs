using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038F RID: 911
	[RequireComponent(typeof(PingerController))]
	[RequireComponent(typeof(CharacterMaster))]
	public class PlayerCharacterMasterController : NetworkBehaviour
	{
		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0000EA0B File Offset: 0x0000CC0B
		public static ReadOnlyCollection<PlayerCharacterMasterController> instances
		{
			get
			{
				return PlayerCharacterMasterController._instancesReadOnly;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06001317 RID: 4887 RVA: 0x0000EA12 File Offset: 0x0000CC12
		// (set) Token: 0x06001318 RID: 4888 RVA: 0x0000EA1A File Offset: 0x0000CC1A
		public CharacterMaster master { get; private set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001319 RID: 4889 RVA: 0x0000EA23 File Offset: 0x0000CC23
		public bool hasEffectiveAuthority
		{
			get
			{
				return this.master.hasEffectiveAuthority;
			}
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0000EA30 File Offset: 0x0000CC30
		private void OnSyncNetworkUserInstanceId(NetworkInstanceId value)
		{
			this.resolvedNetworkUserGameObjectInstance = null;
			this.resolvedNetworkUserInstance = null;
			this.networkUserResolved = (value == NetworkInstanceId.Invalid);
			this.NetworknetworkUserInstanceId = value;
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600131B RID: 4891 RVA: 0x0006B4EC File Offset: 0x000696EC
		// (set) Token: 0x0600131C RID: 4892 RVA: 0x0006B544 File Offset: 0x00069744
		public GameObject networkUserObject
		{
			get
			{
				if (!this.networkUserResolved)
				{
					this.resolvedNetworkUserGameObjectInstance = Util.FindNetworkObject(this.networkUserInstanceId);
					this.resolvedNetworkUserInstance = null;
					if (this.resolvedNetworkUserGameObjectInstance)
					{
						this.resolvedNetworkUserInstance = this.resolvedNetworkUserGameObjectInstance.GetComponent<NetworkUser>();
						this.networkUserResolved = true;
					}
				}
				return this.resolvedNetworkUserGameObjectInstance;
			}
			set
			{
				NetworkInstanceId networknetworkUserInstanceId = NetworkInstanceId.Invalid;
				this.resolvedNetworkUserGameObjectInstance = null;
				this.resolvedNetworkUserInstance = null;
				this.networkUserResolved = true;
				if (value)
				{
					NetworkIdentity component = value.GetComponent<NetworkIdentity>();
					if (component)
					{
						networknetworkUserInstanceId = component.netId;
						this.resolvedNetworkUserGameObjectInstance = value;
						this.resolvedNetworkUserInstance = value.GetComponent<NetworkUser>();
					}
				}
				this.NetworknetworkUserInstanceId = networknetworkUserInstanceId;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600131D RID: 4893 RVA: 0x0000EA58 File Offset: 0x0000CC58
		public NetworkUser networkUser
		{
			get
			{
				if (!this.networkUserObject)
				{
					return null;
				}
				return this.resolvedNetworkUserInstance;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600131E RID: 4894 RVA: 0x0000EA6F File Offset: 0x0000CC6F
		public bool isConnected
		{
			get
			{
				return this.networkUserObject;
			}
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x0000EA7C File Offset: 0x0000CC7C
		private void Awake()
		{
			this.master = base.GetComponent<CharacterMaster>();
			this.netid = base.GetComponent<NetworkIdentity>();
			this.pingerController = base.GetComponent<PingerController>();
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x0000EAA2 File Offset: 0x0000CCA2
		private void OnEnable()
		{
			PlayerCharacterMasterController._instances.Add(this);
			if (PlayerCharacterMasterController.onPlayerAdded != null)
			{
				PlayerCharacterMasterController.onPlayerAdded(this);
			}
		}

		// Token: 0x06001321 RID: 4897 RVA: 0x0000EAC1 File Offset: 0x0000CCC1
		private void OnDisable()
		{
			PlayerCharacterMasterController._instances.Remove(this);
			if (PlayerCharacterMasterController.onPlayerRemoved != null)
			{
				PlayerCharacterMasterController.onPlayerRemoved(this);
			}
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x0000EAE1 File Offset: 0x0000CCE1
		private void Start()
		{
			if (NetworkServer.active && this.networkUser)
			{
				this.CallRpcIncrementRunCount();
			}
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0006B5A4 File Offset: 0x000697A4
		[ClientRpc]
		private void RpcIncrementRunCount()
		{
			if (this.networkUser)
			{
				LocalUser localUser = this.networkUser.localUser;
				if (localUser != null)
				{
					localUser.userProfile.totalRunCount += 1u;
				}
			}
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0006B5E0 File Offset: 0x000697E0
		private void Update()
		{
			if (this.netid.hasAuthority)
			{
				this.SetBody(this.master.GetBodyObject());
				NetworkUser networkUser = this.networkUser;
				if (this.bodyInputs && networkUser && networkUser.inputPlayer != null)
				{
					this.sprintInputPressReceived |= networkUser.inputPlayer.GetButtonDown("Sprint");
					CameraRigController cameraRigController = networkUser.cameraRigController;
					if (cameraRigController)
					{
						if (networkUser.localUser != null && !networkUser.localUser.isUIFocused)
						{
							Vector2 vector = new Vector2(networkUser.inputPlayer.GetAxis("MoveHorizontal"), networkUser.inputPlayer.GetAxis("MoveVertical"));
							float sqrMagnitude = vector.sqrMagnitude;
							if (sqrMagnitude > 1f)
							{
								vector /= Mathf.Sqrt(sqrMagnitude);
							}
							if (this.bodyIsFlier)
							{
								this.bodyInputs.moveVector = cameraRigController.transform.right * vector.x + cameraRigController.transform.forward * vector.y;
							}
							else
							{
								float y = cameraRigController.transform.eulerAngles.y;
								this.bodyInputs.moveVector = Quaternion.Euler(0f, y, 0f) * new Vector3(vector.x, 0f, vector.y);
							}
						}
						else
						{
							this.bodyInputs.moveVector = Vector3.zero;
						}
						this.bodyInputs.aimDirection = (cameraRigController.crosshairWorldPosition - this.bodyInputs.aimOrigin).normalized;
					}
					CharacterEmoteDefinitions component = this.bodyInputs.GetComponent<CharacterEmoteDefinitions>();
					if (component)
					{
						if (Input.GetKeyDown("g"))
						{
							this.bodyInputs.emoteRequest = component.FindEmoteIndex("Point");
							return;
						}
						if (Input.GetKeyDown("t"))
						{
							this.bodyInputs.emoteRequest = component.FindEmoteIndex("Surprise");
						}
					}
				}
			}
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0006B7F0 File Offset: 0x000699F0
		private void FixedUpdate()
		{
			NetworkUser networkUser = this.networkUser;
			if (this.bodyInputs)
			{
				if (networkUser && networkUser.localUser != null && !networkUser.localUser.isUIFocused)
				{
					Player inputPlayer = networkUser.localUser.inputPlayer;
					bool flag = false;
					if (this.body)
					{
						flag = this.body.isSprinting;
						if (this.sprintInputPressReceived)
						{
							this.sprintInputPressReceived = false;
							flag = !flag;
						}
						if (flag)
						{
							Vector3 aimDirection = this.bodyInputs.aimDirection;
							aimDirection.y = 0f;
							aimDirection.Normalize();
							Vector3 moveVector = this.bodyInputs.moveVector;
							moveVector.y = 0f;
							moveVector.Normalize();
							if ((this.body.bodyFlags & CharacterBody.BodyFlags.SprintAnyDirection) == CharacterBody.BodyFlags.None && Vector3.Dot(aimDirection, moveVector) < PlayerCharacterMasterController.sprintMinAimMoveDot)
							{
								flag = false;
							}
						}
					}
					this.bodyInputs.skill1.PushState(inputPlayer.GetButton("PrimarySkill"));
					this.bodyInputs.skill2.PushState(inputPlayer.GetButton("SecondarySkill"));
					this.bodyInputs.skill3.PushState(inputPlayer.GetButton("UtilitySkill"));
					this.bodyInputs.skill4.PushState(inputPlayer.GetButton("SpecialSkill"));
					this.bodyInputs.interact.PushState(inputPlayer.GetButton("Interact"));
					this.bodyInputs.jump.PushState(inputPlayer.GetButton("Jump"));
					this.bodyInputs.sprint.PushState(flag);
					this.bodyInputs.activateEquipment.PushState(inputPlayer.GetButton("Equipment"));
					this.bodyInputs.ping.PushState(inputPlayer.GetButton("Ping"));
				}
				else
				{
					this.bodyInputs.skill1.PushState(false);
					this.bodyInputs.skill2.PushState(false);
					this.bodyInputs.skill3.PushState(false);
					this.bodyInputs.skill4.PushState(false);
					this.bodyInputs.interact.PushState(false);
					this.bodyInputs.jump.PushState(false);
					this.bodyInputs.sprint.PushState(false);
					this.bodyInputs.activateEquipment.PushState(false);
					this.bodyInputs.ping.PushState(false);
				}
				this.CheckPinging();
			}
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x0006BA68 File Offset: 0x00069C68
		private void CheckPinging()
		{
			if (this.hasEffectiveAuthority && this.body && this.bodyInputs && this.bodyInputs.ping.justPressed)
			{
				this.pingerController.AttemptPing(new Ray(this.bodyInputs.aimOrigin, this.bodyInputs.aimDirection), this.body.gameObject);
			}
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0006BADC File Offset: 0x00069CDC
		public string GetDisplayName()
		{
			string result = "";
			if (this.networkUserObject)
			{
				NetworkUser component = this.networkUserObject.GetComponent<NetworkUser>();
				if (component)
				{
					result = component.userName;
				}
			}
			return result;
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0006BB18 File Offset: 0x00069D18
		private void SetBody(GameObject newBody)
		{
			if (newBody)
			{
				this.body = newBody.GetComponent<CharacterBody>();
				this.bodyInputs = newBody.GetComponent<InputBankTest>();
				this.bodyIsFlier = newBody.GetComponent<RigidbodyMotor>();
				return;
			}
			this.body = null;
			this.bodyInputs = null;
			this.bodyIsFlier = false;
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06001329 RID: 4905 RVA: 0x0000EAFD File Offset: 0x0000CCFD
		public bool preventGameOver
		{
			get
			{
				return this.master.preventGameOver;
			}
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0000EB0A File Offset: 0x0000CD0A
		[Server]
		public void OnBodyDeath()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PlayerCharacterMasterController::OnBodyDeath()' called on client");
				return;
			}
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnBodyStart()
		{
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x0600132C RID: 4908 RVA: 0x0006BB6C File Offset: 0x00069D6C
		// (remove) Token: 0x0600132D RID: 4909 RVA: 0x0006BBA0 File Offset: 0x00069DA0
		public static event Action<PlayerCharacterMasterController> onPlayerAdded;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x0600132E RID: 4910 RVA: 0x0006BBD4 File Offset: 0x00069DD4
		// (remove) Token: 0x0600132F RID: 4911 RVA: 0x0006BC08 File Offset: 0x00069E08
		public static event Action<PlayerCharacterMasterController> onPlayerRemoved;

		// Token: 0x06001330 RID: 4912 RVA: 0x0000EB21 File Offset: 0x0000CD21
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				GameObject attacker = damageReport.damageInfo.attacker;
				if (attacker)
				{
					CharacterBody component = attacker.GetComponent<CharacterBody>();
					if (component)
					{
						GameObject masterObject = component.masterObject;
						if (masterObject)
						{
							PlayerCharacterMasterController component2 = masterObject.GetComponent<PlayerCharacterMasterController>();
							if (component2 && Util.CheckRoll(1f * component2.lunarCoinChanceMultiplier, 0f, null))
							{
								PickupDropletController.CreatePickupDroplet(PickupIndex.lunarCoin1, damageReport.victim.transform.position, Vector3.up * 10f);
								component2.lunarCoinChanceMultiplier *= 0.5f;
							}
						}
					}
				}
			};
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0006BC3C File Offset: 0x00069E3C
		static PlayerCharacterMasterController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerCharacterMasterController), PlayerCharacterMasterController.kRpcRpcIncrementRunCount, new NetworkBehaviour.CmdDelegate(PlayerCharacterMasterController.InvokeRpcRpcIncrementRunCount));
			NetworkCRC.RegisterBehaviour("PlayerCharacterMasterController", 0);
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x0006BCAC File Offset: 0x00069EAC
		// (set) Token: 0x06001335 RID: 4917 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public NetworkInstanceId NetworknetworkUserInstanceId
		{
			get
			{
				return this.networkUserInstanceId;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncNetworkUserInstanceId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkInstanceId>(value, ref this.networkUserInstanceId, dirtyBit);
			}
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x0000EBAF File Offset: 0x0000CDAF
		protected static void InvokeRpcRpcIncrementRunCount(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcIncrementRunCount called on server.");
				return;
			}
			((PlayerCharacterMasterController)obj).RpcIncrementRunCount();
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x0006BCC0 File Offset: 0x00069EC0
		public void CallRpcIncrementRunCount()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcIncrementRunCount called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)PlayerCharacterMasterController.kRpcRpcIncrementRunCount);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcIncrementRunCount");
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0006BD2C File Offset: 0x00069F2C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.networkUserInstanceId);
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
				writer.Write(this.networkUserInstanceId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x0006BD98 File Offset: 0x00069F98
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.networkUserInstanceId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncNetworkUserInstanceId(reader.ReadNetworkId());
			}
		}

		// Token: 0x040016D3 RID: 5843
		private static List<PlayerCharacterMasterController> _instances = new List<PlayerCharacterMasterController>();

		// Token: 0x040016D4 RID: 5844
		private static ReadOnlyCollection<PlayerCharacterMasterController> _instancesReadOnly = new ReadOnlyCollection<PlayerCharacterMasterController>(PlayerCharacterMasterController._instances);

		// Token: 0x040016D5 RID: 5845
		private CharacterBody body;

		// Token: 0x040016D6 RID: 5846
		private InputBankTest bodyInputs;

		// Token: 0x040016D7 RID: 5847
		private bool bodyIsFlier;

		// Token: 0x040016D9 RID: 5849
		private PingerController pingerController;

		// Token: 0x040016DA RID: 5850
		[SyncVar(hook = "OnSyncNetworkUserInstanceId")]
		private NetworkInstanceId networkUserInstanceId;

		// Token: 0x040016DB RID: 5851
		private GameObject resolvedNetworkUserGameObjectInstance;

		// Token: 0x040016DC RID: 5852
		private bool networkUserResolved;

		// Token: 0x040016DD RID: 5853
		private NetworkUser resolvedNetworkUserInstance;

		// Token: 0x040016DE RID: 5854
		public float cameraMinPitch = -70f;

		// Token: 0x040016DF RID: 5855
		public float cameraMaxPitch = 70f;

		// Token: 0x040016E0 RID: 5856
		public GameObject crosshair;

		// Token: 0x040016E1 RID: 5857
		public Vector3 crosshairPosition;

		// Token: 0x040016E2 RID: 5858
		private NetworkIdentity netid;

		// Token: 0x040016E3 RID: 5859
		private static readonly float sprintMinAimMoveDot = Mathf.Cos(1.04719758f);

		// Token: 0x040016E4 RID: 5860
		private bool sprintInputPressReceived;

		// Token: 0x040016E7 RID: 5863
		private float lunarCoinChanceMultiplier = 0.5f;

		// Token: 0x040016E8 RID: 5864
		private static int kRpcRpcIncrementRunCount = 1915650359;
	}
}
