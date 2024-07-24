using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllWands : ScriptableObject
{
    public List<Wand> Wands = new List<Wand>();

    public Wand GetRandomWand()
    {
        int index = Random.Range(0, Wands.Count);
        return Wands[index];
    }

}
