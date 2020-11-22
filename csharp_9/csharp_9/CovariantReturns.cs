using System;
namespace csharp_9
{

    public record Food(string FoodName);
    public record Meat() : Food("some_meet");

    abstract class Animal
    {
        public abstract Food GetFood();
    }

    class Tiger : Animal
    {
        public override Meat GetFood() => new Meat();
    }
}