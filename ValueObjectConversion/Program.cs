using System;

namespace ValueObjectConversion
{
    public abstract class ValueObject<T>
    {
        public T Value { get; set; }
    }

    public class RequiredStringVO : ValueObject<string>
    {
        public static implicit operator string(RequiredStringVO vo)
        {
            return vo.GetValueOrThrow();
        }
    }

    public class RequiredDecimalVO : ValueObject<decimal>
    {
        public static implicit operator decimal(RequiredDecimalVO vo)
        {
            return vo.GetValueOrThrow();
        }
    }

    public class OptionalStringVO : ValueObject<string>
    {
        public static implicit operator string(OptionalStringVO vo)
        {
            return vo.GetValueOrNull();
        }
    }

    public class OptionalDecimalVO : ValueObject<decimal>
    {
        public static implicit operator decimal(OptionalDecimalVO vo)
        {
            return vo.GetValueOrThrow();
        }

        public static implicit operator decimal?(OptionalDecimalVO vo)
        {
            return vo.GetValueOrNull();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
        }

        private static void RequiredStringValueObject()
        {
            RequiredStringVO requiredStringVO = null;
            RequiredStringVO requiredStringVO2 = new RequiredStringVO();

            string requiredStringVOString = requiredStringVO; //.GetValueOrThrow();
            string requiredStringVOString2 = requiredStringVO2; //.GetValueOrThrow();
        }

        private static void RequiredDecimalValueObject()
        {
            RequiredDecimalVO requiredDecimalVO = null;
            RequiredDecimalVO requiredDecimalVO2 = new RequiredDecimalVO();

            decimal requiredDecimalVODecimal = requiredDecimalVO; //.GetValueOrThrow();
            decimal requiredDecimalVODecimal2 = requiredDecimalVO2; //.GetValueOrThrow();

            decimal? nullarequiredDecimalVODecimal = requiredDecimalVO; //.GetValueOrThrow();
            decimal? nullarequiredDecimalVODecimal2 = requiredDecimalVO2; //.GetValueOrThrow();
        }

        private static void OptionalStringValueObject()
        {
            OptionalStringVO optionalStringVO = null;
            OptionalStringVO optionalStringVO2 = new OptionalStringVO();

            string optionalStringVOString = optionalStringVO; //.GetValueOrDefault();
            string optionalStringVOString2 = optionalStringVO2; //.GetValueOrDefault();
        }

        private static void OptionalDecimalValueObject()
        {
            OptionalDecimalVO optionalDecimalVO = null;
            OptionalDecimalVO optionalDecimalVO2 = new OptionalDecimalVO();

            decimal optionalDecimalVODecimal = optionalDecimalVO; //.GetValueOrDefault();
            decimal optionalDecimalVODecimal2 = optionalDecimalVO2; //.GetValueOrDefault();

            decimal? nullOptionalDecimalVODecimal = optionalDecimalVO; //.GetValueOrNull();
            decimal? nullOptionalDecimalVODecimal2 = optionalDecimalVO2; //.GetValueOrNull();

            // context depented if required
            decimal requiredDecimalVODecimal = optionalDecimalVO.GetValueOrThrow();
        }
    }

    public static class ObjectExtensions
    {
        // converting required struct and string value object
        public static T GetValueOrThrow<T>(this ValueObject<T> o)
        {
            if (o == null)
                throw new Exception();

            return o.Value;
        }

        public static T GetValueOrDefault<T>(this ValueObject<T> o, T defaultValue = default)
        {
            return o == null
              ? defaultValue
              : o.Value;
        }

        // converting optional string value object
        public static string GetValueOrNull(this ValueObject<string> o)
        {
            return o?.Value;
        }

        // converting optional struct value object
        public static T? GetValueOrNull<T>(this ValueObject<T> o) 
            where T : struct
        {
            return o?.Value;
        }
    }
}