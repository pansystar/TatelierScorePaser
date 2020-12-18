using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{
    /// <summary>
    /// 音符読込設定用情報
    /// </summary>
	class NoteInfo
	{
        /// <summary>
        /// 小節線の表示／非表示
        /// </summary>
		public bool IsVisibleBarLine = true;

        /// <summary>
        /// 1つ前のBPM
        /// </summary>
		public double PrevBPM;

        /// <summary>
        /// BPM
        /// </summary>
		public double BPM;

        /// <summary>
        /// 小節
        /// </summary>
		public double Measure = 4;

        /// <summary>
        /// スクロールスピード
        /// </summary>
		public double ScrollSpeed = 1.0;

        /// <summary>
        /// 音符の時間(ms)
        /// </summary>
		public double PivotTimeMillisec;

        /// <summary>
        /// 現在の小節の開始時間(ms)
        /// </summary>
		public double MeasureStartTimeMillisec;

        /// <summary>
        /// 分岐種別
        /// </summary>
		public BranchType BranchType = BranchType.Common;

        /// <summary>
        /// 1つ前の音符
        /// </summary>
		public Note PrevNote;

        /// <summary>
        /// 
        /// </summary>
		public Note BranchPrevNote;

        /// <summary>
        /// 1つ前の小節線
        /// </summary>
		public MeasureLine PrevMeasure;

		public BranchPlayInfo PrevBranchPlayInfo;

		public BranchScore BranchScore;
		
        /// <summary>
        /// 章リスト ※章の切り替え時間(ms)のリスト
        /// </summary>
		public List<int> SectionList = new List<int>();

		public ScoreInfo ScoreInfo;

		public int NowBalloonIndex = 0;

		/// <summary>
		/// 分岐開始時間(ms)
		/// </summary>
		public double BranchStartTime = 0;

        /// <summary>
        /// 分岐終了時間(ms)
        /// </summary>
        public double BranchEndTime = 0;

	}
}
