using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorMenu : MonoBehaviour {
	public Color32 OutColor = new Color32(255, 255, 255, 255);

	public string ColorHex;

	[Space(15)]
	public TMP_Text Title;

	public Image PreviewImage;

	[Space(15)]
	public ByteSliderUI RValue;
	public ByteSliderUI GValue;
	public ByteSliderUI BValue;

	[Space(15)]
	public ByteSliderUI HValue;
	public ByteSliderUI SValue;
	public ByteSliderUI VValue;

	[Space(15)]
	public HexInput HexValue;

	[Space(15)]
	public Character character;

	[Space(15)]
	public GameObject ColorPresetMenu;
	public GameObject ColorPresetList;
	public GameObject ColorPresetButtonPrefab;

	private ColorPresetsClass ColorPresets = new ColorPresetsClass();

	private string path;

	private void OnEnable() {
		if (File.Exists(path)) {
			StreamReader reader = new StreamReader(path);

			string jsonData = reader.ReadToEnd();
			ColorPresets = JsonUtility.FromJson<ColorPresetsClass>(jsonData);

			reader.Close();
		}
	}

	void Start() {
		path = Application.persistentDataPath + "/ColorPresets/presets.json";

		if (File.Exists(path)) {
			StreamReader reader = new StreamReader(path);

			string jsonData = reader.ReadToEnd();
			ColorPresets = JsonUtility.FromJson<ColorPresetsClass>(jsonData);

			reader.Close();
		} else {
			FileInfo file1 = new System.IO.FileInfo(Application.persistentDataPath + "/ColorPresets/");
			file1.Directory.Create();

			string jsonData = JsonUtility.ToJson(ColorPresets.presets, true);
			File.WriteAllText(path, jsonData);
		}

		RValue.SetValue(OutColor.r);
		GValue.SetValue(OutColor.g);
		BValue.SetValue(OutColor.b);

		Color.RGBToHSV(OutColor, out float H, out float S, out float V);

		HValue.SetValue(H);
		SValue.SetValue(S);
		VValue.SetValue(V);

		HexValue.SetValue(ColorUtility.ToHtmlStringRGB(OutColor));

		UpdateColorRGB();
	}

	public void UpdateColorRGB(bool overrideMode = false) {
		float H, S, V;
		Color32 tmp = new Color32(RValue.GetValue(), GValue.GetValue(), BValue.GetValue(), 255);
		if (tmp.r != OutColor.r || tmp.g != OutColor.g || tmp.b != OutColor.b || overrideMode) {
			Color.RGBToHSV(tmp, out H, out S, out V);
			HValue.SetValue(H);
			SValue.SetValue(S);
			VValue.SetValue(V);

			OutColor = tmp;

			ColorHex = ColorUtility.ToHtmlStringRGB(tmp);
			HexValue.SetValue(ColorHex.ToString());

			PreviewImage.color = OutColor;
		}
	}

	public void UpdateColorHSV() {
		float H, S, V;
		H = HValue.GetValueFloat();
		S = SValue.GetValueFloat();
		V = VValue.GetValueFloat();
		Color32 tmp = Color.HSVToRGB(H, S, V);

		if (tmp.r != OutColor.r || tmp.g != OutColor.g || tmp.b != OutColor.b) {
			RValue.SetValue(tmp.r);
			GValue.SetValue(tmp.g);
			BValue.SetValue(tmp.b);

			HexValue.SetValue(ColorUtility.ToHtmlStringRGB(tmp));

			OutColor = tmp;

			PreviewImage.color = OutColor;
		}
	}

	public void UpdateColorHex() {

		Color color = HexToColor(HexValue.GetValue());

		RValue.SetValue(color.r);
		GValue.SetValue(color.g);
		BValue.SetValue(color.b);

		Color.RGBToHSV(color, out float H, out float S, out float V);
		HValue.SetValue(H);
		SValue.SetValue(S);
		VValue.SetValue(V);

		OutColor.r = (byte)color.r;
		OutColor.g = (byte)color.g;
		OutColor.b = (byte)color.b;

		PreviewImage.color = OutColor;
	}

	public Color32 GetColor() {
		return OutColor;
	}

	public void SetColor(byte[] colorRGB) {
		RValue.SetValue(colorRGB[0]);
		GValue.SetValue(colorRGB[1]);
		BValue.SetValue(colorRGB[2]);

		UpdateColorRGB(true);
	}

	public void SetColor(Color32 colorRGB) {
		RValue.SetValue(colorRGB.r);
		GValue.SetValue(colorRGB.g);
		BValue.SetValue(colorRGB.b);
	}

	public static Color HexToColor(string hexValue) {
		return new Color { r = System.Convert.ToInt32(hexValue.Substring(0, 2), 16), g = System.Convert.ToInt32(hexValue.Substring(2, 2), 16), b = System.Convert.ToInt32(hexValue.Substring(4, 2), 16), a = 255 };
	}

	public void SavePreset() {
		List<string> tempArray = new List<string>();

		tempArray.AddRange(ColorPresets.presets);
		tempArray.Add(HexValue.GetValue());

		ColorPresets.presets = tempArray.ToArray();

		string jsonData = JsonUtility.ToJson(ColorPresets, true);
		File.WriteAllText(path, jsonData);
	}

	public void OpenPresetsMenu() {
		foreach (string preset in ColorPresets.presets) {
			GameObject colorPreset = Instantiate(ColorPresetButtonPrefab, ColorPresetList.transform);

			colorPreset.GetComponent<ColorPresetButton>().HexValue = preset;
			colorPreset.GetComponent<ColorPresetButton>().colorMenu = this;
		}

		ColorPresetMenu.SetActive(true);
	}

	public void ClosePresetsMenu() {
		foreach(Transform menu in ColorPresetList.transform) {
			Destroy(menu.gameObject);
		}

		ColorPresetMenu.SetActive(false);
	}

	public void LoadPreset(string preset) {
		HexValue.SetValue(preset);

		UpdateColorHex();

		ClosePresetsMenu();
	}

	public void RemovePreset(string preset) {
		foreach(Transform Menu in ColorPresetList.transform) {
			if(Menu.name == preset) { Destroy(Menu.gameObject); break; }
		}

		List<string> tempArray = new List<string>();

		tempArray.AddRange(ColorPresets.presets);
		tempArray.Remove(preset);

		ColorPresets.presets = tempArray.ToArray();
	}

	public void CloseMenu() {
		character.CloseColorMenu(transform.GetSiblingIndex());
	}
}

[Serializable]
public class ColorPresetsClass {
	public string[] presets = new string[0];
}
