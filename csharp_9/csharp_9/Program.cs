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



            
        }

        private static bool IsNotNullAndEmpty(string value) => value is not null && value is not "";

        public static bool IsValidAddress(Address address) => 
            IsNotNullAndEmpty(address.City) && IsNotNullAndEmpty(address.PostalCode)
                    && IsNotNullAndEmpty(address.Street);

        public static void CheckCity(Address address)
        {

        }

             address switch
                {
                    Address a when a.City == "Gdańsk" => Console.WriteLine("Oh that's Gdańsk"),
                    Address a when a.City == "Toruń" => Console.WriteLine("Oh that's Toruń"),
                    _ => Console.WriteLine("It's good to have address")
                };


    }
}
