[System.Serializable]
public class BondsConnnectionsJson
{
    public int[] index;
}

[System.Serializable]
public class BondsJson
{
    public BondsConnnectionsJson connections;
    public int[] order;
}

[System.Serializable]
public class MoleculeRequestJson
{
    public BondsJson bonds;
    public int[] elements;
    public float[] coords;

}
