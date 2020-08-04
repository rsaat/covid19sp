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
            this.Location = "sao paulo";
        }

        public DataCovidSp(string textDailyData)
        {
            this.provider = new CultureInfo("pt-BR");
            this.textDailyData = ReplaceAllSpaceWithSingleSpace(textDailyData);
            this.textDailyData = ReplaceDataForQuickFixesForSomeDates(this.textDailyData);
            this.Location = "sao paulo";
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

        [FieldNullValue(0)]
        [FieldOptional]
        [FieldConverter(ConverterKind.Int32)]
        public int Recovered { get; set; }

        [FieldNullValue("sao paulo")]
        [FieldOptional]
        public string Location { get; set; }

        private void ParseData()
        {
            ParseDate();
            ParseFatalityTable();
            ParseCtiUsage();

        }

        private void ParseCtiUsage()
        {

            var pattern = "Taxa.+de.+Ocupação.+UTI.+\\(.*somente.+municipais.*\\)\\s+(\\d+)";
            pattern = "Taxa.+de.+Ocupação.+UTI\\s+(\\d+)";
            var match = SearchTextUsingRegEx(pattern, false);

            var text = 0.ToString();

            if (match.Groups.Count >= 2)
            {
                text = match.Groups[1].Value.Trim();
            }

            this.CtiUsage = ParseInt(text);

        }
        private void ParseFatalityTable()
        {
            var match = SearchTextUsingRegEx("Município\\s+de\\s+São P.{0,1}aulo([\\d/.\\s,\\*\\%x\\–]+)");

            if (match.Groups.Count < 2)
            {
                throw new InvalidOperationException("ParseFatalityTable group not found");
            }

            var text = match.Groups[1].Value.Trim();

            text = text.Replace("\n", " ");
            text = text.Replace("\r", " ");
            text = text.Replace("*", " ");
            text = text.Replace("x", "");
            text = text.Replace("–", "");
            text = text.Replace("%", " ");

            var tokens = text.Split(' ');

            tokens = tokens.Where(t => t.Trim() != "").ToArray();

            this.Suspects = ParseInt(tokens[0]);
            this.ConfirmedCases = ParseInt(tokens[1]);
            if (tokens.Count() == 4)
            {
                this.DeathsSivep = ParseInt(tokens[2]);
            }
            else
            {
                this.DeathsSivep = ParseInt(tokens[3]);
            }

            this.DeathsProAim = this.DeathsSivep; //   ParseInt(tokens[5]);

        }

        private int ParseInt(string text)
        {
            var value = int.Parse(text.Replace(".", ""), this.provider);
            return value;
        }

        private void ParseDate()
        {
            var match = SearchTextUsingRegEx("\\d\\s*\\d\\s*/\\d\\s*\\d\\s*/\\s*\\d{4}");
            var text = match.Value.Replace(" ", "");

            this.Date = DateTime.Parse(text, this.provider);
        }

        private string ReplaceDataForQuickFixesForSomeDates(string text)
        {
            text = text.Replace("Méunicípio de São Paéuélo", "Município de São Paulo");
            text = text.Replace("Móunicípio de São Paóuólo", "Município de São Paulo");
            text = text.Replace("MLunicípio de São PaLuLlo", "Município de São Paulo");
            text = text.Replace("Municípioé deé São Paulo", "Município de São Paulo");
            return text;
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
