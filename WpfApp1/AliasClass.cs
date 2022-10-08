using System.Collections.Generic;

namespace STFC_EventLogger
{
    public class AliasClass
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public string Name { get; set; }
        public List<string> AKA { get; set; }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        public override string ToString()
        {
            return $"{Name}, Count: {AKA.Count}";
        }
    }
}

