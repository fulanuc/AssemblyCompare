using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FC RID: 764
	[RequireComponent(typeof(VoteController))]
	public class GameOverController : NetworkBehaviour
	{
		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0000BDAE File Offset: 0x00009FAE
		// (set) Token: 0x06000F7C RID: 3964 RVA: 0x0000BDB5 File Offset: 0x00009FB5
		public static GameOverController instance { get; private set; }

		// Token: 0x06000F7D RID: 3965 RVA: 0x0000BDBD File Offset: 0x00009FBD
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

		// Token: 0x06000F7E RID: 3966 RVA: 0x0005D2F8 File Offset: 0x0005B4F8
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

		// Token: 0x06000F7F RID: 3967 RVA: 0x0000BDE2 File Offset: 0x00009FE2
		private void Start()
		{
			this.appearanceTimer = this.appearanceDelay;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0000BDF0 File Offset: 0x00009FF0
		private void OnEnable()
		{
			GameOverController.instance = SingletonHelper.Assign<GameOverController>(GameOverController.instance, this);
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0000BE02 File Offset: 0x0000A002
		private void OnDisable()
		{
			GameOverController.instance = SingletonHelper.Unassign<GameOverController>(GameOverController.instance, this);
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0000BE14 File Offset: 0x0000A014
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

		// Token: 0x06000F83 RID: 3971 RVA: 0x0000BE4A File Offset: 0x0000A04A
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

		// Token: 0x06000F84 RID: 3972 RVA: 0x0005D4AC File Offset: 0x0005B6AC
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

		// Token: 0x06000F85 RID: 3973 RVA: 0x0000BE6B File Offset: 0x0000A06B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (((initialState ? 1 : reader.ReadByte()) & 1) > 0)
			{
				this.runReport.Read(reader);
			}
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x0000BE8C File Offset: 0x0000A08C
		[ClientRpc]
		public void RpcClientGameOver()
		{
			if (Run.instance)
			{
				Run.instance.OnClientGameOver(this.runReport);
			}
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0000BEC8 File Offset: 0x0000A0C8
		protected static void InvokeRpcRpcClientGameOver(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcClientGameOver called on server.");
				return;
			}
			((GameOverController)obj).RpcClientGameOver();
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0005D4F0 File Offset: 0x0005B6F0
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

		// Token: 0x06000F8B RID: 3979 RVA: 0x0000BEEB File Offset: 0x0000A0EB
		static GameOverController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(GameOverController), GameOverController.kRpcRpcClientGameOver, new NetworkBehaviour.CmdDelegate(GameOverController.InvokeRpcRpcClientGameOver));
			NetworkCRC.RegisterBehaviour("GameOverController", 0);
		}

		// Token: 0x0400139B RID: 5019
		[Tooltip("How long it takes after the first person has hit the continue button for the game to forcibly end.")]
		public float timeoutDuration;

		// Token: 0x0400139C RID: 5020
		private const uint runReportDirtyBit = 1u;

		// Token: 0x0400139D RID: 5021
		private const uint allDirtyBits = 1u;

		// Token: 0x0400139E RID: 5022
		private RunReport runReport = new RunReport();

		// Token: 0x0400139F RID: 5023
		public GameObject gameEndReportPanelPrefab;

		// Token: 0x040013A0 RID: 5024
		private bool reportScreensGenerated;

		// Token: 0x040013A1 RID: 5025
		public float appearanceDelay = 1f;

		// Token: 0x040013A2 RID: 5026
		private float appearanceTimer;

		// Token: 0x040013A3 RID: 5027
		private static int kRpcRpcClientGameOver = 1518660169;
	}
}
