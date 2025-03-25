using UnityEngine;

public abstract class ButtonAsset : ScriptableObject
{
    public AudioClip clickSound;

    public virtual void Action()
    {
        if (clickSound != null)
            AudioSystem.Play(clickSound);
    }
}
