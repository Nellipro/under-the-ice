using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settingsdriver : MonoBehaviour
{
	[SerializeField] private Slider slider;
	[SerializeField] private TMP_Text textField;
	[SerializeField] private bool showDecimalPoints;

	private void Reset()
	{
		slider = GetComponent<Slider>();
		textField = GetComponentInChildren<TMP_Text>();
	}

	public void HandleSliderValueChanged(float value)
	{
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
