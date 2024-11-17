using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowPlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;

    private void OnEnable()
    {
        _nameText.text = LobbyRelayManager.Instance.GetPlayerName();
    }
}
