using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Tests
{
    public class Address : ValueObject
    {
        private Address(string street, string number)
        {
            Street = street;
            Number = number;
        }

        public string Street { get; set; }
        public string Number { get; set; }

        public static Result<Address> Create(string street, string number)
        {
            // do invariant enforcement/validation
            var result = Result.Create(street.Length <= 5, "Street Length > 5");

            return result.IsFailure
                ? Result.Fail<Address>(result.Error)
                : Result.Ok(new Address(street, number));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return Number;
        }
    }
}