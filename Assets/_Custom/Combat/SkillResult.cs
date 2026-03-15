// Data structure representing the result of a skill usage
public struct SkillResult
{
    public bool wasAttempted;
    public bool wasSuccessful;
    public int damageDealt; // If applicable, how much damage the skill dealt
    public int healingDone; // If applicable, how much healing the skill did
    public string failureReason; // If wasAttempted is false, explains why
    public int staminaUsed; // If applicable, how much stamina the skill used
    public int manaUsed; // If applicable, how much mana the skill used

    public SkillResult(bool wasAttempted, bool wasSuccessful, int damageDealt, int healingDone = 0, int staminaUsed = 0, int manaUsed = 0, string failureReason = "")
    {
        this.wasAttempted = wasAttempted;
        this.wasSuccessful = wasSuccessful;
        this.failureReason = failureReason;
        this.damageDealt = damageDealt;
        this.healingDone = healingDone;
        this.staminaUsed = staminaUsed;
        this.manaUsed = manaUsed;

    }
}