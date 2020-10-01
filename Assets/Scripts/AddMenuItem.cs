using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMenuItem : MonoBehaviour
{
	public Transform ItemList;
	public GameObject prefab;

	public void AddItem() {
		GameObject Item = (GameObject)Instantiate(prefab, ItemList);
		Item.name = prefab.name;
	}

	private void Update() {
		this.transform.SetAsLastSibling();
	}
}
