using System.Collections.Generic;
using TN.TNM.Common.CommonObject;

namespace TN.TNM.Common.Helper
{
    public static class StringHelper
    {
        public static List<NoteObject> ConvertNoteToObject(string note)
        {
            if (string.IsNullOrEmpty(note))
            {
                return null;
            }
            var noteObjects = new List<NoteObject>();
            var notes = note.Split('~');
            foreach (string t in notes)
            {
                if (!string.IsNullOrEmpty(t))
                {
                    var noteElements = t.Split('|');
                    NoteObject noteObject = new NoteObject
                    {
                        Title = noteElements[0],
                        Actor = noteElements[1],
                        SubTitle = noteElements[2],
                        Content = noteElements[3]
                    };
                    noteObjects.Add(noteObject);
                }
            }
            return noteObjects;
        }

    }
}
