using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresetButton : MonoBehaviour {
	public string HexValue;

	private Button button;
	private TMP_Text buttonText;
	private Button removeButton;

	private Color color;

	public ColorMenu colorMenu;

	void Start() {
		button = transform.GetComponent<Button>();
		buttonText = transform.GetComponentInChildren<TMP_Text>();
		removeButton = transform.GetChild(1).GetComponent<Button>();

		color = ColorMenu.HexToColor(HexValue);

		Color32 temp2 = new Color32 { r = (byte)color.r, g = (byte)color.g, b = (byte)color.b, a = 255 };

		button.GetComponent<Image>().color = temp2;

		buttonText.text = HexValue;

		gameObject.name = HexValue;

		Color32 temp = new Color { r = Color.white.r - color.r, g = Color.white.g - color.g, b = Color.white.b - color.b, a = Color.white.a };
		temp2 = new Color32 { r = (byte)temp.r, g = (byte)temp.g, b = (byte)temp.b, a = 255 };

		buttonText.color = temp2;

		removeButton.GetComponent<Image>().color = temp2;
	}

	public void LoadPreset () {
		colorMenu.LoadPreset(HexValue);
	}

	public void RemovePreset() {
		colorMenu.RemovePreset(HexValue);
	}
}
