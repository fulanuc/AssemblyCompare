using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F9 RID: 761
	[RequireComponent(typeof(VoteController))]
	public class GameOverController : NetworkBehaviour
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000F6B RID: 3947 RVA: 0x0000BD00 File Offset: 0x00009F00
		// (set) Token: 0x06000F6C RID: 3948 RVA: 0x0000BD07 File Offset: 0x00009F07
		public static GameOverController instance { get; private set; }

		// Token: 0x06000F6D RID: 3949 RVA: 0x0000BD0F File Offset: 0x00009F0F
		[Server]
		public void SetRunReport([NotNull] RunReport newRunReport)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GameOverController::SetRunReport(RoR2.RunReport)' called on client");
				return;
			}
			base.SetDirtyBit(1u);
			this.runReport = newRunReport;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0005D0D8 File Offset: 0x0005B2D8
		private void GenerateReportScreens()
		{
			GameOverController.<>c__DisplayClass11_0 CS$<>8__locals1 = new GameOverController.<>c__DisplayClass11_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.voteControllerGameObject = base.gameObject;
			VoteController component = CS$<>8__locals1.voteControllerGameObject.GetComponent<VoteController>();
			using (IEnumerator<LocalUser> enumerator = LocalUserManager.readOnlyLocalUsersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LocalUser localUser = enumerator.Current;
					CameraRigController cameraRigController = CameraRigController.readOnlyInstancesList.FirstOrDefault((CameraRigController v) => v.viewer == localUser.currentNetworkUser);
					if (cameraRigController && cameraRigController.hud)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameEndReportPanelPrefab, cameraRigController.hud.transform);
						cameraRigController.hud.mainUIPanel.SetActive(false);
						gameObject.transform.parent = cameraRigController.hud.transform;
						gameObject.GetComponent<MPEventSystemProvider>().eventSystem = localUser.eventSystem;
						GameEndReportPanelController component2 = gameObject.GetComponent<GameEndReportPanelController>();
						GameEndReportPanelController.DisplayData displayData = new GameEndReportPanelController.DisplayData
						{
							runReport = this.runReport,
							playerIndex = CS$<>8__locals1.<GenerateReportScreens>g__FindPlayerIndex|0(localUser)
						};
						component2.SetDisplayData(displayData);
						component2.continueButton.onClick.AddListener(delegate()
						{
							if (localUser.currentNetworkUser)
							{
								localUser.currentNetworkUser.CallCmdSubmitVote(CS$<>8__locals1.voteControllerGameObject, 0);
							}
						});
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/VoteInfoPanel"), (RectTransform)component2.continueButton.transform.parent);
						gameObject2.transform.SetAsFirstSibling();
						gameObject2.GetComponent<VoteInfoPanelController>().voteController = component;
					}
				}
			}
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0000BD34 File Offset: 0x00009F34
		private void Start()
		{
			this.appearanceTimer = this.appearanceDelay;
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0000BD42 File Offset: 0x00009F42
		private void OnEnable()
		{
			GameOverController.instance = SingletonHelper.Assign<GameOverController>(GameOverController.instance, this);
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x0000BD54 File Offset: 0x00009F54
		private void OnDisable()
		{
			GameOverController.instance = SingletonHelper.Unassign<GameOverController>(GameOverController.instance, this);
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x0000BD66 File Offset: 0x00009F66
		private void Update()
		{
			if (!this.reportScreensGenerated)
			{
				this.appearanceTimer -= Time.deltaTime;
				if (this.appearanceTimer <= 0f)
				{
					this.reportScreensGenerated = true;
					this.GenerateReportScreens();
				}
			}
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x0000BD9C File Offset: 0x00009F9C
		[Server]
		private void EndRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GameOverController::EndRun()' called on client");
				return;
			}
			UnityEngine.Object.Destroy(Run.instance);
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0005D28C File Offset: 0x0005B48C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1u;
			}
			bool flag = (num & 1u) > 0u;
			if (!initialState)
			{
				writer.Write((byte)num);
			}
			if (flag)
			{
				this.runReport.Write(writer);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x0000BDBD File Offset: 0x00009FBD
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (((initialState ? 1 : reader.ReadByte()) & 1) > 0)
			{
				this.runReport.Read(reader);
			}
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0000BDDE File Offset: 0x00009FDE
		[ClientRpc]
		public void RpcClientGameOver()
		{
			if (Run.instance)
			{
				Run.instance.OnClientGameOver(this.runReport);
			}
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x0000BE1A File Offset: 0x0000A01A
		protected static void InvokeRpcRpcClientGameOver(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcClientGameOver called on server.");
				return;
			}
			((GameOverController)obj).RpcClientGameOver();
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0005D2D0 File Offset: 0x0005B4D0
		public void CallRpcClientGameOver()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcClientGameOver called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)GameOverController.kRpcRpcClientGameOver);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcClientGameOver");
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0000BE3D File Offset: 0x0000A03D
		static GameOverController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(GameOverController), GameOverController.kRpcRpcClientGameOver, new NetworkBehaviour.CmdDelegate(GameOverController.InvokeRpcRpcClientGameOver));
			NetworkCRC.RegisterBehaviour("GameOverController", 0);
		}

		// Token: 0x04001384 RID: 4996
		[Tooltip("How long it takes after the first person has hit the continue button for the game to forcibly end.")]
		public float timeoutDuration;

		// Token: 0x04001385 RID: 4997
		private const uint runReportDirtyBit = 1u;

		// Token: 0x04001386 RID: 4998
		private const uint allDirtyBits = 1u;

		// Token: 0x04001387 RID: 4999
		private RunReport runReport = new RunReport();

		// Token: 0x04001388 RID: 5000
		public GameObject gameEndReportPanelPrefab;

		// Token: 0x04001389 RID: 5001
		private bool reportScreensGenerated;

		// Token: 0x0400138A RID: 5002
		public float appearanceDelay = 1f;

		// Token: 0x0400138B RID: 5003
		private float appearanceTimer;

		// Token: 0x0400138C RID: 5004
		private static int kRpcRpcClientGameOver = 1518660169;
	}
}
