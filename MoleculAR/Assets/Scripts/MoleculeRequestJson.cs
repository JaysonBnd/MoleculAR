[System.Serializable]
public class BondsConnectionsJson
{
    public int[] index;
}

[System.Serializable]
public class BondsJson
{
    public BondsConnectionsJson connections;
    public int[] order;
}

[System.Serializable]
public class MoleculeRequestJson
{
    public BondsJson bonds;
    public int[] elements;
    public float[] coords;

}
