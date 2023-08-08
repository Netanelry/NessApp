using System.ComponentModel.DataAnnotations.Schema;

namespace NessServer.Models
{
    [Table("Client")]
    public class Client
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("Full_Name")]
        public string FullName { get; set; }
        [Column("Phone_Number")]
        public string PhoneNumber { get; set; }
        [Column("IP_Address")]
        public string IPAddress { get; set; }
    }
}
