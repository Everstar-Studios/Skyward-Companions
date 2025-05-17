using UnityEngine;

public static class Settings
{
    private const string SensitivityKey = "CameraSensitivity";
    private static float _sensitivity = -1f;

    public static float Sensitivity
    {
        get
        {
            if (_sensitivity < 0)
                _sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 60f);
            return _sensitivity;
        }
        set
        {
            _sensitivity = value;
            PlayerPrefs.SetFloat(SensitivityKey, value);
            PlayerPrefs.Save();
        }
    }
}