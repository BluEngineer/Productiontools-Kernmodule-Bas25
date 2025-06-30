public class QuestReward
{
    public string ItemID;
    public int Quantity;
    public int Gold;
    public int Experience;

    public QuestReward Clone()
    {
        return new QuestReward
        {
            ItemID = this.ItemID,
            Quantity = this.Quantity,
            Gold = this.Gold,
            Experience = this.Experience
        };
    }
}