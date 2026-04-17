namespace RequestManagement.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public  ICollection<Request> Requests { get; set; } 
                                                                                
    }
}