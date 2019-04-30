using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Tests
{
    public class CustomerName : ValueObject
    {
        private CustomerName(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static Result<CustomerName> Create(string value)
        {
            // do invariant enforcement/validation

            return Result.Ok(new CustomerName(value));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}