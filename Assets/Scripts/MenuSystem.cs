using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour {

	public string StartMenu;
	public List<Menu> menus = new List<Menu>();

	[HideInInspector]
	public string currentlyLodadedMenu;

	private JsonFileIO FileIO;

	void Start() {
		FileIO = FindObjectOfType<JsonFileIO>();
		if (StartMenu == "") {
			StartMenu = menus[0].MenuName;
		}

		LoadMenu(StartMenu);
	}

	void Update() {

	}

	public void LoadMenu(string MenuName) {
		foreach (Menu i in menus) {
			if (i.MenuObject == null) { continue; }
			if (i.MenuName == MenuName) {
				i.MenuObject.gameObject.SetActive(true);
			} else {
				i.MenuObject.gameObject.SetActive(false);
			}
		}
		currentlyLodadedMenu = MenuName;
	}

	public void LoadSentenceData(string MenuName) {
		if (FileIO.IsFileLoaded()) {
			LoadMenu(MenuName);
			return;
		}
		Debug.LogError("No file loaded!");
	}

	public void Exit() {
		Application.Quit();
	}
}

[Serializable]
public class Menu {
	public string MenuName;
	public Transform MenuObject;
}
