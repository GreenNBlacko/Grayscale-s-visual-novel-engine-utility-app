using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ByteSliderUI : MonoBehaviour {
	public ColorMenu colorMenu;

	public int ColorType;

	public float value;

	public Slider ItemSlider;
	public TMP_InputField ItemInput;

	public void SetValue(Slider input) {
		value = input.value;
		value = (float)Math.Round((double)value, 2);
		ItemInput.text = value.ToString();
		if (ColorType == 0) {
			colorMenu.UpdateColorRGB();
		} else {
			colorMenu.UpdateColorHSV();
		}
	}

	public void SetValue(TMP_InputField input) {
		if (input.text == "") { input.text = "0"; }
		value = float.Parse(input.text);
		value = (float)Math.Round((double)value, 2);
		ItemSlider.value = value;
		ItemInput.text = value.ToString();
		if (ColorType == 0) {
			colorMenu.UpdateColorRGB();
		} else {
			colorMenu.UpdateColorHSV();
		}
	}

	public byte GetValue() {
		return (byte)value;
	}

	public float GetValueFloat() {
		return value;
	}

	public void SetValue(byte value) {
		Slider.SliderEvent bak1 = ItemSlider.onValueChanged;
		TMP_InputField.OnChangeEvent bak2 = ItemInput.onValueChanged;

		ItemSlider.onValueChanged = new Slider.SliderEvent();
		ItemInput.onValueChanged = new TMP_InputField.OnChangeEvent();

		this.value = value;
		ItemSlider.value = this.value;
		ItemInput.text = this.value.ToString();

		ItemSlider.onValueChanged = bak1;
		ItemInput.onValueChanged = bak2;
	}

	public void SetValue(float value) {
		Slider.SliderEvent bak1 = ItemSlider.onValueChanged;
		TMP_InputField.OnChangeEvent bak2 = ItemInput.onValueChanged;

		ItemSlider.onValueChanged = new Slider.SliderEvent();
		ItemInput.onValueChanged = new TMP_InputField.OnChangeEvent();

		this.value = (float)Math.Round((double)value, 2);
		ItemSlider.value = this.value;
		ItemInput.text = this.value.ToString();

		ItemSlider.onValueChanged = bak1;
		ItemInput.onValueChanged = bak2;
	}
}
