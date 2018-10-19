using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    /// <summary>
    /// These enums will be needed when loading an Lilypond file.
    /// These are the types we currently support. It is not an exhausted list.
    /// </summary>
    public enum LilypondTokenKind
    {
        Unknown,
        Relative,
        RelativeValue,
        Note,
        Rest,
        Bar,
        Clef,
        ClefValue,
        Time,
        TimeValue,
        Tempo,
        TempoValue,
        Staff,
        Repeat,
        Alternative,
        SectionStart,
        SectionEnd
    }
}
