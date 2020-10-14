using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter : MonoBehaviour {
	public string ChapterName;
	public int NextChapter;

	private MenuSystem MenuSystem;

	public GameObject SentencesMenuPrefab;

	private GameObject SentencesMenu;

	public TMP_InputField ChapterNameInput;
	public TMP_Dropdown NextChapterList;

	private JsonFileIO fileIO;

	void Start() {
		MenuSystem = FindObjectOfType<MenuSystem>();
		fileIO = FindObjectOfType<JsonFileIO>();

		List<string> options = new List<string>();

		options.Add("None");
		NextChapterList.AddOptions(options);

		if (SentencesMenu != null) { return; }

		CreateSentencesMenu(MenuSystem.currentlyLodadedMenu);
	}

	public void CreateSentencesMenu(string ParentMenu) {
		MenuSystem = FindObjectOfType<MenuSystem>();

		SentencesMenu = (GameObject)Instantiate(SentencesMenuPrefab);

		SentencesMenu.GetComponent<ItemMenu>().MenuName = ChapterName + "Sentences";

		SentencesMenu.name = ChapterName + "Sentences";
		SentencesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentences(" + ChapterName + ")";
		SentencesMenu.GetComponent<ItemMenu>().ParentMenu = ParentMenu;

		SentencesMenu.SetActive(false);

		Menu menu = new Menu();
		menu.MenuName = ChapterName + "Sentences";
		menu.MenuObject = SentencesMenu.transform;

		MenuSystem.menus.Add(menu);
	}

	public void CreateSentenceSubmenus() {
		if (SentencesMenu.GetComponent<ItemMenu>().ItemList.childCount <= 1) { return; }
		foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			if (!sen.TryGetComponent(out Sentence sentence)) { continue; }
			if (sentence.ChoicesMenu != null) { continue; }
			sentence.CreateMenus();
		}
	}

	void Update() {
		List<string> options = fileIO.ReturnAllChapters();

		int tmp = NextChapterList.value;

		if (options.Count != NextChapterList.options.Count) {
			NextChapterList.ClearOptions();
			NextChapterList.AddOptions(options);
		}

		for (int i = 0; i < NextChapterList.options.Count; i++) {
			if (i >= options.Count) {
				NextChapterList.ClearOptions();
				NextChapterList.AddOptions(options);
				break;
			}
			if (options[i] != NextChapterList.options[i].text) {
				NextChapterList.ClearOptions();
				NextChapterList.AddOptions(options);
			}
		}

		NextChapterList.value = tmp;

		if (NextChapterList.value >= NextChapterList.options.Count) {
			NextChapterList.value = 0;
		}
	}

	public void ShowSentenceMenu() {
		foreach(Transform menu in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			if(menu.TryGetComponent(out Sentence sentence)) {
				sentence.UpdateLists();
			}
		}
		MenuSystem.LoadMenu(SentencesMenu.GetComponent<ItemMenu>().MenuName);
	}

	public void SetValue(TMP_InputField input) {
		Menu menu = new Menu();
		menu.MenuName = SentencesMenu.GetComponent<ItemMenu>().MenuName;
		SentencesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentences(" + ChapterName + ")";
		menu.MenuObject = SentencesMenu.transform;

		int index = 0;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus[index].MenuName = input.text + "Sentences";

		if (fileIO == null) { fileIO = FindObjectOfType<JsonFileIO>(); }
		ChapterName = input.text;

		UpdateMenuNames(fileIO.ChapterMenu.MenuName);
	}

	public void UpdateMenuNames(string ParentMenu) {
		SentencesMenu.name = ChapterName + "Sentences";
		SentencesMenu.GetComponent<ItemMenu>().MenuName = SentencesMenu.name;
		SentencesMenu.GetComponent<ItemMenu>().ParentMenu = ParentMenu;
		if (SentencesMenu.GetComponent<ItemMenu>().ItemList.childCount == 1) { return; }
		foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			sen.TryGetComponent(out Sentence sentence);
			if (sentence == null) { continue; }
			sentence.UpdateMenuNames();
		}
	}

	public void SetValues() {
		fileIO = FindObjectOfType<JsonFileIO>();
		List<string> options = fileIO.ReturnAllChapters();

		if (options.Count != NextChapterList.options.Count) {
			NextChapterList.ClearOptions();
			NextChapterList.AddOptions(options);
		}

		for (int i = 0; i < NextChapterList.options.Count; i++) {
			if (i >= options.Count) {
				NextChapterList.ClearOptions();
				NextChapterList.AddOptions(options);
				break;
			}
			if (options[i] != NextChapterList.options[i].text) {
				NextChapterList.ClearOptions();
				NextChapterList.AddOptions(options);
			}
		}

		NextChapterList.value = NextChapter + 1;

		if (NextChapterList.value >= NextChapterList.options.Count) {
			NextChapterList.value = 0;
		}
	}

	public void SetValue(TMP_Dropdown input) {
		NextChapter = input.value;
		--NextChapter;
	}

	public void RemoveItem() {
		Menu menu = new Menu();
		menu.MenuObject = SentencesMenu.transform;

		foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			if (!sen.TryGetComponent(out Sentence sentence)) { continue; }
			sentence.RemoveItem();
		}

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				MenuSystem.menus.Remove(item);
				break;
			}
		}

		Destroy(SentencesMenu);
		Destroy(this.gameObject);
	}

	public ItemMenu GetSentencesMenu() {
		return SentencesMenu.GetComponent<ItemMenu>();
	}
}
