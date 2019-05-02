using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A7 RID: 679
	public class CombineMesh : MonoBehaviour
	{
		// Token: 0x06000DDF RID: 3551 RVA: 0x00056404 File Offset: 0x00054604
		private void Start()
		{
			Renderer component = base.GetComponent<Renderer>();
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			CombineInstance[] array = new CombineInstance[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				array[i].mesh = componentsInChildren[i].sharedMesh;
				array[i].transform = componentsInChildren[i].transform.localToWorldMatrix;
				componentsInChildren[i].gameObject.SetActive(false);
			}
			MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = new Mesh();
			meshFilter.mesh.CombineMeshes(array, true, true, true);
			component.material = componentsInChildren[0].GetComponent<Renderer>().sharedMaterial;
			base.gameObject.SetActive(true);
		}
	}
}
