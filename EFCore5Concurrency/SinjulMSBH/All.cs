using System.ComponentModel.DataAnnotations;

namespace EFCore5Concurrency.SinjulMSBH
{
    public class Actor
    {
        public int ActorId { get; set; }
        [ConcurrencyCheck] public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
    }


    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        [Timestamp] public byte[] Timestamp { get; set; }
    }


    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        [Timestamp] public byte[] Timestamp { get; set; }
    }
}
