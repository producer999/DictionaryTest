using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryTest
{
    public enum PartsOfSpeech
    {
        NounM,
        NounF,
        VerbT,
        VerbNT,
        Adjective,
        Adverb,
        Postposition,
        Pronoun,
        Conjunction,
        Interjections,
        CaseMarker,
        Other
    }


    public class Definition
    {
        [SQLite.Net.Attributes.PrimaryKey, SQLite.Net.Attributes.AutoIncrement]
        public int id { get; set; }
        public string term { get; set; }
        public string definition { get; set; }
        public string googleDefinition { get; set; }
        public string importedDefinition { get; set; }
        public string altDefinitions { get; set; }
        public string altForms { get; set; }
        public string example { get; set; }
        public PartsOfSpeech pos { get; set; }



        public Definition()
        {
            
        }

        public Definition(string newTerm, string newDefinition, string newGoogleDef)
        {
            term = newTerm;
            definition = newDefinition;
            googleDefinition = newGoogleDef;
        }

        public Definition(string newTerm, string newDefinition)
        {
            term = newTerm;
            definition = newDefinition;
        }

        public Definition(string newTerm)
        {
            term = newTerm;
            definition = "";

        }
    }
}
