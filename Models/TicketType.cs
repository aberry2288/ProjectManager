namespace BugTracker.Models
{
    public class TicketType
    {
        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
