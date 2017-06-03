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

        public Definition(string importedTerm, string importedDef, string pos, string example)
        {
            term = importedTerm;
            importedDefinition = importedDef;
            this.example = example;
            
            switch(pos)
            {
                case "N":
                    this.pos = PartsOfSpeech.NounF;
                    break;
                case "V":
                    this.pos = PartsOfSpeech.VerbNT;
                    break;
                case "ADV":
                    this.pos = PartsOfSpeech.Adverb;
                    break;
                case "ADJ":
                    this.pos = PartsOfSpeech.Adjective;
                    break;
                case "PRON":
                    this.pos = PartsOfSpeech.Pronoun;
                    break;
                case "CONJ":
                    this.pos = PartsOfSpeech.Conjunction;
                    break;
                case "INTJ":
                    this.pos = PartsOfSpeech.Interjections;
                    break;
                case "POSTPOSTN":
                    this.pos = PartsOfSpeech.Postposition;
                    break;
                case "CASE-MARK":
                    this.pos = PartsOfSpeech.CaseMarker;
                    break;
                default:
                    this.pos = PartsOfSpeech.Other;
                    break;
            }
           
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
