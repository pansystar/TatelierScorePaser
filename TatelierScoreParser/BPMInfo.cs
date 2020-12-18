using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{
	[DebuggerDisplay("BPM : {BPM}, [{StartTime} ～ {EndTime}]")]
	class BPMInfo
	{
		public int StartTime;
		public int EndTime = int.MaxValue;

		public double BPM;

		public void SetEndTime(double endTime)
		{
			EndTime = (int)endTime;
		}

		public BPMInfo(double startTime, double bpm)
		{
			this.StartTime = (int)startTime;
			this.BPM = bpm;
		}
	}
}
