[System.Serializable]
public struct CamerShakeSettings
{
    public bool shake;
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    public void Shake()
    {
        if (shake)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(
                magnitude,
                roughness,
                fadeInTime,
                fadeOutTime);
        }
    }
}
