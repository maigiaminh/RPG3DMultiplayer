
[System.Serializable]
public class ItemStack 
{
    public ItemData item;
    public int quantity; 
    public ItemStack(ItemData item, int quantity){
        this.item = item;
        this.quantity = quantity;
    }
}
