using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using static System.Console;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Linq;

var user = "Lion-O";
var password = "jaga";
var rememberMe = true;
LoginResourceRecord lrr1 = new(user, password, rememberMe);
var lrr2 = new LoginResourceRecord(user, password, rememberMe);
var lrc1 = new LoginResourceClass(user, password, rememberMe);
var lrc2 = new LoginResourceClass(user, password, rememberMe);

WriteLine($"Test record equality -- lrr1 == lrr2 : {lrr1 == lrr2}");
WriteLine($"Test class equality  -- lrc1 == lrc2 : {lrc1 == lrc2}");
WriteLine($"Print lrr1 hash code -- lrr1.GetHashCode(): {lrr1.GetHashCode()}");
WriteLine($"Print lrr2 hash code -- lrr2.GetHashCode(): {lrr2.GetHashCode()}");
WriteLine($"Print lrc1 hash code -- lrc1.GetHashCode(): {lrc1.GetHashCode()}");
WriteLine($"Print lrc2 hash code -- lrc2.GetHashCode(): {lrc2.GetHashCode()}");
WriteLine($"{nameof(LoginResourceRecord)} implements IEquatable<T>: {lrr1 is IEquatable<LoginResourceRecord>} ");
WriteLine($"{nameof(LoginResourceClass)}  implements IEquatable<T>: {lrr1 is IEquatable<LoginResourceClass>}");
WriteLine($"Print {nameof(LoginResourceRecord)}.ToString -- lrr1.ToString(): {lrr1.ToString()}");
WriteLine($"Print {nameof(LoginResourceClass)}.ToString  -- lrc1.ToString(): {lrc1.ToString()}");


// Record syntax
// Construction follows the requirements of a constructor with parameters (including the allowance for optional parameters).
var login = new LoginResource("Lion-O", "jaga", true);
//You can also use target typing if you prefer.
LoginResource login2 = new("Lion-O", "jaga", true);
// Construction uses object initializers and could look like the following:
LoginResource2 login3 = new()
{
    Username = "Lion-O",
    Password = "jaga",
    RememberMe = true
};
// Construction could look like the following, with RememberMe unspecified.
LoginResource3 login4 = new("Lion-O", "jaga");
//And with RememberMe specified.
LoginResource3 login5 = new("Lion-O", "jaga")
{
    RememberMe = true
};


//I want to make sure that you don’t think that records are exclusively for immutable data. You can opt into exposing mutable properties, as you can see in the following example that reports information about batteries. Model and TotalCapacityAmpHours properties are immutable and RemainingCapacityPercentange is mutable.
Battery battery = new Battery("CR2032", 0.235)
{
    RemainingCapacityPercentage = 100
};
Console.WriteLine(battery);
for (int i = battery.RemainingCapacityPercentage; i >= 0; i--)
{
    battery.RemainingCapacityPercentage = i;
}
Console.WriteLine(battery);


// Non-destructive record mutation
// The login record hasn’t been changed. In fact, that’s impossible. The transformation has only affected loginLowercased. Other than the lowercase transformation to loginLowercased, it’s identical to login.
LoginResource login6 = new("Lion-O", "jaga", true);
LoginResource loginLowercased = login6 with { Username = login6.Username.ToLowerInvariant() };
Console.WriteLine(login6);
Console.WriteLine(loginLowercased);

WriteLine($"Record equality: {login == loginLowercased}");
Console.WriteLine(
    $"Property equality: Username == {login.Username == loginLowercased.Username};" +
    $" Password == {login.Password == loginLowercased.Password};" +
    $" RememberMe == {login.RememberMe == loginLowercased.RememberMe}")
;

// Record inheritance
var weight = 200;
WeightMeasurement measurement = new(DateTime.Now, weight)
{
    Pounds = WeightMeasurement.GetPounds(weight)
};

// Records and Nullability
Author author = new(null, null);
//Console.WriteLine(author.Name.ToString());

// This program compiles and will throw a NullReference exception, due to dereferencing author.Name, which is null.
// To further drive home this point, the following will not compile. author.Name is initialized as null and then cannot be changed, since the property is immutable.
Author2 author2 = new(null, null);
// author2.Name = "Colin Meloy";

