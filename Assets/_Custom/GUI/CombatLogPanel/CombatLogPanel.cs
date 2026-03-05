using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatLogPanel : MonoBehaviour
{
    public int maxMessages = 20;

    public GameObject combatLogContent, textObject;
    public Color info, enemyAttack, playerAttack;

    [SerializeField] List<CombatMessage> combatMessageList = new List<CombatMessage>();

    public void SendMessageToCombatLog(string text, CombatMessage.CombatMessageType combatMessageType)
    {
        if (combatMessageList.Count >= maxMessages)
        {
            Destroy(combatMessageList[0].textObject.gameObject);
            combatMessageList.Remove(combatMessageList[0]);
        }

        CombatMessage newMessage = new CombatMessage();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, combatLogContent.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(combatMessageType);

        combatMessageList.Add(newMessage);
    }

    Color MessageTypeColor(CombatMessage.CombatMessageType combatMessageType)
    {
        Color color = info;

        switch (combatMessageType)
        {
            case CombatMessage.CombatMessageType.playerAttack:
                color = playerAttack;
                break;
            case CombatMessage.CombatMessageType.enemyAttack:
                color = enemyAttack;
                break;
            case CombatMessage.CombatMessageType.info:
                color = info;
                break;
            default:
                break;
        }
        return color;
    }
}

[System.Serializable]
public class CombatMessage
{
    public string text;
    public TMP_Text textObject;
    public CombatMessageType combatMessageType;

    public enum CombatMessageType
    {
        playerAttack,
        enemyAttack,
        info
    }
}
