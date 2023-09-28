namespace BugTracker.Models
{
    public class TicketPriority
    {
        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
