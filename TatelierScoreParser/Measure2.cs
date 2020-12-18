using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{
	class Measure2Info
	{
		/// <summary>
		/// 1小節分のテキスト
		/// </summary>
		public StringBuilder OneLineText;

		public double BPM = 120;

		public double ScrollSpeed = 1;

		public double PivotTimeMillisec = 0;
		
		public double Beat = 4;
	}
	class Measure2
	{
		public MeasureType MeasureType = MeasureType.Normal;
		
		List<Note> NoteList = new List<Note>();

		public Measure2(Measure2Info info)
		{
			NoteList = new List<Note>();

			string oneLineText = $"{info.OneLineText}".Replace("\r", "");

			var sbSplit = oneLineText.Split('\n');

			int noteNum = 0;

			// #から始まる行はノートの数計算の対象外とする
			foreach (var item in sbSplit)
			{
				var str = item;
				if (str.Length > 0 && str[0] != '#')
				{
					noteNum += str.Length;
				}
			}

			var sharpLineSb = new StringBuilder();

			bool isSharpLine = false;

			foreach(var c in oneLineText)
			{
				if (isSharpLine)
				{
					switch(c)
					{
						case '\n':
							isSharpLine = false;
							sharpLineSb.Clear();
							break;
						default:
							sharpLineSb.Append(c);
							break;
					}
				}
				else
				{
					switch (c)
					{
						case '#':
							isSharpLine = true;
							break;
						case '0':
						case '5':
						case '6':
						case '7':
						case '8':
							info.PivotTimeMillisec += (60000 * info.Beat) / (info.BPM * (noteNum > 0 ? noteNum : 1));
							break;
						case '1':
						case '2':
						case '3':
						case '4':
							NoteList.Add(new Note((NoteType)c, info));
							info.PivotTimeMillisec += (60000 * info.Beat) / (info.BPM * (noteNum > 0 ? noteNum : 1));
							break;
					}
				}
			}
		}
	}
}
