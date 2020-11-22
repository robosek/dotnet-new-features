using System;
namespace csharp_9
{
    public record Address
    {
        public string City { get; init; }
        public string PostalCode { get; init; }
        public string Street { get; init; }
    }
     
    public record Person(string FirstName, string LastName)
    {
        public void IntroduceMySelf()
        {
            Console.WriteLine($"Hello my name is {FirstName} {LastName}");
        }
    }

    public record Pet(string Name);

    public class Point
    {
        public int Y { get; init; }
        public int X { get; init; }

        public Point(int x, int y)
        {
            Y = y;
            X = x;
        }
    }

    public record PointRecord(int X, int Y);

}