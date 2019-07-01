using CSharpFunctionalExtensions;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Tests.Solution
{
    public class ValidateableParam<T>
    {
        private Result<T> _result;

        public T Value => _result.Value;

        public string Error => _result.Error;

        public bool IsValid => _result.IsSuccess;
        
        public ValidateableParam(Result<T> result)
        {
            _result = result;
        }

        public static implicit operator Result<T> (ValidateableParam<T> validateableParam)
        {
            return validateableParam.IsValid
                ? Result.Ok(validateableParam.Value)
                : Result.Fail<T>(validateableParam.Error);
        }

        public static implicit operator Result(ValidateableParam<T> validateableParam)
        {
            return validateableParam.IsValid
                ? Result.Ok(validateableParam.Value)
                : Result.Fail<T>(validateableParam.Error);
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
            return new ValidateableParam<T>(result);
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

        private static bool IfNameStartsWithAThenStreetAlsoShouldStartWithA(CustomerName name,
                Address mainAddress)
        {
            return name.Value.StartsWith("A") && mainAddress.Street.StartsWith("A");
        }

        public static Result<Customer> Create(ValidateableParam<CustomerName> name,
            ValidateableParam<Address> mainAddress,
            ValidateableParam<Address> secondAddress)
        {
            var nameMainAddressCombinationResult = Validate(name, mainAddress,
                IfNameStartsWithAThenStreetAlsoShouldStartWithA, 
                "If name starts with A then street should also start with A.");
            
            var result = Result.Combine(nameMainAddressCombinationResult, secondAddress);
            if (result.IsFailure)
                return Result.Fail<Customer>(result.Error);

            return Result.Ok(new Customer(name.Value,
                mainAddress.Value,
                secondAddress.Value));
        }

        private static Result Validate<T1, T2>(ValidateableParam<T1> param1, 
            ValidateableParam<T2> param2, Func<T1, T2, bool> isValidExpression, string errorMessage)
        {
            Result param1Result = param1;
            Result param2Result = param2;

            var combinationResult = Result.Combine(param1Result, param2Result);
            if (combinationResult.IsFailure)
                return combinationResult;

            var isValid = isValidExpression(param1.Value, param2.Value);

            return isValid ? Result.Ok() : Result.Fail(errorMessage);
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