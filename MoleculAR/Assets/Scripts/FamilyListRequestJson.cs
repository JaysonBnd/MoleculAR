[System.Serializable]
public class FamilyDataJson
{
    public string name;
    public string path;
}

[System.Serializable]
public class FamilyListJson
{
    public FamilyDataJson[] families;
}
