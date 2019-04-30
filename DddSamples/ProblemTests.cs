using CSharpFunctionalExtensions;
using NUnit.Framework;

namespace Tests.Problem
{
    public class Customer
    {
        public CustomerName Name { get; private set; }
        public Address MainAddress { get; private set; }
        public Address SecondAddress { get; private set; }

        private Customer(CustomerName name, Address mainAddress, Address secondAddress)
        {
            Name = name;
            MainAddress = mainAddress;
            SecondAddress = secondAddress;
        }

        public static Result Create(CustomerName name, Address mainAddress, Address secondAddress)
        {
            if (name.Value.StartsWith("A") && !mainAddress.Street.StartsWith("A"))
                return Result.Fail("If name starts with A then street should also start with A.");

            return Result.Ok(new Customer(name, mainAddress, secondAddress));
        }
    }

    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void Create_WithInvalidSecondAddressAndInvalidCombinationOfNameAndMainAddress_ShouldReturnAllErrorMessages()
        {
            var nameResult = CustomerName.Create("Altmann");
            var mainAddressResult = Address.Create("Str", "1");
            var secondAddressResult = Address.Create("Street2", "2");

            var customerResult = Customer.Create(nameResult.Value, mainAddressResult.Value, secondAddressResult.Value);

            // Passing Value Object in Customer.Create(...) is not possible because Value Objects have to be valid.
        }
    }
}