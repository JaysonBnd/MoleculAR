using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomData : ScriptableObject
{
    public List<Atom> Atoms = new List<Atom>();

    public Atom AtomOfType(AtomType type )
    {
        return this.Atoms[(int)type - 1];
    }
}
