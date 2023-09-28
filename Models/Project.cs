using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        private DateTime _created;
        private DateTime? _updated;
        private DateTime _startDate;
        private DateTime _endDate;

        //Primary Key - to locate one specific record in the database
        public int Id { get; set; }

        //Foreign Key
        public int CompanyId { get; set; }

        [Required]
        public string? Name { get; set; }

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

        public DateTime StartDate
        {
            get
            {
                return _startDate;

            }
            set
            {
                _startDate = value.ToUniversalTime();
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;

            }
            set
            {
                _endDate = value.ToUniversalTime();
            }
        }

        public int ProjectPriorityId { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public bool Archived { get; set; }

        //Navigation Properties
        public virtual Company? Company { get; set; }

        public virtual ProjectPriority? ProjectPriority { get; set; }

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();


    }
}
