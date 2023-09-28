namespace BugTracker.Models
{
    public class TicketHistory
    {
        private DateTime _created;

        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        //Foreign Key
        public int TicketId { get; set; }

        public string? PropertyName { get; set; }

        public string? Description { get; set; }

        public DateTime Created
        {
            get
            {
                return _created;

            }
            set
            {
                _created = value.ToUniversalTime();
            }

        }

        public string? OldValue { get; set; }

        public string? NewValue { get; set;}

        //Foreign Key
        public string? UserId { get; set; }

        //Navigation Properties
        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? User { get; set; }
    }
}
