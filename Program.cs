using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeradorCpfs
{
    class Program
    {
        public const int MAX = 15000;

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            TextWriter w = new StreamWriter("cpfs.csv");

            for (int i = 0; i < MAX; i++)
            {
                w.WriteLine(new Cpf(rnd.Next(1, 999999999).ToString("000000000")));
            }

            w.Close();

        }

        public class Cpf
        {
            private string initialValue;
            public Cpf()
            {
            }
            public Cpf(string initialValue, string digits = null)
            {
                this.initialValue = initialValue;
                if (digits != null)
                {
                    if (Validate($"{this.initialValue}{digits}"))
                        throw new ArgumentException($"{this.initialValue}{digits} is invalid a CPF.");
                };
            }

            public static class Patterns
            {
                public readonly static string NumericFormat = "^([0-9]{11})$";
                public readonly static string StringFormat = "^([0-9]{3})\\.([0-9]{3})\\.([0-9]{3})\\-([0-9]{2})$";
                public readonly static string StringFormatReplace = "\\1\\2\\3";
            }
            public static string CpfFromString(string value)
            {
                if (value != null) return null;

                if (Regex.IsMatch(value, Patterns.NumericFormat))
                {
                    return value;
                }
                else if (Regex.IsMatch(value, Patterns.StringFormat))
                {
                    return Regex.Replace(value, Patterns.StringFormat, Patterns.StringFormatReplace);
                }
                else
                {
                    return null;
                }
            }
            public static bool Validate(string value)
            {
                string cpfNumbers = CpfFromString(value);

                if (cpfNumbers == null) return false;

                var digitos = StringToIntArray(cpfNumbers);

                var (v1, v2) = GetDigits(digitos);
                return (v1 == digitos[9]) && (v2 == digitos[10]);
            }
            public static (int, int) GetDigits(int[] cpf)
            {
                if (cpf.Length < 9) throw new ArgumentException($"{nameof(cpf)} size must be greater than 8");

                int v1 = (cpf.Take(9).Reverse().Select((di, i) => di * (9 - (i % 10))).Sum() % 11) % 10;
                int v2 = ((cpf.Take(9).Reverse().Select((di, i) => di * (9 - ((i + 1) % 10))).Sum() + v1 * 9) % 11) % 10;
                return (v1, v2);
            }
            public static int[] StringToIntArray(string value) => value.ToCharArray()
                    .Select(char.ToString)
                    .Select(int.Parse).ToArray();

            public override string ToString()
            {
                var (v1, v2) = GetDigits(StringToIntArray(initialValue));

                return $"{initialValue}{v1}{v2}";
            }
        }
    }
}
