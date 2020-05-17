using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtractCovid19Sp
{
    /// <summary>
    ///    Extrai dados do boletim diário da prefeitura de são paulo 
    /// </summary>
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class DataCovidSp
    {
        [FieldHidden]
        private readonly string textDailyData;

        [FieldHidden]
        private CultureInfo provider;

        public DataCovidSp()
        {

        }

        public DataCovidSp(string textDailyData)
        {
            this.provider = new CultureInfo("pt-BR");
            this.textDailyData = ReplaceAllSpaceWithSingleSpace(textDailyData);
            ParseData();

        }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date { get; set; }

        [FieldConverter(ConverterKind.Int32)]
        public int Suspects { get; set; }

        [FieldConverter(ConverterKind.Int32)]
        public int ConfirmedCases { get; set; }

        [FieldConverter(ConverterKind.Int32)]
        public int DeathsSivep { get; set; }

        [FieldConverter(ConverterKind.Int32)]
        public int DeathsProAim { get; set; }

        [FieldConverter(ConverterKind.Int32)]
        public int CtiUsage { get; set; }

        private void ParseData()
        {
            ParseDate();
            ParseFatalityTable();
            ParseCtiUsage();

        }

        private void ParseCtiUsage()
        {

            
            var match = SearchTextUsingRegEx("Taxa.+de.+Ocupação.+UTI\\s+(\\d+)", false);

            var text = 0.ToString();
            
            if (match.Groups.Count >= 2)
            {
                text = match.Groups[1].Value.Trim();
            }

            this.CtiUsage = ParseInt(text);

        }
        private void ParseFatalityTable()
        {
            var match = SearchTextUsingRegEx("Município\\s+de\\s+São Paulo([\\d/.\\s,]+)");

            if (match.Groups.Count < 2)
            {
                throw new InvalidOperationException("ParseFatalityTable group not found");
            }

            var text = match.Groups[1].Value.Trim();

            text = text.Replace("\n", " ");
            text = text.Replace("\r", " ");

            var tokens = text.Split(' ');

            tokens = tokens.Where(t => t.Trim() != "").ToArray();

            this.Suspects = ParseInt(tokens[0]);
            this.ConfirmedCases = ParseInt(tokens[1]);
            this.DeathsSivep = ParseInt(tokens[3]);
            this.DeathsProAim = ParseInt(tokens[5]);

        }

        private int ParseInt(string text)
        {
            var value = int.Parse(text.Replace(".", ""), this.provider);
            return value;
        }

        private void ParseDate()
        {
            var match = SearchTextUsingRegEx("\\d{2}\\s*/\\s*\\d{2}\\s*/\\s*\\d{4}");
            var text = match.Value;

            this.Date = DateTime.Parse(text, this.provider);
        }

        private string ReplaceAllSpaceWithSingleSpace(string text)
        {
            var regex = new Regex(
      "[\\s\\t\\n\\r]+",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.Singleline
    | RegexOptions.CultureInvariant
    | RegexOptions.Compiled
    );

            text = regex.Replace(text, " ");

            regex = new Regex(
           "[\\s]+",
         RegexOptions.IgnoreCase
         | RegexOptions.Multiline
         | RegexOptions.Singleline
         | RegexOptions.CultureInvariant
         | RegexOptions.Compiled
         );

            text = regex.Replace(text, " ");
            return text;
        }

        private Match SearchTextUsingRegEx(string pattern, bool allowNoMatch = false)
        {
            var regex = new Regex(
      pattern,
    RegexOptions.IgnoreCase
    | RegexOptions.Singleline
    | RegexOptions.CultureInvariant
    | RegexOptions.Compiled
    );

            var m = regex.Match(this.textDailyData);

            if (!m.Success)
            {
                if (!allowNoMatch)
                {
                    throw new InvalidOperationException($"date not found with pattern {pattern}");
                }
            }

            return m;
        }
    }
}
