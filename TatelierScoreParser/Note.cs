using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{

	[DebuggerDisplay("{NoteType}, Time : {StartTimeMillisec}~{FinishTimeMillisec}")]
	class Note
	{
		/// <summary>
		/// 音符種別
		/// </summary>
		public NoteType NoteType = NoteType.Don;

		/// <summary>
		/// 音符文字種別
		/// </summary>
		public NoteTextType NoteTextType = NoteTextType.Do;

		/// <summary>
		/// 1つ前の音符
		/// </summary>
		public Note PrevNote = null;

		/// <summary>
		/// スクロールスピ－ド
		/// </summary>
		public float ScrollSpeed = 1.0F;

		double bpm = 120.0;

		/// <summary>
		/// BPM
		/// </summary>
		public double BPM
		{
			get => bpm;
			set
			{
				bpm = value;
				OneBarTime = (float)(240000 / bpm);
			}
		}
		
		/// <summary>
		/// 小節の時間
		/// </summary>
		public float OneBarTime { get; protected set; }

		/// <summary>
		/// 開始時間
		/// </summary>
		public int StartTimeMillisec;

		/// <summary>
		/// 終了時間
		/// </summary>
		public int FinishTimeMillisec;

		/// <summary>
		/// 描画開始時間
		/// </summary>
		public int StartDrawTimeMillisec = int.MinValue;

		/// <summary>
		/// 描画用の倍率
		/// </summary>
		public float MagForDraw;


        /// <summary>
        /// 描画開始時間関連を設定する
        /// </summary>
        /// <param name="areaWidth">描画エリア全体の幅</param>
        /// <param name="width">1小節分の幅</param>
        public void SetDrawTime(float areaWidth, float width)
        {
            SetDrawTime(areaWidth, width, 1.0F, ScrollSpeed);
        }

        /// <summary>
        /// 描画開始時間関連を設定する
        /// </summary>
        /// <param name="areaWidth">描画エリア全体の幅</param>
        /// <param name="width">1小節分の幅</param>
        /// <param name="settingScrSpd">設定部のスクロールスピード</param>
        public void SetDrawTime(float areaWidth, float width, float settingScrSpd)
		{
            SetDrawTime(areaWidth, width, settingScrSpd, ScrollSpeed);
		}

        /// <summary>
        /// 描画開始時間関連を設定する
        /// </summary>
        /// <param name="areaWidth">描画エリア全体の幅</param>
        /// <param name="width">1小節分の幅</param>
        /// <param name="settingScrSpd">設定部のスクロールスピード</param>
        /// <param name="settingScrSpd">音符部のスクロールスピード</param>
        public void SetDrawTime(float areaWidth, float width, float settingScrSpd, float noteScrSpd)
        {
            var scrspd = (noteScrSpd * settingScrSpd);
            var area = (areaWidth * scrspd);
            StartDrawTimeMillisec = (int)(StartTimeMillisec - width * OneBarTime / (area * scrspd));
            MagForDraw = area / OneBarTime;
        }

		/// <summary>
		/// 叩かれ状態
		/// / true : 叩かれ済, false : 未叩かれ
		/// </summary>
		public bool Hit = false;

		/// <summary>
		/// 描画するか否か
		/// / true : 描画する, false : しない
		/// </summary>
		public bool Visible = true;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="noteType"></param>
		/// <param name="info"></param>
		public Note(NoteType noteType, NoteInfo info)
		{
			NoteType = noteType;
			switch (NoteType)
			{
				case NoteType.Don:
				case NoteType.DonBig:
					NoteTextType = NoteTextType.Do;
					break;
				case NoteType.Kat:
				case NoteType.KatBig:
					NoteTextType = NoteTextType.Kat;
					break;
				case NoteType.Roll:
				case NoteType.RollBig:
					NoteTextType = NoteTextType.Renda;
					break;
				case NoteType.Balloon:
				case NoteType.Dull:
					NoteTextType = NoteTextType.GekiRenda;
					break;
				case NoteType.End:
					break;
			}
			StartTimeMillisec = (int)info.PivotTimeMillisec;
			FinishTimeMillisec = StartTimeMillisec;
			ScrollSpeed = (float)info.ScrollSpeed;
			BPM = info.BPM;

			// 終端音符の場合は、特殊音符を持っておく
			if (noteType == NoteType.End)
			{
				PrevNote = info.PrevNote;

				// 1つ前の音符の終了時間を設定する
				PrevNote.FinishTimeMillisec = StartTimeMillisec;
			}
		}

		[Obsolete]
		public Note(NoteType noteType, double startMillisec, double bpm, double scrollSpeed)
		{
			NoteType = noteType;
			StartTimeMillisec = (int)startMillisec;
			FinishTimeMillisec = StartTimeMillisec;
			ScrollSpeed = (float)scrollSpeed;
			OneBarTime = (float)(240000.0 / bpm);
		}
		
		[Obsolete]
		public Note(NoteType noteType, Measure2Info info)
		{
			NoteType = noteType;
			StartTimeMillisec = (int)(info.PivotTimeMillisec);
			FinishTimeMillisec = StartTimeMillisec;
			ScrollSpeed = (float)info.ScrollSpeed;
		}
	}
}
