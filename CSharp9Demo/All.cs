namespace CSharp9Demo
{
    //! Init-only properties
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Person2
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }

    //! Init accessors and readonly fields
    public class Person3
    {
        private readonly string firstName;
        private readonly string lastName;

        public string FirstName
        {
            get => firstName;
            init => firstName = (value ?? throw new ArgumentNullException(nameof(FirstName)));
        }
        public string LastName
        {
            get => lastName;
            init => lastName = (value ?? throw new ArgumentNullException(nameof(LastName)));
        }
    }

    //! Records
    public data class Person4    
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }

    //! Data members
    public data class Person5 { string FirstName; string LastName; }

    public data class Person6
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }

    //! Positional records
    public data class Person7
    {
        string FirstName;
        string LastName;
        public Person7(string firstName, string lastName)
          => (FirstName, LastName) = (firstName, lastName);
        public void Deconstruct(out string firstName, out string lastName)
          => (firstName, lastName) = (FirstName, LastName);
    }

    public data class Person8(string FirstName, string LastName);

    //! With-expressions and inheritance
    public data class Person9 { string FirstName; string LastName; }
    public data class Student : Person { int ID; }


    //? Covariant returns
    abstract class Animal
    {
        public abstract Food GetFood();
    }
    class Tiger : Animal
    {
        public override Meat GetFood() => ...;
    }



}
