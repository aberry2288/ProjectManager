using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Notification
    {
        private DateTime _created;

        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        //Foreign Key
        public int? ProjectId { get; set; }

        //Foreign Key
        public int? TicketId { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Message { get; set; }

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

        //Foreign Key
        [Required]
        public string? SenderId { get; set; }

        //Foreign Key
        [Required]
        public string? RecipientId { get; set; }

        public int NotificationTypeId { get; set; }

        public bool HasBeenViewed { get; set; }

        //Navigation Properties
        public virtual NotificationType? NotificationType { get; set; }

        public virtual Ticket? Ticket { get; set; }

        public virtual Project? Project { get; set; }

        public virtual BTUser? Sender { get; set; }

        public virtual BTUser? Recipient { get; set; }


    }
}
