using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoleculeAtom
{
    public int atomNumber = 0;
    public Vector3 position = Vector3.zero;
}

[System.Serializable]
public class MoleculeBond
{
    public int first = 0;
    public int second = 0;
}

[System.Serializable]
public class MoleculeItem
{
    public List<MoleculeAtom> atomsList = new List<MoleculeAtom>();
    public List<MoleculeBond> bondsList = new List<MoleculeBond>();
}