Author3 lord = new Author3("Karen Lord")
{
    Website = "https://karenlord.wordpress.com/",
    RelatedAuthors = new()
};
lord.Books.AddRange(
    new Book2[]
    {
        new Book2("The Best of All Possible Worlds", 2013, lord),
        new Book2("The Galaxy Game", 2015, lord)
    }
);
lord.RelatedAuthors.AddRange(
    new Author3[]
    {
        new ("Nalo Hopkinson"),
        new ("Ursula K. Le Guin"),
        new ("Orson Scott Card"),
        new ("Patrick Rothfuss")
    }
);
Console.WriteLine($"Author: {lord.Name}");
Console.WriteLine($"Books: {lord.Books.Count}");
Console.WriteLine($"Related authors: {lord.RelatedAuthors.Count}");


Author3 GetAuthor()
{
    return new Author3("Karen Lord")
    {
        Website = "https://karenlord.wordpress.com/",
        RelatedAuthors = new()
    };
}
Author3 lord2 = GetAuthor();

lord2.RelatedAuthors!.Add(lord);

if (lord2.RelatedAuthors is object)
{
    lord.RelatedAuthors.Add(lord);
}


string serviceURL = "https://localhost:5001/WeatherForecast";
HttpClient client = new();
Forecast2[] forecasts = await client.GetFromJsonAsync<Forecast2[]>(serviceURL);
foreach (Forecast2 forecast2 in forecasts)
{
    Console.WriteLine($"{forecast2.Date}; {forecast2.TemperatureC}C; {forecast2.Summary}");
}

var json = "{\"date\":\"2020-09-06T11:31:01.923395-07:00\",\"temperatureC\":-1,\"temperatureF\":31,\"summary\":\"Scorching\"} ";
var options = new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true,
    IncludeFields = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
var forecast = JsonSerializer.Deserialize<Forecast2>(json, options);

Console.WriteLine(forecast.Date);
Console.WriteLine(forecast.TemperatureC);
Console.WriteLine(forecast.TemperatureF);
Console.WriteLine(forecast.Summary);

var roundTrippedJson = JsonSerializer.Serialize<Forecast2>(forecast, options);

Console.WriteLine(roundTrippedJson);

// Support for records
Forecast3 forecast3 = new(DateTime.Now, 40)
{
    Summary = "Hot!"
};
string forecastJson = JsonSerializer.Serialize<Forecast3>(forecast3);
Console.WriteLine(forecastJson);
Forecast3? forecastObj = JsonSerializer.Deserialize<Forecast3>(forecastJson);
Console.Write(forecastObj);

// Improved Dictionary<K,V> support
Dictionary<int, string> numbers = new()
{
    { 0, "zero" },
    { 1, "one" },
    { 2, "two" },
    { 3, "three" },
    { 5, "five" },
    { 8, "eight" },
    { 13, "thirteen" },
    { 21, "twenty one" },
    { 34, "thirty four" },
    { 55, "fifty five" },
};
var json2 = JsonSerializer.Serialize<Dictionary<int, string>>(numbers);
Console.WriteLine(json);
var dictionary = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
Console.WriteLine(dictionary[55]);

// Support for fields
var json3 = "{\"date\":\"2020-09-06T11:31:01.923395-07:00\",\"temperatureC\":-1,\"temperatureF\":31,\"summary\":\"Scorching\"} ";
var options2 = new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true,
    IncludeFields = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
var forecast4 = JsonSerializer.Deserialize<Forecast3>(json, options);
Console.WriteLine(forecast.Date);
Console.WriteLine(forecast.TemperatureC);
Console.WriteLine(forecast.TemperatureF);
Console.WriteLine(forecast.Summary);
var roundTrippedJson2 = JsonSerializer.Serialize<Forecast3>(forecast4, options2);
Console.WriteLine(roundTrippedJson);

// Preserving references in JSON object graphs
Employee janeEmployee = new()
{
    Name = "Jane Doe",
    YearsEmployed = 10
};
Employee johnEmployee = new()
{
    Name = "John Smith"
};
janeEmployee.Reports = new List<Employee> { johnEmployee };
johnEmployee.Manager = janeEmployee;
JsonSerializerOptions options3 = new()
{
    // NEW: globally ignore default values when writing null or default
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    // NEW: globally allow reading and writing numbers as JSON strings
    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
    // NEW: globally support preserving object references when (de)serializing
    ReferenceHandler = ReferenceHandler.Preserve,
    IncludeFields = true, // NEW: globally include fields for (de)serialization
    WriteIndented = true,
};

