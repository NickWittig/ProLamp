using UnityEngine;
using UnityEngine.UI;

public class ScaleSlider : MonoBehaviour
{


    [SerializeField] private GameObject canvasGo;
    [SerializeField] private Slider slider;
    [SerializeField] private Transform goTransform;
    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("scale", 1f);
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(slider.value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasGo.SetActive(!canvasGo.activeSelf);      
        }
    }

    private void OnSliderValueChanged(float value)
    {
        goTransform.transform.localScale = new Vector3(value, value, value);
        PlayerPrefs.SetFloat("scale", value);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }
}

