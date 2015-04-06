using System;
using System.Collections.Generic;
using System.Diagnostics;

using EvernoteSDK;
using EvernoteSDK.Advanced;
using Evernote.EDAM.Type;

namespace EvernoteReadOnlyTags
{
    class Program
    {
        public const string READONLY_CONTENT_CLASS = @"jasonsherron.readonly";
        public const string READONLY_TAG = @"ReadOnly";

        static void Main(string[] args)
        {
            EvernoteConnection.Create();
            
            //SampleData.CreateSampleNote(true, READONLY_TAG);

            // lock newly-tagged notes
            SetTaggedNotesToReadOnly();

            // unset - let user remove tags to reset notes to r/w
            InteractiveRemoveReadOnlyTags();

            // sweep and reset to read-write
            ResetUntaggedNotesToReadWrite();
        }

        private static void InteractiveRemoveReadOnlyTags()
        {
            const string READONLY_TAG_SEARCH_QUERY = @"tag:" + READONLY_TAG;

            List<ENSessionFindNotesResult> readOnlyNotes = FindNotes(READONLY_TAG_SEARCH_QUERY);

            Console.WriteLine("all readonly notes: " + readOnlyNotes.Count);
            Debug.WriteLine("all readonly notes: " + readOnlyNotes.Count);

            foreach(ENSessionFindNotesResult readOnlyNote in readOnlyNotes)
            {
                Console.WriteLine(string.Format("Untag '{0}'?", readOnlyNote.Title));
                Console.Write("Y / N: ");

                if ("Y" == Console.ReadLine().ToUpper())
                {
                    ENNoteRef noteRef = readOnlyNote.NoteRef;
                    ENNoteStoreClient noteStore = ENSessionAdvanced.SharedSession.NoteStoreForNoteRef(noteRef);
                    List<string> tagGuids = noteStore.GetNoteTagNames(noteRef.Guid);
                    List<Tag> tags = noteStore.ListTags();
                    Tag readOnlyTag = tags.Find(x => x.Name.Contains(READONLY_TAG));
                    Note loadedNote = noteStore.GetNote(noteRef.Guid, true, false, false, false);

                    loadedNote.TagGuids.Remove(readOnlyTag.Guid);
                    noteStore.UpdateNote(loadedNote);
                }
            }
        }

        private static void SetTaggedNotesToReadOnly()
        {
            // find notes with readonly tag but no contentClass
            const string READONLY_TAG_NO_CONTENTCLASS_SEARCH = @"tag:" + READONLY_TAG + " -contentClass:" + READONLY_CONTENT_CLASS;

            List<ENSessionFindNotesResult> notesToSetReadOnly = FindNotes(READONLY_TAG_NO_CONTENTCLASS_SEARCH);

            Console.WriteLine("newly-tagged notes to mark readonly: " + notesToSetReadOnly.Count);
            Debug.WriteLine("newly-tagged notes to mark readonly: " + notesToSetReadOnly.Count);

            // add r/o contentClass to every found note (with r/o tag)
            foreach (ENSessionFindNotesResult noteToSetReadOnly in notesToSetReadOnly)
            {
                ENNoteRef noteRef = noteToSetReadOnly.NoteRef;
                ENNoteStoreClient noteStore = ENSessionAdvanced.SharedSession.NoteStoreForNoteRef(noteRef);
                Note loadedNote = noteStore.GetNote(noteRef.Guid, true, false, false, false);
                if (null == loadedNote.Attributes.ContentClass)
                {
                    loadedNote.Attributes.ContentClass = READONLY_CONTENT_CLASS;
                    noteStore.UpdateNote(loadedNote);
                }
            }
        }

        private static void ResetUntaggedNotesToReadWrite()
        {
            // find notes with the contentClass but no tag
            const string READONLY_NOTAG_CONTENT_CLASS_SEARCH = @"contentClass:" + READONLY_CONTENT_CLASS + " -tag:" + READONLY_TAG;

            List<ENSessionFindNotesResult> untaggedNotes = FindNotes(READONLY_NOTAG_CONTENT_CLASS_SEARCH);

            Console.WriteLine("untagged notes to reset to r/w: " + untaggedNotes.Count);
            Debug.WriteLine("untagged notes to reset to r/w: " + untaggedNotes.Count);

            // Clear the contentClass for anything without the ReadOnly tag
            foreach (ENSessionFindNotesResult noteToSetReadWrite in untaggedNotes)
            {
                ENNoteRef noteRef = noteToSetReadWrite.NoteRef;
                ENNoteStoreClient noteStore = ENSessionAdvanced.SharedSession.NoteStoreForNoteRef(noteRef);
                List<string> tags = noteStore.GetNoteTagNames(noteRef.Guid);
                Note loadedNote = noteStore.GetNote(noteRef.Guid, true, false, false, false);

                loadedNote.Attributes.ContentClass = null;
                noteStore.UpdateNote(loadedNote);
            }
        }

        private static List<ENSessionFindNotesResult> FindNotes(string evernoteSearchTerm)
        {
            List<ENSessionFindNotesResult> readOnlyNotes =
                EvernoteConnection.CurrentSession.FindNotes(ENNoteSearch.NoteSearch(evernoteSearchTerm),
                    null,
                    ENSession.SearchScope.All,
                    ENSession.SortOrder.Normal,
                    -1);
            return readOnlyNotes;
        }
    }
}
