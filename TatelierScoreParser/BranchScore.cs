using System.Collections.Generic;
using System.Linq;

namespace Tatelier
{
	class BranchScoreInfo
	{

	}
	class BranchScore
	{
		public bool IsFixed = false;
		public int StartTime;
		public List<MeasureLine> Measures = new List<MeasureLine>();
		public List<Note> Notes = new List<Note>();

		SortedDictionary<int, List<List<Note>>> noteList;

		public Note[][][] NoteList;

		int nowLayer = 0;

		public void Build()
		{
			NoteList = noteList.Select(v => v.Value.Select(w => w.ToArray()).ToArray()).ToArray();
		}

		public void ChangeLayer(int layer)
		{
			nowLayer = layer;
			if (!noteList.ContainsKey(nowLayer))
			{
				noteList[nowLayer] = new List<List<Note>>();
			}
		}

		public void ChangeSection()
		{
			noteList[nowLayer].Add(new List<Note>());
		}

		public void AddNote(Note note)
		{
			Notes.Add(note);
			noteList[nowLayer][noteList[nowLayer].Count - 1].Add(note);
		}

		public void Reset()
		{
			IsFixed = false;
		}

		public BranchScore()
		{
			noteList = new SortedDictionary<int, List<List<Note>>>();
			noteList[nowLayer] = new List<List<Note>>();
			ChangeSection();
		}
	}
}