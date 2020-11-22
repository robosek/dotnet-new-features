using System;

namespace csharp_9
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person("Robert", "Sęk");
            var address = new Address()
            {
                City = "Gdańsk",
                PostalCode = "80-200",
            };

            var pet = new Pet("doggy");

            if (IsValidAddress(address))
                Console.WriteLine("Address is valid");
            else
            {
                var newAddress = address with { Street = "Morelowa" };
                Console.WriteLine($"Is valid address?: {IsValidAddress(newAddress)}");
            }

            Point point = new (3, 5);
            PointRecord pointRecord = new(3, 5);
            PointRecord pointRecord1 = new(3, 5);

            Console.WriteLine($"Are records equal? {pointRecord == pointRecord1}");
        }

        private static bool IsNotNullAndEmpty(string value) => value is not null && value is not "";

        public static bool IsValidAddress(Address address) => 
            IsNotNullAndEmpty(address.City) && IsNotNullAndEmpty(address.PostalCode)
                    && IsNotNullAndEmpty(address.Street);

        public static string CheckCity(Address address) =>
            address switch
            {
                Address { City: "Gdańsk" } => "Oh that's Gdańsk",
                Address { City: "Toruń" } => "Oh that's Toruń",
                _ => "It's good to have address"
            };
    }
}
