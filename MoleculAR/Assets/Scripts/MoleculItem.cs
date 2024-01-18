using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtomItem
{
    public int atomNumber = 0;
    public Vector3 position = Vector3.zero;
}

public class AtomBond
{
    public int first = 0;
    public int second = 0;
}

[System.Serializable]
public class MoleculeItem
{
    public List<AtomItem> atomsList = new List<AtomItem>();
    public List<AtomBond> bondsList = new List<AtomBond>();
}
