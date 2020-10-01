using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {
	public GameObject ColorMenuPrefab;

	private List<GameObject> ColorMenus = new List<GameObject>();
	private List<ColorMenu> Colors = new List<ColorMenu>();

	public string CharacterName;
	public bool UseSeperateColors;
	public Color32 NameColor;
	public Color32 TextColor;
	public Color32 Color;
	public int GradientType;
	public bool UseSeperateGradientColors;
	public Color32 NameGradientColor;
	public Color32 TextGradientColor;
	public Color32 GradientColor;

	public TMP_InputField CharacterNameInput;
	public Toggle UseSeperateColorsToggle;
	public TMP_Dropdown GradientTypeDropdown;
	public Toggle UseSeperateGradientColorsToggle;

	public GameObject[] CharacterColorObjects = new GameObject[6];
	public Image[] CharacterColorDisplays = new Image[6];

	private GameObject CharacterColorList;

	void Start() {
		CharacterColorList = new GameObject(CharacterName + "Colors");

		ColorMenus.Add(Instantiate(ColorMenuPrefab, CharacterColorList.transform));
		ColorMenus.Add(Instantiate(ColorMenuPrefab, CharacterColorList.transform));
		ColorMenus.Add(Instantiate(ColorMenuPrefab, CharacterColorList.transform));
		ColorMenus.Add(Instantiate(ColorMenuPrefab, CharacterColorList.transform));
		ColorMenus.Add(Instantiate(ColorMenuPrefab, CharacterColorList.transform));
		ColorMenus.Add(Instantiate(ColorMenuPrefab, CharacterColorList.transform));

		foreach (GameObject colorMenu in ColorMenus) {
			Colors.Add(colorMenu.GetComponent<ColorMenu>());
			colorMenu.GetComponent<ColorMenu>().character = this;
		}

		Colors[0].Title.text = "Sentence data: Name Color(" + CharacterName + ")";
		Colors[1].Title.text = "Sentence data: Text Color(" + CharacterName + ")";
		Colors[2].Title.text = "Sentence data: Color(" + CharacterName + ")";
		Colors[3].Title.text = "Sentence data: Name Gradient Color(" + CharacterName + ")";
		Colors[4].Title.text = "Sentence data: Text Gradient Color(" + CharacterName + ")";
		Colors[5].Title.text = "Sentence data: Gradient Color(" + CharacterName + ")";

		SetValues();

		CloseColorMenu(0);
		CloseColorMenu(1);
		CloseColorMenu(2);
		CloseColorMenu(3);
		CloseColorMenu(4);
		CloseColorMenu(5);
	}

	public void SetValues() { 
		CharacterName = CharacterNameInput.text;
		UseSeperateColors = UseSeperateColorsToggle.isOn;
		GradientType = GradientTypeDropdown.value;
		UseSeperateGradientColors = UseSeperateGradientColorsToggle.isOn;

		CharacterColorList.name = CharacterName + "Colors";

		Colors[0].Title.text = "Sentence data: Name Color(" + CharacterName + ")";
		Colors[1].Title.text = "Sentence data: Text Color(" + CharacterName + ")";
		Colors[2].Title.text = "Sentence data: Color(" + CharacterName + ")";
		Colors[3].Title.text = "Sentence data: Name Gradient Color(" + CharacterName + ")";
		Colors[4].Title.text = "Sentence data: Text Gradient Color(" + CharacterName + ")";
		Colors[5].Title.text = "Sentence data: Gradient Color(" + CharacterName + ")";

		if (GradientType == 0) {
			UseSeperateGradientColorsToggle.transform.parent.gameObject.SetActive(false);
			CharacterColorObjects[3].SetActive(false);
			CharacterColorObjects[4].SetActive(false);
			CharacterColorObjects[5].SetActive(false);
		} else if (GradientType == 1) {
			UseSeperateGradientColorsToggle.transform.parent.gameObject.SetActive(false);
			CharacterColorObjects[3].SetActive(true);
			CharacterColorObjects[4].SetActive(false);
			CharacterColorObjects[5].SetActive(false);

		} else if (GradientType == 2) {
			UseSeperateGradientColorsToggle.transform.parent.gameObject.SetActive(false);
			CharacterColorObjects[3].SetActive(false);
			CharacterColorObjects[4].SetActive(true);
			CharacterColorObjects[5].SetActive(false);
		} else if (GradientType == 3) {
			UseSeperateGradientColorsToggle.transform.parent.gameObject.SetActive(true);
		}

		if (UseSeperateColors && UseSeperateColorsToggle.transform.parent.gameObject.activeInHierarchy) {
			CharacterColorObjects[0].SetActive(true);
			CharacterColorObjects[1].SetActive(true);
			CharacterColorObjects[2].SetActive(false);
		} else {
			CharacterColorObjects[0].SetActive(false);
			CharacterColorObjects[1].SetActive(false);
			CharacterColorObjects[2].SetActive(true);
		}

		if (UseSeperateGradientColors && GradientType == 3 && UseSeperateGradientColorsToggle.transform.parent.gameObject.activeInHierarchy) {
			CharacterColorObjects[3].SetActive(true);
			CharacterColorObjects[4].SetActive(true);
			CharacterColorObjects[5].SetActive(false);
		} else if (GradientType == 3 && !UseSeperateGradientColors && UseSeperateGradientColorsToggle.transform.parent.gameObject.activeInHierarchy) {
			CharacterColorObjects[3].SetActive(false);
			CharacterColorObjects[4].SetActive(false);
			CharacterColorObjects[5].SetActive(true);
		}
	}

	public void OpenColorMenu(int index) {
		ColorMenus[index].SetActive(true);
	}

	public void CloseColorMenu(int index) {
		NameColor = Colors[0].GetColor();
		TextColor = Colors[1].GetColor();
		Color = Colors[2].GetColor();
		NameGradientColor = Colors[3].GetColor();
		TextGradientColor = Colors[4].GetColor();
		GradientColor = Colors[5].GetColor();

		CharacterColorDisplays[0].color = NameColor;
		CharacterColorDisplays[1].color = TextColor;
		CharacterColorDisplays[2].color = Color;
		CharacterColorDisplays[3].color = NameGradientColor;
		CharacterColorDisplays[4].color = TextGradientColor;
		CharacterColorDisplays[5].color = GradientColor;

		ColorMenus[index].SetActive(false);
	}

	public void RemoveItem() {
		Destroy(CharacterColorList);
		Destroy(gameObject);
	}
}

