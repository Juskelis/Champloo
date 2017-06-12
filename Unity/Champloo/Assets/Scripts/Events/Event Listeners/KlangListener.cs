using System;
using UnityEngine;

public class KlangListener : MonoBehaviour
{
    [SerializeField]
    private float klangKnockBack = 20f;

    [SerializeField]
    private Transform spawnOnKlang;

    [SerializeField]
    private CamerShakeSettings shakeSettings;

    void Start()
    {
        EventDispatcher.Instance.AddListener<KlangEvent>(OnKlang);
    }

    void OnDestroy()
    {
        EventDispatcher dispatcher = EventDispatcher.Instance;
        if (dispatcher != null)
        {
            dispatcher.RemoveListener<KlangEvent>(OnKlang);
        }
    }

    private void OnKlang(object sender, EventArgs args)
    {
        KlangEvent convertedArgs = (KlangEvent) args;
        Weapon wA = convertedArgs.A;
        Weapon wB = convertedArgs.B;
        Player pA = wA.OurPlayer;
        Player pB = wB.OurPlayer;
        Vector3 centerOfEvent = (wA.transform.position + wB.transform.position)/2;
        if (shakeSettings.shake)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(
                shakeSettings.magnitude,
                shakeSettings.roughness,
                shakeSettings.fadeInTime,
                shakeSettings.fadeOutTime);
        }

        //reset weapons
        wA.Reset();
        wB.Reset();

        //cancel hits
        pA.CancelHit();
        pB.CancelHit();

        //pushback
        pA.ApplyForce((pA.transform.position - centerOfEvent).normalized * klangKnockBack);
        pB.ApplyForce((pB.transform.position - centerOfEvent).normalized * klangKnockBack);

        //create effect
        Instantiate(spawnOnKlang, centerOfEvent, Quaternion.identity);
    }
}
