using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    PlayerThrowCoin playerThrowCoin;
    TMP_Text text;

    int maxCoins;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        playerThrowCoin = PlayerLocator.Player.GetComponent<PlayerThrowCoin>();
        maxCoins = playerThrowCoin.MaxCoinCount;
        playerThrowCoin.coinsChanged += OnCoinsChanged;
        OnCoinsChanged(playerThrowCoin.MaxCoinCount);
    }

    void OnCoinsChanged(int coins)
    {
        text.text = coins + " / " + maxCoins;
    }
}
