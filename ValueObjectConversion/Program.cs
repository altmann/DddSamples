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

            string optionalStringVOString = optionalStringVO; //.GetValueOrNull();
            string optionalStringVOString2 = optionalStringVO2; //.GetValueOrNull();
        }

        private static void OptionalDecimalValueObject()
        {
            OptionalDecimalVO optionalDecimalVO = null;
            OptionalDecimalVO optionalDecimalVO2 = new OptionalDecimalVO();

            decimal optionalDecimalVODecimal = optionalDecimalVO; //.GetValueOrThrow();
            decimal optionalDecimalVODecimal2 = optionalDecimalVO2; //.GetValueOrThrow();

            decimal? nullOptionalDecimalVODecimal = optionalDecimalVO; //.GetValueOrNull();
            decimal? nullOptionalDecimalVODecimal2 = optionalDecimalVO2; //.GetValueOrNull();

            // context dependent if required
            decimal requiredDecimalVODecimal = optionalDecimalVO; //.GetValueOrThrow();
            decimal? nullOptionalDecimalVODecimal3 = optionalDecimalVO; //.GetValueOrNull();
        }

        // Lösung 1 - Simple Umsetzung
        // Es wird bei der Casting Logik nicht nach Optional/Required unterschieden.
        // Die Casting Logik beinhaltet immer die Optional VO Logik
        //   - Bei string VO auf string: GetValueOrNull()
        //   - Bei struct VO auf struct: GetValueOrThrow()
        //   - Bei struct VO auf nullable struct: GetValueOrNull()
        // VT: Einheitliche Logik
        // NT: struct VO is required. 
        //     Bei Konvertierung von required struct VO auf nullable struct"
        //     wird keine Exception geworfen, da die Logik GetValueOrNull() ist.
        //     In diesem Fall wäre besser GetValueOrThrow(). siehe Zeile 97. 

        // Lösung 2 - Komplexere Umsetzung
        // Es wird bei der Casting Logik nach Optional/Required unterschieden. 
        //   - Bei Optional VO oben umgesetzte Casting Logik
        //   - Bei Required VO oben umgesetzte Casting Logik
        // VT: Nachteil von Lösung 1 ist behoben. 
        // NT: Wird das VO geshared (ein mal optional, ein mal required), dann muss das VO
        //     nur mit der Optional Cast Logik umgesetzt werden. 
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