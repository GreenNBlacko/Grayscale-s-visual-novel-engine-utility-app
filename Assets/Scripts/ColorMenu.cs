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

	public Character character;


	// Start is called before the first frame update
	void Start() {
		RValue.SetValue(255);
		GValue.SetValue(255);
		BValue.SetValue(255);

		HValue.SetValue(1);
		SValue.SetValue(0);
		VValue.SetValue(1);

		HexValue.SetValue("FFFFFF");

		UpdateColorRGB();
	}

	void Update() {

	}

	public void UpdateColorRGB() {
		float H, S, V;
		Color32 tmp = new Color32(RValue.GetValue(), GValue.GetValue(), BValue.GetValue(), 255);
		if (tmp.r != OutColor.r || tmp.g != OutColor.g || tmp.b != OutColor.b) {
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
		float H, S, V;
		Color color = new Color();

		color.r = System.Convert.ToInt32(HexValue.GetValue().Substring(0, 2), 16);
		color.g = System.Convert.ToInt32(HexValue.GetValue().Substring(2, 2), 16);
		color.b = System.Convert.ToInt32(HexValue.GetValue().Substring(4, 2), 16);
		color.a = 255;

		RValue.SetValue(color.r);
		GValue.SetValue(color.g);
		BValue.SetValue(color.b);

		Color.RGBToHSV(color, out H, out S, out V);
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

		UpdateColorRGB();
	}

	public void CloseMenu() {
		character.CloseColorMenu(transform.GetSiblingIndex());
	}
}
