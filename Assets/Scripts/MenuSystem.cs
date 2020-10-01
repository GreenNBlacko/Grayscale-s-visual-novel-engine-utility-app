using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuSystem : MonoBehaviour {

	public string StartMenu;
	public List<Menu> menus = new List<Menu>();

	[HideInInspector]
	public string currentlyLodadedMenu;

	private JsonFileIO FileIO;

	void Start() {
		FileIO = FindObjectOfType<JsonFileIO>();
		if(StartMenu == "") {
			LoadMenu(menus[0].MenuName);
		} else {
			LoadMenu(StartMenu);
		}
	}

	public void LoadMenu(string MenuName) {
		foreach (Menu i in menus) {
			if (i.MenuName == MenuName) {
				i.MenuObject.gameObject.SetActive(true);
			} else {
				i.MenuObject.gameObject.SetActive(false);
			}
		}
		currentlyLodadedMenu = MenuName;
	}

	public void LoadSentenceData(string MenuName) {
		if(FileIO.IsFileLoaded()) {
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
