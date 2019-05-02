using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x0200040E RID: 1038
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputStickVisualizer : MonoBehaviour
	{
		// Token: 0x06001747 RID: 5959 RVA: 0x000117E4 File Offset: 0x0000F9E4
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x000117F2 File Offset: 0x0000F9F2
		private Player GetPlayer()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem == null)
			{
				return null;
			}
			return eventSystem.player;
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0001180A File Offset: 0x0000FA0A
		private CameraRigController GetCameraRigController()
		{
			if (CameraRigController.readOnlyInstancesList.Count <= 0)
			{
				return null;
			}
			return CameraRigController.readOnlyInstancesList[0];
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x00079724 File Offset: 0x00077924
		private void SetBarValues(Vector2 vector, Scrollbar scrollbarX, Scrollbar scrollbarY)
		{
			if (scrollbarX)
			{
				scrollbarX.value = Util.Remap(vector.x, -1f, 1f, 0f, 1f);
			}
			if (scrollbarY)
			{
				scrollbarY.value = Util.Remap(vector.y, -1f, 1f, 0f, 1f);
			}
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0007978C File Offset: 0x0007798C
		private void Update()
		{
			Player player = this.GetPlayer();
			CameraRigController cameraRigController = this.GetCameraRigController();
			if (!cameraRigController || player == null)
			{
				return;
			}
			Vector2 vector = new Vector2(player.GetAxis(0), player.GetAxis(1));
			Vector2 vector2 = new Vector2(player.GetAxis(16), player.GetAxis(17));
			this.SetBarValues(vector, this.moveXBar, this.moveYBar);
			this.SetBarValues(vector2, this.aimXBar, this.aimYBar);
			this.SetBarValues(cameraRigController.aimStickPostDualZone, this.aimStickPostDualZoneXBar, this.aimStickPostDualZoneYBar);
			this.SetBarValues(cameraRigController.aimStickPostExponent, this.aimStickPostExponentXBar, this.aimStickPostExponentYBar);
			this.SetBarValues(cameraRigController.aimStickPostSmoothing, this.aimStickPostSmoothingXBar, this.aimStickPostSmoothingYBar);
			this.moveXLabel.text = string.Format("move.x={0:0.0000}", vector.x);
			this.moveYLabel.text = string.Format("move.y={0:0.0000}", vector.y);
			this.aimXLabel.text = string.Format("aim.x={0:0.0000}", vector2.x);
			this.aimYLabel.text = string.Format("aim.y={0:0.0000}", vector2.y);
		}

		// Token: 0x04001A4D RID: 6733
		[Header("Move")]
		public Scrollbar moveXBar;

		// Token: 0x04001A4E RID: 6734
		public Scrollbar moveYBar;

		// Token: 0x04001A4F RID: 6735
		public TextMeshProUGUI moveXLabel;

		// Token: 0x04001A50 RID: 6736
		public TextMeshProUGUI moveYLabel;

		// Token: 0x04001A51 RID: 6737
		[Header("Aim")]
		public Scrollbar aimXBar;

		// Token: 0x04001A52 RID: 6738
		public Scrollbar aimYBar;

		// Token: 0x04001A53 RID: 6739
		public TextMeshProUGUI aimXLabel;

		// Token: 0x04001A54 RID: 6740
		public TextMeshProUGUI aimYLabel;

		// Token: 0x04001A55 RID: 6741
		public Scrollbar aimStickPostSmoothingXBar;

		// Token: 0x04001A56 RID: 6742
		public Scrollbar aimStickPostSmoothingYBar;

		// Token: 0x04001A57 RID: 6743
		public Scrollbar aimStickPostDualZoneXBar;

		// Token: 0x04001A58 RID: 6744
		public Scrollbar aimStickPostDualZoneYBar;

		// Token: 0x04001A59 RID: 6745
		public Scrollbar aimStickPostExponentXBar;

		// Token: 0x04001A5A RID: 6746
		public Scrollbar aimStickPostExponentYBar;

		// Token: 0x04001A5B RID: 6747
		private MPEventSystemLocator eventSystemLocator;
	}
}
