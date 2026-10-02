using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settingsdriver : MonoBehaviour
{
	private enum MixerGroup
	{
		Master,
		Music,
		SFX,
		Voices
	}

	[SerializeField] private Slider slider;
	[SerializeField] private TMP_Text textField;
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private MixerGroup mixerGroup;
	[SerializeField] private float minDecibels = -80f;
	[SerializeField] private float maxDecibels = 0f;
	[SerializeField] private bool showDecimalPoints;

	private void Reset()
	{
		slider = GetComponent<Slider>();
		textField = GetComponentInChildren<TMP_Text>();
	}

	private void OnEnable()
	{
		if (slider != null)
		{
			slider.onValueChanged.AddListener(HandleSliderValueChanged);
		}
	}

	private void OnDisable()
	{
		if (slider != null)
		{
			slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
		}
	}

	private void Start()
	{
		if (slider == null || audioMixer == null)
		{
			return;
		}

		string volumeParameter = GetVolumeParameter();
		if (audioMixer.GetFloat(volumeParameter, out float decibels))
		{
			float normalizedValue = Mathf.InverseLerp(minDecibels, maxDecibels, decibels);
			float sliderValue = Mathf.Lerp(slider.minValue, slider.maxValue, normalizedValue);
			slider.SetValueWithoutNotify(sliderValue);
			UpdateText(sliderValue);
		}
	}

	public void HandleSliderValueChanged(float value)
	{
		UpdateText(value);

		if (audioMixer == null || slider == null)
		{
			return;
		}

		string volumeParameter = GetVolumeParameter();
		float normalizedValue = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
		float decibels = Mathf.Lerp(minDecibels, maxDecibels, normalizedValue);
		audioMixer.SetFloat(volumeParameter, decibels);
	}

	private string GetVolumeParameter()
	{
		switch (mixerGroup)
		{
			case MixerGroup.Music:
				return "MusicVolume";
			case MixerGroup.SFX:
				return "SFXVolume";
			case MixerGroup.Voices:
				return "VoicesVolume";
			default:
				return "MasterVolume";
		}
	}

	private void UpdateText(float value)
	{
		if (textField == null)
		{
			return;
		}

		if (showDecimalPoints)
		{
			textField.SetText(value.ToString("F2"));
		}
		else
		{
			textField.SetText(value.ToString("F0"));
		}
	}
}
