using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sentence : MonoBehaviour {
	public string CharacterName;
	public string Text;
	public int ArtworkType;
	public int ArtworkName;
	public bool Choice;
	public bool voiced;
	public string VAClipPath;

	public TMP_Dropdown NameDropDown;
	public TMP_InputField SentenceText;
	public TMP_Dropdown ArtworkTypeDropDown;
	public TMP_Dropdown ArtworkNameDropDown;
	public Toggle ChoiceToggle;
	public Button ChoiceButton;
	public Toggle VoicedToggle;
	public TMP_InputField VAClipInput;

	private MenuSystem MenuSystem;

	public GameObject ChoicesMenuPrefab;
	public GameObject SentenceActionsMenuPrefab;

	private GameObject ChoicesMenu;
	private GameObject SentenceActionsMenu;

	public string CharactersMenuName;


	// Start is called before the first frame update
	void Start() {
		SetValues();

		MenuSystem = FindObjectOfType<MenuSystem>();

		if(ChoicesMenu != null) { return; }

		CreateMenus();
	}

	private void Update() {
		List<string> options = GetCharacterList();

		int tmp = NameDropDown.value;

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

		options = GetArtworkList(ArtworkType - 1);

		tmp = ArtworkNameDropDown.value;

		if (options.Count != ArtworkNameDropDown.options.Count) {
			ArtworkNameDropDown.ClearOptions();
			ArtworkNameDropDown.AddOptions(options);
		}

		for (int i = 0; i < ArtworkNameDropDown.options.Count; i++) {
			if (i >= options.Count) {
				ArtworkNameDropDown.ClearOptions();
				ArtworkNameDropDown.AddOptions(options);
				break;
			}
			if (options[i] != ArtworkNameDropDown.options[i].text) {
				ArtworkNameDropDown.ClearOptions();
				ArtworkNameDropDown.AddOptions(options);
			}
		}

		ArtworkNameDropDown.value = tmp;

		if (ArtworkNameDropDown.value >= ArtworkNameDropDown.options.Count) {
			ArtworkNameDropDown.value = 0;
		}
	}

	public void CreateMenus() {
		MenuSystem = FindObjectOfType<MenuSystem>();

		ChoicesMenu = Instantiate(ChoicesMenuPrefab);

		ChoicesMenu.GetComponent<ItemMenu>().MenuName = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.name = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Choices(" + MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		ChoicesMenu.GetComponent<ItemMenu>().ParentMenu = MenuSystem.currentlyLodadedMenu;

		ChoicesMenu.SetActive(false);

		Menu menu = new Menu();
		menu.MenuName = ChoicesMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = ChoicesMenu.transform;

		MenuSystem.menus.Add(menu);


		SentenceActionsMenu = Instantiate(SentenceActionsMenuPrefab);

		SentenceActionsMenu.GetComponent<ItemMenu>().MenuName = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.name = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentence Actions(" + MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		SentenceActionsMenu.GetComponent<ItemMenu>().ParentMenu = MenuSystem.currentlyLodadedMenu;

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
		MenuSystem.LoadMenu(SentenceActionsMenu.GetComponent<ItemMenu>().MenuName);

	}

	public void SetValues() {
		CharacterName = NameDropDown.captionText.text;
		Text = SentenceText.text;
		ArtworkType = ArtworkTypeDropDown.value;
		ArtworkName = ArtworkNameDropDown.value;
		Choice = ChoiceToggle.isOn;
		voiced = VoicedToggle.isOn;
		VAClipPath = VAClipInput.text;

		if(ArtworkType == 0 && ArtworkNameDropDown.transform.parent.gameObject.activeInHierarchy) {
			ArtworkNameDropDown.transform.parent.gameObject.SetActive(false);
		} 
		if(ArtworkType != 0 && !ArtworkNameDropDown.transform.parent.gameObject.activeInHierarchy) {
			ArtworkNameDropDown.transform.parent.gameObject.SetActive(true);
		}

		if (Choice && !ChoiceButton.transform.parent.gameObject.activeInHierarchy) {
			ChoiceButton.transform.parent.gameObject.SetActive(true);
		} 
		if(!Choice && ChoiceButton.transform.parent.gameObject.activeInHierarchy) {
			ChoiceButton.transform.parent.gameObject.SetActive(false);
		}

		if (voiced && !VAClipInput.transform.parent.gameObject.activeInHierarchy) {
			VAClipInput.transform.parent.gameObject.SetActive(true);
		} 
		if(!voiced && VAClipInput.transform.parent.gameObject.activeInHierarchy) {
			VAClipInput.transform.parent.gameObject.SetActive(false);
		}
	}

	public void UpdateMenuNames() {
		Menu menu = new Menu();
		menu.MenuName = ChoicesMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = ChoicesMenu.transform;

		int index = 0;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus[index].MenuName = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";

		ChoicesMenu.GetComponent<ItemMenu>().MenuName = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.name = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "Choices";
		ChoicesMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Choices(" + MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		ChoicesMenu.GetComponent<ItemMenu>().ParentMenu = MenuSystem.currentlyLodadedMenu;

		menu = new Menu();
		menu.MenuName = SentenceActionsMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = SentenceActionsMenu.transform;

		index = 0;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus[index].MenuName = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";

		SentenceActionsMenu.GetComponent<ItemMenu>().MenuName = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.name = MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/") + transform.GetSiblingIndex() + "SentenceActions";
		SentenceActionsMenu.GetComponent<ItemMenu>().title.text = "Sentence data: Sentence Actions(" + MenuSystem.currentlyLodadedMenu.Replace("Sentences", "/Sentence Index - ") + transform.GetSiblingIndex() + ")";

		SentenceActionsMenu.GetComponent<ItemMenu>().ParentMenu = MenuSystem.currentlyLodadedMenu;
	}

	public List<string> GetCharacterList() {
		List<string> characters = new List<string>();

		JsonFileIO fileIO = FindObjectOfType<JsonFileIO>();

		foreach(Transform temp in fileIO.CharacterMenu.ItemList) {
			temp.TryGetComponent(out Character character);

			if(character == null) { continue; }
			characters.Add(character.CharacterName);
		}

		return characters;
	}

	public List<string> GetArtworkList(int Type) {
		List<string> artworks = new List<string>();

		JsonFileIO fileIO = FindObjectOfType<JsonFileIO>();

		foreach (Transform temp in fileIO.ArtworkMenu.ItemList) {
			temp.TryGetComponent(out Artwork artwork);

			if (artwork == null) { continue; }
			if(artwork.ArtworkType == Type)
				artworks.Add(artwork.ArtworkName);
			Debug.Log(artwork.ArtworkName);
		}

		return artworks;
	}

	public void RemoveItem() {
		transform.SetAsLastSibling();
		foreach(Transform sentence in transform.parent) {
			sentence.TryGetComponent(out Sentence sen);
			if(sen != this && sen != null) {
				sen.UpdateMenuNames();
			}
		}

		Menu menu = new Menu();
		menu.MenuName = ChoicesMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = ChoicesMenu.transform;

		int index = 0;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus.Remove(MenuSystem.menus[index]);

		Destroy(ChoicesMenu);

		menu = new Menu();

		menu.MenuName = SentenceActionsMenu.GetComponent<ItemMenu>().MenuName;
		menu.MenuObject = SentenceActionsMenu.transform;

		index = 0;

		foreach (Menu item in MenuSystem.menus) {
			if (item.MenuObject == menu.MenuObject) {
				index = MenuSystem.menus.IndexOf(item);
			}
		}

		MenuSystem.menus.Remove(MenuSystem.menus[index]);

		Destroy(SentenceActionsMenu);
		Destroy(this.gameObject);
	}
}
