using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Invite
    {
        private DateTime _created;
        private DateTime? _updated;
        private DateTime _inviteDate;
        private DateTime _joinDate;

        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        public DateTime InviteDate
        {
            get
            {
                return _inviteDate;

            }
            set
            {
                _inviteDate = value.ToUniversalTime();
            }
        }

        public DateTime JoinDate
        {
            get
            {
                return _joinDate;

            }
            set
            {
                _joinDate = value.ToUniversalTime();
            }
        }

        public Guid CompanyToken { get; set; }

        //Foreign Key
        public int CompanyId { get; set; }

        //Foreign Key
        public int ProjectId { get; set; }

        //Foreign Key
        [Required]
        public string? InvitorId { get; set; }

        //Foreign Key
        public string? InviteeId { get; set; }

        [Required]
        public string? InviteeEmail { get; set; }

        [Required]
        public string? InviteeFirstName { get; set; }

        [Required]
        public string? InviteeLastName { get; set; }

        public string? Message { get; set; }

        public bool IsValid { get; set; }

        //Navigation Properties
        public virtual Company? Company { get; set; }

        public virtual Project? Project { get; set; }

        public virtual BTUser? Invitor { get; set; } 

        public virtual BTUser? Invitee { get; set; }




    }
}
