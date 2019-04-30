using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Solution
{
    public class ValidateableParam<T>
    {
        public T Value { get; private set; }

        public bool IsValid => Value != null;

        public ValidateableParam(T value)
        {
            Value = value;
        }

        public static implicit operator ValidateableParam<T>(Result<T> result)
        {
            return result.ToValidateableParam();
        }
    }

    public static class Extensions
    {
        public static ValidateableParam<T> ToValidateableParam<T>(this Result<T> result)
        {
            return result.IsFailure
                ? new ValidateableParam<T>(default(T))
                : new ValidateableParam<T>(result.Value);
        }
    }


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

        public static Result Create(ValidateableParam<CustomerName> name,
            ValidateableParam<Address> mainAddress,
            ValidateableParam<Address> secondAddress)
        {
            if (name.IsValid && name.Value.Value.StartsWith("A")
                && mainAddress.IsValid && !mainAddress.Value.Street.StartsWith("A"))
                return Result.Fail("If name starts with A then street should also start with A.");

            return Result.Ok(new Customer(name.Value,
                mainAddress.Value,
                secondAddress.Value));
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
            
            var customerResult = Customer.Create(nameResult, mainAddressResult, secondAddressResult);

            var validationResult = Result.Combine(
                nameResult,
                mainAddressResult,
                secondAddressResult,
                customerResult);

            validationResult.Error.Should().Be("Street Length > 5, If name starts with A then street should also start with A.");
        }
    }
}