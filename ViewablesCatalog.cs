using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004E5 RID: 1253
	public static class ViewablesCatalog
	{
		// Token: 0x06001C7E RID: 7294 RVA: 0x0008CCE8 File Offset: 0x0008AEE8
		public static void AddNodeToRoot(ViewablesCatalog.Node node)
		{
			node.SetParent(ViewablesCatalog.rootNode);
			foreach (ViewablesCatalog.Node node2 in node.Descendants())
			{
				if (ViewablesCatalog.fullNameToNodeMap.ContainsKey(node2.fullName))
				{
					Debug.LogFormat("Tried to add duplicate node {0}", new object[]
					{
						node2.fullName
					});
				}
				else
				{
					ViewablesCatalog.fullNameToNodeMap.Add(node2.fullName, node2);
				}
			}
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x0008CD78 File Offset: 0x0008AF78
		public static ViewablesCatalog.Node FindNode(string fullName)
		{
			ViewablesCatalog.Node result;
			if (ViewablesCatalog.fullNameToNodeMap.TryGetValue(fullName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x0008CD98 File Offset: 0x0008AF98
		[ConCommand(commandName = "viewables_list", flags = ConVarFlags.None, helpText = "Displays the full names of all viewables.")]
		private static void CCViewablesList(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from node in ViewablesCatalog.rootNode.Descendants()
			select node.fullName).ToArray<string>()));
		}

		// Token: 0x04001E79 RID: 7801
		private static readonly ViewablesCatalog.Node rootNode = new ViewablesCatalog.Node("", true, null);

		// Token: 0x04001E7A RID: 7802
		private static readonly Dictionary<string, ViewablesCatalog.Node> fullNameToNodeMap = new Dictionary<string, ViewablesCatalog.Node>();

		// Token: 0x020004E6 RID: 1254
		public class Node
		{
			// Token: 0x1700028B RID: 651
			// (get) Token: 0x06001C82 RID: 7298 RVA: 0x00014F15 File Offset: 0x00013115
			// (set) Token: 0x06001C83 RID: 7299 RVA: 0x00014F1D File Offset: 0x0001311D
			public ViewablesCatalog.Node parent { get; private set; }

			// Token: 0x1700028C RID: 652
			// (get) Token: 0x06001C84 RID: 7300 RVA: 0x00014F26 File Offset: 0x00013126
			public string fullName
			{
				get
				{
					if (this.fullNameDirty)
					{
						this.GenerateFullName();
					}
					return this._fullName;
				}
			}

			// Token: 0x06001C85 RID: 7301 RVA: 0x0008CDE8 File Offset: 0x0008AFE8
			public Node(string name, bool isFolder, ViewablesCatalog.Node parent = null)
			{
				this.name = name;
				this.isFolder = isFolder;
				this.shouldShowUnviewed = new Func<UserProfile, bool>(this.DefaultShouldShowUnviewedTest);
				this.children = this._children.AsReadOnly();
				this.SetParent(parent);
			}

			// Token: 0x06001C86 RID: 7302 RVA: 0x0008CE48 File Offset: 0x0008B048
			public void SetParent(ViewablesCatalog.Node newParent)
			{
				if (this.parent == newParent)
				{
					return;
				}
				ViewablesCatalog.Node parent = this.parent;
				if (parent != null)
				{
					parent._children.Remove(this);
				}
				this.parent = newParent;
				ViewablesCatalog.Node parent2 = this.parent;
				if (parent2 != null)
				{
					parent2._children.Add(this);
				}
				this.fullNameDirty = true;
			}

			// Token: 0x06001C87 RID: 7303 RVA: 0x0008CE9C File Offset: 0x0008B09C
			private void GenerateFullName()
			{
				string text = this.name;
				if (this.parent != null)
				{
					text = this.parent.fullName + text;
				}
				if (this.isFolder)
				{
					text += "/";
				}
				this._fullName = text;
				this.fullNameDirty = false;
			}

			// Token: 0x06001C88 RID: 7304 RVA: 0x0008CEEC File Offset: 0x0008B0EC
			public bool DefaultShouldShowUnviewedTest(UserProfile userProfile)
			{
				if (!this.isFolder && userProfile.HasViewedViewable(this.fullName))
				{
					return false;
				}
				using (IEnumerator<ViewablesCatalog.Node> enumerator = this.children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.shouldShowUnviewed(userProfile))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x06001C89 RID: 7305 RVA: 0x00014F3C File Offset: 0x0001313C
			public IEnumerable<ViewablesCatalog.Node> Descendants()
			{
				yield return this;
				foreach (ViewablesCatalog.Node node in this._children)
				{
					foreach (ViewablesCatalog.Node node2 in node.Descendants())
					{
						yield return node2;
					}
					IEnumerator<ViewablesCatalog.Node> enumerator2 = null;
				}
				List<ViewablesCatalog.Node>.Enumerator enumerator = default(List<ViewablesCatalog.Node>.Enumerator);
				yield break;
				yield break;
			}

			// Token: 0x04001E7B RID: 7803
			public readonly string name;

			// Token: 0x04001E7C RID: 7804
			public readonly bool isFolder;

			// Token: 0x04001E7E RID: 7806
			private readonly List<ViewablesCatalog.Node> _children = new List<ViewablesCatalog.Node>();

			// Token: 0x04001E7F RID: 7807
			public ReadOnlyCollection<ViewablesCatalog.Node> children;

			// Token: 0x04001E80 RID: 7808
			private string _fullName;

			// Token: 0x04001E81 RID: 7809
			private bool fullNameDirty = true;

			// Token: 0x04001E82 RID: 7810
			public Func<UserProfile, bool> shouldShowUnviewed;
		}
	}
}
