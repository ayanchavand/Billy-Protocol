using UnityEngine;

[RequireComponent(typeof(PlatformerController))]
public class PlayerVFX : MonoBehaviour
{
    public ParticleSystem jumpDust;
    public ParticleSystem landDust;
    public ParticleSystem respawnFlash;
    void PlayJump() => jumpDust?.Play();
    public void PlayLand() => landDust?.Play();
    void PlayRespawn() => respawnFlash?.Play();
}