string serialized = JsonSerializer.Serialize(janeEmployee, options);
Console.WriteLine($"Jane serialized: {serialized}");

Employee janeDeserialized = JsonSerializer.Deserialize<Employee>(serialized, options);
Console.Write("Whether Jane's first report's manager is Jane: ");
Console.WriteLine(janeDeserialized.Reports[0].Manager == janeDeserialized);


public record LoginResource(string Username, string Password, bool RememberMe);
// The next syntax makes all the properties optional. There is an implicit parameterless constructor provided for the record.
public record LoginResource2
{
    public string Username { get; init; }
    public string Password { get; init; }
    public bool RememberMe { get; init; }
}
// Maybe you want to make those two properties required, with the other one optional. This last pattern would look like the following.
public record LoginResource3(string Username, string Password)
{
    public bool RememberMe { get; init; }
}

// Records are immutable data types
// Records enable you to create immutable data types. This is great for defining types that store small amounts of data.

// The following is an example of a record. It stores user information from a login screen.
public record LoginResourceRecord(string Username, string Password, bool RememberMe);

public class LoginResourceClass
{
    public LoginResourceClass(string username, string password, bool rememberMe)
    {
        Username = username;
        Password = password;
        RememberMe = rememberMe;
    }


    // init is a new keyword that is an alternative to set. set allows you to assign to a property at any time. init allows you to assign to a property only during object construction. It’s the building block that records rely on for immutability. Any type can use init. It isn’t specific to records, as you can see in the previous class definition.
    // private set might seem similar to init; private set prevents other code (outside the type) from mutating data. init will generate compiler errors when a type mutates a property accidentally (after construction). private set isn’t intended to model immutable data, so doesn’t generate any compiler errors or warnings when the type mutates a property value after construction.
    public string Username { get; init; }
    public string Password { get; init; }
    public bool RememberMe { get; init; }
}

public record Battery(string Model, double TotalCapacityAmpHours)
{
    public int RemainingCapacityPercentage { get; set; }
}

public record LoginWithUserDataResource(string Username, string Password, DateTime LastLoggedIn) : LoginResource3(Username, Password)
{
    public int DiscountTier { get; init; }
    public bool FreeShipping { get; init;  }
}

public record WeightMeasurement(DateTime Date, double Kilograms)
{
    public double Pounds { get; init; }
    public static double GetPounds(double kilograms) => kilograms * 2.20462262;
}

public record Author(string Name, List<Book> Books)
{
    public string Website { get; init; }
    public string Genre { get; init; }
    public List<Author> RelatedAuthors { get; init; }
}
// I updated the Author record with null annotations that describe my intended use of the record.
public record Author2(string? Name, List<Book> Books)
{
    public string? Website { get; init; }
    public string? Genre { get; init; }
    public List<Author>? RelatedAuthors { get; init; }
}

public record Book(string name, int Published, Author author);

public record Author3(string Name)
{
    private List<Book2> _books = new();

    public List<Book2> Books => _books;

    public string? Website { get; init; }
    public string? Genre { get; init; }
    public List<Author3>? RelatedAuthors { get; init; }
}

public record Book2(string name, int Published, Author3 author);


// {"date":"2020-09-06T11:31:01.923395-07:00","temperatureC":-1,"temperatureF":31,"summary":"Scorching"}            
public record Forecast(DateTime Date, int TemperatureC, int TemperatureF, string Summary);

public struct Forecast2
{
    public DateTime Date { get; }
    public int TemperatureC { get; }
    public int TemperatureF { get; }
    public string Summary { get; }
    [JsonConstructor]
    public Forecast2(DateTime date, int temperatureC, int temperatureF, string summary) => (Date, TemperatureC, TemperatureF, Summary) = (date, temperatureC, temperatureF, summary);
}

public record Forecast3(DateTime Date, int TemperatureC)
{
    public string? Summary { get; init; }
};

public class Forecast4
{
    public DateTime Date;
    public int TemperatureC;
    public int TemperatureF;
    public string Summary;
}

public class Employee
{
    // NEW: Allows use of non-public property accessor.
    // Can also be used to include fields "per-field", rather than globally with JsonSerializerOptions.
    [JsonInclude]
    public string Name { get; internal set; }

    public Employee Manager { get; set; }

    public List<Employee> Reports;

    public int YearsEmployed { get; set; }

    // NEW: Always include when (de)serializing regardless of global options
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool IsManager => Reports?.Count > 0;
}