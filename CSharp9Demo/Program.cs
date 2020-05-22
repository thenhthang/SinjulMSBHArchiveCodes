using System;

namespace CSharp9Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            new Person2
            {
                FirstName = "Scott",
                LastName = "Hunter"
            };


            //? With-expressions
            var otherPerson = Person4 with { LastName = "Hanselman" };

            protected Person(Person original) { /* copy all the fields */ } // generated

            //! Value - based equality
            var originalPerson = otherPerson with { LastName = "Hunter" };

            var person = new Person8("Scott", "Hunter"); // positional construction
            var (f, l) = person;                        // positional deconstruction

            Person9 person = new Student { FirstName = "Scott", LastName = "Hunter", ID = GetNewId() };
            otherPerson = person with { LastName = "Hanselman" };

            //? Value-based equality and inheritance
            Person9 person1 = new Person9 { FirstName = "Scott", LastName = "Hunter" };
            Person9 person2 = new Student { FirstName = "Scott", LastName = "Hunter", ID = GetNewId() };


            //? Improved pattern matching
            public static decimal CalculateToll(object vehicle) =>
              vehicle switch
              {

                  DeliveryTruck t when t.GrossWeightClass > 5000 => 10.00m + 5.00m,
                  DeliveryTruck t when t.GrossWeightClass < 3000 => 10.00m - 2.00m,
                  DeliveryTruck _ => 10.00m,

                  _ => throw new ArgumentException("Not a known vehicle type", nameof(vehicle))
              };

            //? Simple type patterns
            DeliveryTruck => 10.00m,

            //? Relational patterns
            DeliveryTruck t when t.GrossWeightClass switch
            {
                > 5000 => 10.00m + 5.00m,
                < 3000 => 10.00m - 2.00m,
                _ => 10.00m,
            },

            //? Logical patterns
            DeliveryTruck t when t.GrossWeightClass switch
            {
                < 3000 => 10.00m - 2.00m,
                >= 3000 and <= 5000 => 10.00m,
                > 5000 => 10.00m + 5.00m,
            },

            not null => throw new ArgumentException($"Not a known vehicle type: {vehicle}", nameof(vehicle)),
            null => throw new ArgumentNullException(nameof(vehicle))

            if (!(e is Customer)) { ... }

            //? You can just say
            if (e is not Customer) { ... }

            //? Target - typed new expressions
            Point p = new(3, 5);

            //? Target typed ?? and ?:
            Person person = student ?? customer; // Shared base type
            int? result = b ? 0 : null; // nullable value type


        }
    }
}

//! Top-level programs
using System;

Console.WriteLine("Hello World!");