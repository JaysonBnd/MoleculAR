[System.Serializable]
public class MoleculeDataJson
{
    public string name;
    public string path;
}

[System.Serializable]
public class MoleculeListJson
{
    public MoleculeDataJson[] molecules;
}
