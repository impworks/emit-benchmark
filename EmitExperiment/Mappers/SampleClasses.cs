using System;

namespace EmitExperiment.Mappers
{
    /// <summary>
    /// Database-bound profile of the user.
    /// </summary>
    public class UserProfile
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public bool IsSuspended { get; set; }
        public DateTime? LastEdit { get; set; }
    }

    /// <summary>
    /// Safe object for data transfering.
    /// </summary>
    public class UserDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public DateTime? LastEdit { get; set; }
    }
}
