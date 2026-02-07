namespace FamilyRestraunt.Models
{
    public class Orders
    {
        public Orders() { 
               OrderItems = new List<OrderItem>();
        }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string? UserId { get; set; }
        public ApplicationUsers? Users { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
