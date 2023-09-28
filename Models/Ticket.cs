using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Ticket
    {
        private DateTime _created;
        private DateTime? _updated;

        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
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

        public DateTime? Updated
        {
            get => _updated;
            set
            {
                if (value.HasValue)
                {
                    _updated = value.Value.ToUniversalTime();
                }
                else
                {
                    _updated = null;
                }
            }

        }

        public bool Archived { get; set; }

        public bool ArchivedByProject { get; set; }

        //Foreign Key
        public int ProjectId { get; set; }

        //Foreign Key
        public int TicketTypeId { get; set; }

        //Foreign Key
        public int TicketStatusId { get; set; }

        //Foreign Key
        public int TicketPriorityId { get; set; }

        //Foreign Key
        public string? DeveloperUserId { get; set; }

        //Foreign Key
        [Required]
        public string? SubmitterUserId { get; set; }

        //Navigation Properties
        public virtual Project? Project { get; set; }

        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual TicketType? TicketType { get; set; }

        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual BTUser? DeveloperUser { get; set; }

        public virtual BTUser? SubmitterUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();

        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();


    }
}
