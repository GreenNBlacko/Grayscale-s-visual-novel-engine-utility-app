using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PreviewChapter : MonoBehaviour {
	public bool IsExpanded;

	public TMP_Text Title;
	public Transform ItemList;

	private void Update() {
		ItemList.gameObject.SetActive(IsExpanded);
	}

	public void ToggleMenu() {
		IsExpanded = !IsExpanded;
	}
}
