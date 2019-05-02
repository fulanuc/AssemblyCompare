using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BC RID: 700
	public class Corpse : MonoBehaviour
	{
		// Token: 0x06000E3E RID: 3646 RVA: 0x0000B034 File Offset: 0x00009234
		private void CollectRenderers()
		{
			if (this.renderers == null)
			{
				this.renderers = base.GetComponentsInChildren<Renderer>();
			}
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x0000B04A File Offset: 0x0000924A
		private void OnEnable()
		{
			Corpse.instancesList.Add(this);
			if (Corpse.disposalMode == Corpse.DisposalMode.OutOfSight)
			{
				this.CollectRenderers();
			}
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x0000B065 File Offset: 0x00009265
		private void OnDisable()
		{
			Corpse.instancesList.Remove(this);
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x0000B073 File Offset: 0x00009273
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void StaticInit()
		{
			RoR2Application.onUpdate += Corpse.StaticUpdate;
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x0000B086 File Offset: 0x00009286
		private static void IncrementCurrentCheckIndex()
		{
			Corpse.currentCheckIndex++;
			if (Corpse.currentCheckIndex >= Corpse.instancesList.Count)
			{
				Corpse.currentCheckIndex = 0;
			}
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00057D84 File Offset: 0x00055F84
		private static bool CheckCorpseOutOfSight(Corpse corpse)
		{
			foreach (Renderer renderer in corpse.renderers)
			{
				if (renderer && renderer.isVisible)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x00057DC0 File Offset: 0x00055FC0
		private static void StaticUpdate()
		{
			if (Corpse.maxCorpses < 0)
			{
				return;
			}
			int num = Corpse.instancesList.Count - Corpse.maxCorpses;
			int num2 = Math.Min(Math.Min(num, Corpse.maxChecksPerUpdate), Corpse.instancesList.Count);
			Corpse.DisposalMode disposalMode = Corpse.disposalMode;
			if (disposalMode == Corpse.DisposalMode.Hard)
			{
				for (int i = num - 1; i >= 0; i--)
				{
					Corpse.DestroyCorpse(Corpse.instancesList[i]);
				}
				return;
			}
			if (disposalMode != Corpse.DisposalMode.OutOfSight)
			{
				return;
			}
			for (int j = 0; j < num2; j++)
			{
				Corpse.IncrementCurrentCheckIndex();
				if (Corpse.CheckCorpseOutOfSight(Corpse.instancesList[Corpse.currentCheckIndex]))
				{
					Corpse.DestroyCorpse(Corpse.instancesList[Corpse.currentCheckIndex]);
				}
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x0000B0AB File Offset: 0x000092AB
		private static void DestroyCorpse(Corpse corpse)
		{
			if (corpse)
			{
				UnityEngine.Object.Destroy(corpse.gameObject);
			}
		}

		// Token: 0x04001217 RID: 4631
		private static readonly List<Corpse> instancesList = new List<Corpse>();

		// Token: 0x04001218 RID: 4632
		private Renderer[] renderers;

		// Token: 0x04001219 RID: 4633
		private static int maxCorpses = 25;

		// Token: 0x0400121A RID: 4634
		private static Corpse.DisposalMode disposalMode = Corpse.DisposalMode.OutOfSight;

		// Token: 0x0400121B RID: 4635
		private static int maxChecksPerUpdate = 3;

		// Token: 0x0400121C RID: 4636
		private static int currentCheckIndex = 0;

		// Token: 0x020002BD RID: 701
		private class CorpsesMaxConVar : BaseConVar
		{
			// Token: 0x06000E48 RID: 3656 RVA: 0x000090A8 File Offset: 0x000072A8
			private CorpsesMaxConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000E49 RID: 3657 RVA: 0x00057E70 File Offset: 0x00056070
			public override void SetString(string newValue)
			{
				int maxCorpses;
				if (TextSerialization.TryParseInvariant(newValue, out maxCorpses))
				{
					Corpse.maxCorpses = maxCorpses;
				}
			}

			// Token: 0x06000E4A RID: 3658 RVA: 0x0000B0E5 File Offset: 0x000092E5
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Corpse.maxCorpses);
			}

			// Token: 0x0400121D RID: 4637
			private static Corpse.CorpsesMaxConVar instance = new Corpse.CorpsesMaxConVar("corpses_max", ConVarFlags.Archive | ConVarFlags.Engine, "25", "The maximum number of corpses allowed.");
		}

		// Token: 0x020002BE RID: 702
		public enum DisposalMode
		{
			// Token: 0x0400121F RID: 4639
			Hard,
			// Token: 0x04001220 RID: 4640
			OutOfSight
		}

		// Token: 0x020002BF RID: 703
		private class CorpseDisposalConVar : BaseConVar
		{
			// Token: 0x06000E4C RID: 3660 RVA: 0x000090A8 File Offset: 0x000072A8
			private CorpseDisposalConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000E4D RID: 3661 RVA: 0x00057E90 File Offset: 0x00056090
			public override void SetString(string newValue)
			{
				try
				{
					Corpse.DisposalMode disposalMode = (Corpse.DisposalMode)Enum.Parse(typeof(Corpse.DisposalMode), newValue, true);
					if (disposalMode != Corpse.disposalMode)
					{
						Corpse.disposalMode = disposalMode;
						if (disposalMode != Corpse.DisposalMode.Hard && disposalMode == Corpse.DisposalMode.OutOfSight)
						{
							foreach (Corpse corpse in Corpse.instancesList)
							{
								corpse.CollectRenderers();
							}
						}
					}
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06000E4E RID: 3662 RVA: 0x0000B10E File Offset: 0x0000930E
			public override string GetString()
			{
				return Corpse.disposalMode.ToString();
			}

			// Token: 0x04001221 RID: 4641
			private static Corpse.CorpseDisposalConVar instance = new Corpse.CorpseDisposalConVar("corpses_disposal", ConVarFlags.Archive | ConVarFlags.Engine, null, "The corpse disposal mode. Choices are Hard and OutOfSight.");
		}
	}
}
