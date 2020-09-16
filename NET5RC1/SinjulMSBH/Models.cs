using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET5RC1.SinjulMSBH
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<Post> Posts { get; set; }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Post> Posts { get; set; }
    }


    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
    }
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
    }


    // Better migrations column ordering
    // https://github.com/dotnet/efcore/issues/11314
    class MyEntity
    {
        public int Id { get; set; }

        [Column(Order = 2)]
        public DateTime DateCreated { get; set; }
    }

    class Persion : MyEntity
    {
        public string Name { get; set; }
    }


}
