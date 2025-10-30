using UnityEngine;

public class Currency : MonoBehaviour
{
    public int startingGold = 500;
    public int Gold { get; private set; }

    void Awake() => Gold = startingGold;

    public bool TrySpend(int amt)
    {
        if(Gold < amt)
        {
            return false;
        }

        Gold -= amt;
        return true;
    }

    public void Add(int amt) => Gold += amt;
}
