using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chapter : MonoBehaviour
{
	public string ChapterName;
	public int NextChapter;

	private MenuSystem MenuSystem;

	public GameObject SentencesMenuPrefab;

	private GameObject SentencesMenu;

	public TMP_InputField ChapterNameInput;
	public TMP_Dropdown NextChapterList;

	void Start() {
		MenuSystem = FindObjectOfType<MenuSystem>();

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

		foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			sen.TryGetComponent(out Sentence sentence);
			if (sentence == null) { continue; }
			sentence.UpdateMenuNames();
			sentence.CreateMenus();
		}
	}

	void Update() {
		List<string> options = ReturnAllChapters();

		int tmp = NextChapterList.value;

		if (options.Count != NextChapterList.options.Count) {
			NextChapterList.ClearOptions();
			NextChapterList.AddOptions(options);
		}

		for(int i = 0; i < NextChapterList.options.Count; i++) {
			if(i >= options.Count) {
				NextChapterList.ClearOptions();
				NextChapterList.AddOptions(options);
				break;
			}
			if(options[i] != NextChapterList.options[i].text) {
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
		Debug.Log(SentencesMenu.GetComponent<ItemMenu>().MenuName);
		MenuSystem.LoadMenu(SentencesMenu.GetComponent<ItemMenu>().MenuName);
	}

	public void SetValue(TMP_InputField input) {
		Menu menu = new Menu();
		menu.MenuName = SentencesMenu.GetComponent<ItemMenu>().MenuName;
		SentencesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentences(" + ChapterName + ")";
		menu.MenuObject = SentencesMenu.transform;

		int index = 0;

		foreach(Menu item in MenuSystem.menus) {
			if(item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus[index].MenuName = input.text + "Sentences";

		SentencesMenu.name = input.text + "Sentences";
		SentencesMenu.GetComponent<ItemMenu>().MenuName = SentencesMenu.name;
		SentencesMenu.GetComponent<ItemMenu>().ParentMenu = MenuSystem.currentlyLodadedMenu;
		if (SentencesMenu.GetComponent<ItemMenu>().ItemList.childCount <= 1) { ChapterName = input.text; return; }
		foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			sen.TryGetComponent(out Sentence sentence);

			if(sentence == null) { continue; }
			sentence.UpdateMenuNames();
		}
	}

	public void UpdateMenuNames(string ParentMenu) {
		SentencesMenu.name = ChapterName + "Sentences";
		SentencesMenu.GetComponent<ItemMenu>().MenuName = SentencesMenu.name;
		SentencesMenu.GetComponent<ItemMenu>().ParentMenu = ParentMenu;
		if (SentencesMenu.GetComponent<ItemMenu>().ItemList.childCount == 1) { return; }
		foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
			sen.TryGetComponent(out Sentence sentence);
			if(sentence == null) { continue; }
			sentence.UpdateMenuNames();
			sentence.CreateMenus();
		}
	}

	public void SetValues() {
		List<string> options = ReturnAllChapters();

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
		menu.MenuName = SentencesMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = SentencesMenu.transform;

		int index = 0;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus.Remove(MenuSystem.menus[index]);

		if (SentencesMenu.GetComponent<ItemMenu>().ItemList.childCount > 1) { 
			foreach (Transform sen in SentencesMenu.GetComponent<ItemMenu>().ItemList) {
				sen.TryGetComponent(out Sentence sentence);
				if(sentence == null) { continue; }
				sentence.RemoveItem();
			}
		}

		Destroy(SentencesMenu);
		Destroy(this.gameObject);
	}

	public List<string> ReturnAllChapters() {
		List<string> chapterNames = new List<string>();

		chapterNames.Add("None");

		foreach (Transform child in transform.parent) {
			
			if(!child.TryGetComponent(out Chapter chapter)) { continue; }
			chapterNames.Add(chapter.ChapterName);
		}

		return chapterNames;
	}

	public ItemMenu GetSentencesMenu() {
		return SentencesMenu.GetComponent<ItemMenu>();
	}
}
