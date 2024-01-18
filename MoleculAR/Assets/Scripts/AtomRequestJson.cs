[System.Serializable]
public class AtomAtomsJson
{
    public int AtomicNumber;
    public string Symbol;
    public float Scale;
    public int R;
    public int G;
    public int B;
    public int A;
}

[System.Serializable]
public class AtomRequestJson
{
    public AtomAtomsJson[] atoms;
}
