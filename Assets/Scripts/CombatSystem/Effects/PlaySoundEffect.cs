using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaySoundEffect : Effect
{
    [SerializeField] string sound;
    public override void Execute(Combatant caster, Combatant target)
    {
        AudioManager.Play(sound);
    }
}
