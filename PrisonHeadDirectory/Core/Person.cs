namespace Core
{
    public abstract class Person : BaseEntity
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string MiddleName { get; set; }
    }
}