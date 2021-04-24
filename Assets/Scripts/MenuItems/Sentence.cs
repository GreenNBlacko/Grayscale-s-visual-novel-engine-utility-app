using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sentence : MonoBehaviour {
	public string CharacterName;
	public bool OverrideName;
	public string DisplayName;
	public string Text;
	public bool Choice;
	public bool voiced;
	public string VAClipPath;

	public TMP_Dropdown NameDropDown;
	public Toggle NameOverrideToggle;
	public TMP_InputField DisplayNameText;
	public TMP_InputField SentenceText;
	public Toggle ChoiceToggle;
	public Button ChoiceButton;
	public Toggle VoicedToggle;
	public TMP_InputField VAClipInput;

	private MenuSystem MenuSystem;

	public GameObject ChoicesMenuPrefab;
	public GameObject SentenceActionsMenuPrefab;

	public GameObject ChoicesMenu;
	public GameObject SentenceActionsMenu;

	public string CharactersMenuName;

	private JsonFileIO fileIO;


	// Start is called before the first frame update
	void Start() {
		SetValues();

		MenuSystem = FindObjectOfType<MenuSystem>();
		fileIO = FindObjectOfType<JsonFileIO>();

		if (ChoicesMenu != null || SentenceActionsMenu != null) { return; }

		CreateMenus();
	}

	public void CreateMenus() {
		MenuSystem = FindObjectOfType<MenuSystem>();

		ChoicesMenu = Instantiate(ChoicesMenuPrefab);

		ChoicesMenu.GetComponent<ItemMenu>().MenuName = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.name = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Choices(" + transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		ChoicesMenu.GetComponent<ItemMenu>().ParentMenu = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName;

		ChoicesMenu.SetActive(false);

		Menu menu = new Menu();
		menu.MenuName = ChoicesMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = ChoicesMenu.transform;

		MenuSystem.menus.Add(menu);


		SentenceActionsMenu = Instantiate(SentenceActionsMenuPrefab);

		SentenceActionsMenu.GetComponent<ItemMenu>().MenuName = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.name = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentence Actions(" + transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		SentenceActionsMenu.GetComponent<ItemMenu>().ParentMenu = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName;

		SentenceActionsMenu.SetActive(false);

		menu = new Menu();
		menu.MenuName = SentenceActionsMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = SentenceActionsMenu.transform;

		MenuSystem.menus.Add(menu);
	}

	public void ShowChoiceMenu() {
		MenuSystem.LoadMenu(ChoicesMenu.GetComponent<ItemMenu>().MenuName);

	}

	public void ShowSentenceActionMenu() {
		foreach(Transform menu in SentenceActionsMenu.GetComponent<ItemMenu>().ItemList) {
			if(menu.TryGetComponent(out SentenceAction sentenceAction)) {
				sentenceAction.SetDropdownValues();
				sentenceAction.ManageVariables(sentenceAction.ActionType, true);
				sentenceAction.SetValues();
			}
		}
		MenuSystem.LoadMenu(SentenceActionsMenu.GetComponent<ItemMenu>().MenuName);

	}

	public void UpdateLists() {
		fileIO = FindObjectOfType<JsonFileIO>();
		List<string> options = fileIO.GetCharacterList();

		int tmp = fileIO.ReturnCharacterIndex(CharacterName);

		if (options.Count != NameDropDown.options.Count) {
			NameDropDown.ClearOptions();
			NameDropDown.AddOptions(options);
		}

		for (int i = 0; i < NameDropDown.options.Count; i++) {
			if (i >= options.Count) {
				NameDropDown.ClearOptions();
				NameDropDown.AddOptions(options);
				break;
			}
			if (options[i] != NameDropDown.options[i].text) {
				NameDropDown.ClearOptions();
				NameDropDown.AddOptions(options);
			}
		}

		NameDropDown.value = tmp;

		if (NameDropDown.value >= NameDropDown.options.Count) {
			NameDropDown.value = 0;
		}
	}

	public void SetValues() {
		CharacterName = NameDropDown.captionText.text;
		OverrideName = NameOverrideToggle.isOn;
		DisplayName = DisplayNameText.text;
		Text = SentenceText.text;
		Choice = ChoiceToggle.isOn;
		voiced = VoicedToggle.isOn;
		VAClipPath = VAClipInput.text;

		if(OverrideName && !DisplayNameText.transform.parent.gameObject.activeInHierarchy) {
			DisplayNameText.transform.parent.gameObject.SetActive(true);
		}
		if (!OverrideName && DisplayNameText.transform.parent.gameObject.activeInHierarchy) {
			DisplayNameText.transform.parent.gameObject.SetActive(false);
		}

		if (Choice && !ChoiceButton.transform.parent.gameObject.activeInHierarchy) {
			ChoiceButton.transform.parent.gameObject.SetActive(true);
		}
		if (!Choice && ChoiceButton.transform.parent.gameObject.activeInHierarchy) {
			ChoiceButton.transform.parent.gameObject.SetActive(false);
		}

		if (voiced && !VAClipInput.transform.parent.gameObject.activeInHierarchy) {
			VAClipInput.transform.parent.gameObject.SetActive(true);
		}
		if (!voiced && VAClipInput.transform.parent.gameObject.activeInHierarchy) {
			VAClipInput.transform.parent.gameObject.SetActive(false);
		}
	}

	public void UpdateMenuNames() {
		Menu menu = new Menu();
		menu.MenuObject = ChoicesMenu.transform;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject.gameObject == ChoicesMenu && item.MenuName == ChoicesMenu.name) {
				item.MenuName = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
				break;
			}
		}

		ChoicesMenu.GetComponent<ItemMenu>().MenuName = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.name = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Choices(" + transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		ChoicesMenu.GetComponent<ItemMenu>().ParentMenu = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName;

		menu = new Menu();
		menu.MenuObject = SentenceActionsMenu.transform;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				item.MenuName = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
				break;
			}
		}

		SentenceActionsMenu.GetComponent<ItemMenu>().MenuName = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.name = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentence Actions(" + transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		SentenceActionsMenu.GetComponent<ItemMenu>().ParentMenu = transform.parent.parent.parent.parent.parent.GetComponent<ItemMenu>().MenuName;
	}

	public void RemoveItem() {
		transform.SetAsLastSibling();

		foreach (Transform sentence in transform.parent) {
			sentence.TryGetComponent(out Sentence sen);
			if (sen != null && sen != this) {
				sen.UpdateMenuNames();
				continue;
			}
			sentence.SetAsLastSibling();
		}

		UpdateMenuNames();

		foreach (Menu i in MenuSystem.menus) {
			if (i.MenuObject.gameObject == ChoicesMenu) {
				int index = MenuSystem.menus.IndexOf(i);
				this.MenuSystem.menus.Remove(i);
				break;
			}
		}

		Destroy(ChoicesMenu);

		foreach (Menu i in MenuSystem.menus) {
			if (i.MenuObject == SentenceActionsMenu.transform) {
				int index = MenuSystem.menus.IndexOf(i);
				this.MenuSystem.menus.Remove(i);
				break;
			}
		}

		try {
			Destroy(SentenceActionsMenu);
		}
		catch {
			Debug.LogError("Can't delete");
		}


		Destroy(gameObject);
	}
}
