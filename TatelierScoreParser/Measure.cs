using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{
	class MeasureLine
	{
		List<Note> Notes = new List<Note>();
		MeasureType MeasureType = MeasureType.Normal;

		public bool Visible = true;

		public float ScrollSpeed = 1.0F;

		public int StartTimeMillisec;
		
		public int StartDrawTimeMillisec = int.MinValue;

		public float MagForDraw;

		public float OneBarTime;

        public void SetDrawTime(float areaWidth, float width, float settingScrollSpeed)
        {
            SetDrawTime(areaWidth, width, settingScrollSpeed, ScrollSpeed);
        }

        public void SetDrawTime(float areaWidth, float width, float settingScrollSpeed, float itemScrollSpeed)
        {
            var scrspd = ScrollSpeed * settingScrollSpeed;
            var area = areaWidth * scrspd;

            StartDrawTimeMillisec = (int)(StartTimeMillisec - width * OneBarTime / (area * scrspd));
            MagForDraw = area / OneBarTime;
		}

		public MeasureLine(MeasureType measureType, NoteInfo info)
		{
			MeasureType = measureType;
			Visible = info.IsVisibleBarLine;
			StartTimeMillisec = (int)info.MeasureStartTimeMillisec;
			ScrollSpeed = (float)info.ScrollSpeed;
			OneBarTime = (float)(240000.0 / info.BPM);
		}

		[Obsolete]
		public MeasureLine(MeasureType measureType, bool isVisible, double startMillisec, double bpm, double scrollSpeed)
		{
			MeasureType = measureType;
			Visible = isVisible;
			StartTimeMillisec = (int)startMillisec;
			ScrollSpeed = (float)scrollSpeed;
			OneBarTime = (float)(240000.0 / bpm);
		}
	}
}
