using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Solution
{
    public class ValidateableParam<T>
    {
        private Result<T> _result;

        public T Value => _result.Value;

        public string Error => _result.Error;

        public bool IsValid => _result.IsSuccess;

        public bool IsValidAndHasValue => IsValid && HasValue;

        public Result Result => _result;

        public bool HasValue => _result.IsSuccess && Value != null;

        public bool HasNoValue => !HasValue;

        public ValidateableParam(Result<T> result)
        {
            _result = result;
        }

        public ValidateableParam(T value)
        {
            _result = Result.Ok(value);
        }

        public static implicit operator ValidateableParam<T>(Result<T> result)
        {
            return new ValidateableParam<T>(result);
        }

        public static implicit operator ValidateableParam<T>(T value)
        {
            return new ValidateableParam<T>(value);
        }
    }

    public class Customer
    {
        public CustomerName Name { get; private set; } //Required
        public Address MainAddress { get; private set; } //Required
        public Address SecondAddress { get; private set; } //Optional

        private Customer(CustomerName name, Address mainAddress, Address secondAddress)
        {
            Name = name;
            MainAddress = mainAddress;
            SecondAddress = secondAddress;
        }

        private static bool IfNameStartsWithAThenStreetAlsoShouldStartWithA(CustomerName name,
                Address mainAddress)
        {
            return name.Value.StartsWith("A") && mainAddress.Street.StartsWith("A");
        }

        public static Result<Customer> Create(ValidateableParam<CustomerName> name,
            ValidateableParam<Address> mainAddress,
            ValidateableParam<Address> secondAddress)
        {
            var nameRequiredResult = Result.Create(name.HasValue, "Customer Name is required");
            var mainAddressRequiredResult = Result.Create(mainAddress.HasValue, "Main Address is required");

            var nameMainAddressCombinationResult = name.IsValidAndHasValue 
                && mainAddress.IsValidAndHasValue
                && !IfNameStartsWithAThenStreetAlsoShouldStartWithA(name.Value, mainAddress.Value)
                ? Result.Fail("If name starts with A then street should also start with A.")
                : Result.Ok();
                
            var result = Result.Combine(
                nameRequiredResult,
                mainAddressRequiredResult,
                name.Result, 
                mainAddress.Result, 
                nameMainAddressCombinationResult, 
                secondAddress.Result);

            if (result.IsFailure)
                return Result.Fail<Customer>(result.Error);

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

            nameResult.IsSuccess.Should().BeTrue();
            mainAddressResult.IsSuccess.Should().BeTrue();
            secondAddressResult.IsSuccess.Should().BeFalse();
            customerResult.IsSuccess.Should().BeFalse();
            customerResult.Error.Should().Be("If name starts with A then street should also start with A., Street Length > 5");
        }

        [Test]
        public void Create_WithInvalidSecondAddress_ShouldReturnAllErrorMessages()
        {
            var nameResult = CustomerName.Create("Altmann");
            var mainAddressResult = Address.Create("Aa", "1");
            var secondAddressResult = Address.Create("Street2", "2");

            var customerResult = Customer.Create(nameResult, mainAddressResult, secondAddressResult);
            
            nameResult.IsSuccess.Should().BeTrue();
            mainAddressResult.IsSuccess.Should().BeTrue();
            secondAddressResult.IsSuccess.Should().BeFalse();
            customerResult.IsSuccess.Should().BeFalse();
            customerResult.Error.Should().Be("Street Length > 5");
        }

        [Test]
        public void Create_WithNullSecondAddress_ShouldReturnAllErrorMessages()
        {
            var nameResult = CustomerName.Create("Altmann");
            var mainAddressResult = Address.Create("Aa", "1");
            Address secondAddress = null;

            var customerResult = Customer.Create(nameResult, mainAddressResult, secondAddress);

            nameResult.IsSuccess.Should().BeTrue();
            mainAddressResult.IsSuccess.Should().BeTrue();
            customerResult.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Create_WithNullMainAddress_ShouldReturnAllErrorMessages()
        {
            var nameResult = CustomerName.Create("Altmann");
            Address mainAddress = null;
            var secondAddressResult = Address.Create("Stree", "2");

            var customerResult = Customer.Create(nameResult, mainAddress, secondAddressResult);

            nameResult.IsSuccess.Should().BeTrue();
            secondAddressResult.IsSuccess.Should().BeTrue();
            customerResult.IsSuccess.Should().BeFalse();
            customerResult.Error.Should().Be("Main Address is required");
        }

        [Test]
        public void Create_WithValidSecondAddressAndValidCombinationOfNameAndMainAddress_ShouldReturnAllErrorMessages()
        {
            var nameResult = CustomerName.Create("Altmann");
            var mainAddressResult = Address.Create("Aaaa", "1");
            var secondAddressResult = Address.Create("Stree", "2");

            var customerResult = Customer.Create(nameResult, mainAddressResult, secondAddressResult);

            nameResult.IsSuccess.Should().BeTrue();
            mainAddressResult.IsSuccess.Should().BeTrue();
            secondAddressResult.IsSuccess.Should().BeTrue();
            customerResult.IsSuccess.Should().BeTrue();
        }
    }
}