using System.ComponentModel.DataAnnotations;

namespace DotNET5Preview8.SinjulMSBH
{
    public class Person
    {
        [Required]
        public byte Age { get; set; } = 28;


        [Range(0, 150)]
        public string Name { get; set; } = "SinjulMSBH";
    }
}
