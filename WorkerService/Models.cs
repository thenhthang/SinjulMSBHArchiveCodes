using System;
using System.Collections.Generic;

namespace WorkerService
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Uri Source { get; set; }

        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }

    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public int BogId { get; set; }
        public Blog Blog { get; set; }

        public int AuthorId { get; set; }
        public Person Author { get; set; }
    }

    public class MediaPost : Post
    {
        public string MediaType { get; set; }
        public Uri MediaUrl { get; set; }
    }
}
