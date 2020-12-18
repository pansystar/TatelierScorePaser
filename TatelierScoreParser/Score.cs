using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tatelier
{
    /// <summary>
    /// 譜面情報
    /// </summary>
	class ScoreInfo
	{
        /// <summary>
        /// コース名
        /// </summary>
		public string CourseName;

		public double StartBPM;

		public double Offset;

		public int[] balloonCountList;
	}

	[DebuggerDisplay("CourseName : {CourseName}")]
	class Score
	{
		/// <summary>
		/// 1分のミリ秒表記
		/// </summary>
		public const int OneMinuteInMillisec = 60000;

		/// <summary>
		/// 初期BPM
		/// </summary>
		public double StartBPM = 200;

		/// <summary>
		/// 音源再生Offset
		/// </summary>
		public int OffsetMillisec = 0;

		/// <summary>
		/// コース名
		/// </summary>
		public string CourseName = "";

		/// <summary>
		/// 
		/// </summary>
		public int ScoreInitPoint = 100;

		/// <summary>
		/// 
		/// </summary>
		public int ScoreDiffPoint = 100;

		/// <summary>
		/// すべての音符
		/// </summary>
		public List<Note> Notes = new List<Note>();

		/// <summary>
		/// すべての小節
		/// </summary>
		public List<MeasureLine> Measures = new List<MeasureLine>();

		/// <summary>
		/// BPM情報リスト
		/// </summary>
		public List<BPMInfo> BPMInfoList = new List<BPMInfo>();

		/// <summary>
		/// 分岐毎の譜面管理
		/// </summary>
		public BranchScoreControl2 BranchScoreControl2 = new BranchScoreControl2();

		/// <summary>
		/// 風船管理情報
		/// </summary>
		public BalloonControlInfo BalloonControlInfo = new BalloonControlInfo();

		/// <summary>
		/// 分岐演奏情報リスト
		/// </summary>
		public List<BranchPlayInfo> BranchPlayInfoList = new List<BranchPlayInfo>();

		/// <summary>
		/// 時間ごと章リスト
		/// </summary>
		public List<int> SectionList = new List<int>();

		/// <summary>
		/// ゴーゴーリスト
		/// </summary>
		public List<(bool, int)> GogoList = new List<(bool, int)>();

		/// <summary>
		/// 分岐有無
		/// </summary>
		public bool HasBranch = false;

		/// <summary>
		/// 音符の最大数を取得する。
		/// ※ドンとカツのみで計算
		/// </summary>
		public int MaxNoteCount
		{
			get
			{
				int sum = 0;

				foreach(var item in BranchScoreControl2.GetBranchScoreList())
				{
					sum += item.Notes.Sum(v =>
					{
						switch (v.NoteType)
						{
							case NoteType.Don:
							case NoteType.Kat:
							case NoteType.DonBig:
							case NoteType.KatBig:
								return 1;
							default:
								return 0;
						}
					});
				}

				return sum;
			}
		}

		#region プライベートメソッド

		#region 小節線
		int SetBARLINEOFF(NoteInfo info, string[] args)
		{
			info.IsVisibleBarLine = false;
			return 0;
		}
		int SetBARLINEON(NoteInfo info, string[] args)
		{
			info.IsVisibleBarLine = true;
			return 0;
		}
		#endregion

		#region GOGO
		int SetGOGOSTART(NoteInfo info, string[] args)
		{
			GogoList.Add((true, (int)info.PivotTimeMillisec));
			return 0;
		}
		int SetGOGOEND(NoteInfo info, string[] args)
		{
			GogoList.Add((false, (int)info.PivotTimeMillisec));
			return 0;
		}
		#endregion

		int SetBPMCHANGE(NoteInfo info, string[] args)
		{
			if (args.Length > 0)
			{
				info.BPM = double.Parse(args[0]);
				BPMInfoList.Last().SetEndTime(info.PivotTimeMillisec);
				BPMInfoList.Add(new BPMInfo(info.PivotTimeMillisec, info.BPM));

				return 0;
			}
			else
			{
				return -1;
			}
		}
		int SetMEASURE(NoteInfo info, string[] args)
		{
			if (args.Length == 1)
			{
				var split = args[0].Split('/');
				info.Measure = double.Parse(split[0]) * 4 / double.Parse(split[1]);

				return 0;
			}
			else
			{
				return -1;
			}
		}		
		int SetSCROLL(NoteInfo info, string[] args)
		{
			if (args.Length > 0)
			{
				info.ScrollSpeed = double.Parse(args[0]);
				return 0;
			}
			else
			{
				return -1;
			}

		}

		#region 分岐情報
		int SetBRANCHSTART(NoteInfo info, string[] args)
		{
			var playInfo = new BranchPlayInfo(info);

			args[0] = string.Join("", args);

			var split = args[0].Split(',');

			for (int i = 0; i < split.Length; i++)
			{
				split[i] = split[i].Replace(" ", "");
			}

			playInfo.Type = split[0][0];

			playInfo.ExpertValue = float.Parse(split[1]);
			playInfo.MasterValue = float.Parse(split[2]);

			BranchPlayInfoList.Add(playInfo);

			info.PrevBranchPlayInfo = playInfo;
			info.BranchStartTime = info.PivotTimeMillisec;
			info.BranchEndTime = info.PivotTimeMillisec;
			info.BranchPrevNote = info.PrevNote;

			BranchScoreControl2.OneBeforeMeasureTime.Add((int)(info.PivotTimeMillisec - (OneMinuteInMillisec * 4 / info.PrevBPM)));

			HasBranch = true;

			BranchScoreControl2.KeyList.Add((int)info.PivotTimeMillisec);
			BranchScoreControl2.BranchTypeList[(int)info.PivotTimeMillisec] = BranchType.Normal;

			return 0;
		}
		int SetBRANCHEND(NoteInfo info, string[] args)
		{
			info.BranchStartTime = 0;
			info.PivotTimeMillisec = info.BranchEndTime;
			info.BranchType = BranchType.Common;
			info.BranchScore = BranchScoreControl2.CommonScore;
			return 0;
		}
		int SetSECTION(NoteInfo info, string[] args)
		{
			SectionList.Add((int)info.PivotTimeMillisec);

			return 0;
		}
		int SetBranchScore(NoteInfo info, string[] args, BranchType type)
		{
			if (info.BranchEndTime == info.BranchStartTime)
			{
				info.BranchEndTime = info.PivotTimeMillisec;
			}
			info.PrevNote = info.BranchPrevNote;
			info.PivotTimeMillisec = info.BranchStartTime;
			info.BranchType = type;
			info.BranchScore = new BranchScore();
			BranchScoreControl2.ExpertScoreList[(int)info.PivotTimeMillisec] = info.BranchScore;

			return 0;
		}
		int SetN(NoteInfo info, string[] args)
		{
			return SetBranchScore(info, args, BranchType.Normal);
		}
		int SetE(NoteInfo info, string[] args)
		{
			return SetBranchScore(info, args, BranchType.Expert);
		}
		int SetM(NoteInfo info, string[] args)
		{
			return SetBranchScore(info, args, BranchType.Master);
		}
		#endregion

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="textSB"></param>
		/// <param name="info"></param>
		public Score(StringBuilder textSB, ScoreInfo info = null)
		{			
			if (info == null)
			{
				info = new ScoreInfo();
			}

			CourseName = info.CourseName;
			StartBPM = info.StartBPM;			
			OffsetMillisec = (int)(info.Offset * 1000);
			
			var noteInfo = new NoteInfo()
			{
				BPM = StartBPM,
				PivotTimeMillisec = 0, // -info.Offset * 1000,
				Measure = 4,
				PrevNote = null,
				ScoreInfo = info,
			};

			BPMInfoList.Add(new BPMInfo(int.MinValue, noteInfo.BPM));
			noteInfo.BranchScore = BranchScoreControl2.CommonScore;

			var sharpMethodMap = new Dictionary<string, Func<NoteInfo, string[], int>>()
			{
				{ "MEASURE", SetMEASURE },
				{ "BPMCHANGE", SetBPMCHANGE },
				{ "SCROLL", SetSCROLL },
				{ "BARLINEOFF", SetBARLINEOFF },
				{ "BARLINEON", SetBARLINEON },

				// ゴーゴー関連
				{ "GOGOSTART", SetGOGOSTART },
				{ "GOGOEND", SetGOGOEND },

				// 分岐関係
				{ "BRANCHSTART",  SetBRANCHSTART },
				{ "BRANCHEND",  SetBRANCHEND  },
				{ "SECTION", SetSECTION },
				{ "N",  SetN  },
				{ "E",  SetE  },
				{ "M",  SetM  },
				//{ "SECTION", NoteInfo.TryBARLINEON },
				//{ "LEVELHOLD", NoteInfo.TryBARLINEON }
			};

			var measureSB = new StringBuilder();
			bool isIgnore = false; // 文字無視フラグ
			bool isSharpLine = false;

			for (int i = 0; i < textSB.Length; i++)
			{
				var c = textSB[i];

				if (isIgnore)
				{
					// 文字無視フラグは改行文字で折る
					if (c == '\n') isIgnore = false;
					continue;
				}

				switch (c)
				{
					case '/':
						// 次の文字も'/'のときはコメントみなして、
						// 改行文字が来るまで以降の文字を無視する
						if (i + 1 < textSB.Length && textSB[i + 1] == '/')
						{
							isIgnore = true;
							i++;
						}
						else
						{
							measureSB.Append(c);
						}
						break;
					case '#':
						isSharpLine = true;
						measureSB.Append(c);
						break;
					case ' ':
						if (isSharpLine)
						{
							measureSB.Append(c);
						}
						break;
					case '\r': // 不必要な文字のため無視
					case '\t': // 不必要な文字のため無視
						break;
					case '\n':
						{
							// 文字無視フラグは改行文字で折る
							isIgnore = false;
							isSharpLine = false;
							measureSB.Append(c);
						}
						break;
					case ',':
						// 1小節取得完了
						{
							if (isSharpLine)
							{
								measureSB.Append(c);
							}
							else
							{
								TryCreateNoteListFromMeasure(noteInfo, sharpMethodMap, measureSB, out var notes, out var measures);
								measureSB.Clear();
								Measures.AddRange(measures);
								Notes.AddRange(notes);
							}
						}
						break;
					default:
						{
							measureSB.Append(c);
						}
						break;
				}
			}


			BranchScoreControl2.Build();
		}

		/// <summary>
		/// 描画時間を再設定する
		/// </summary>
		/// <param name="areaWidth">描画幅</param>
		/// <param name="width">1小節の幅</param>
		public void SetDrawTime(float areaWidth, float width, float settingScrollSpeed)
		{
			// 音符の設定
			foreach (var note in Notes)
			{
				note.SetDrawTime(areaWidth, width, settingScrollSpeed);
			}

			// 小節線の設定
			foreach (var measure in Measures)
			{
				measure.SetDrawTime(areaWidth, width, settingScrollSpeed);
			}
		}

        /// <summary>
        /// 描画時間を再設定する
        /// </summary>
        /// <param name="areaWidth">描画幅</param>
        /// <param name="width">1小節の幅</param>
        public void SetDrawTime(float areaWidth, float width, float settingScrollSpeed, float itemScrollSpeed = 1.0F)
        {
            // 音符の設定
            foreach (var note in Notes)
            {
                note.SetDrawTime(areaWidth, width, settingScrollSpeed, itemScrollSpeed);
            }

            // 小節線の設定
            foreach (var measure in Measures)
            {
                measure.SetDrawTime(areaWidth, width, settingScrollSpeed, itemScrollSpeed);
            }
        }

        /// <summary>
        /// 1小節分のデータを作る
        /// </summary>
        /// <param name="noteInfo"></param>
        /// <param name="sharpMethodMap"></param>
        /// <param name="measureSB"></param>
        /// <returns></returns>
        bool TryCreateNoteListFromMeasure(
			NoteInfo noteInfo,
			Dictionary<string, Func<NoteInfo, string[], int>> sharpMethodMap,
			StringBuilder measureSB,
			out IEnumerable<Note> notes,
			out IEnumerable<MeasureLine> measureLines)
		{

			var sbSplit = $"{measureSB}".Split('\n');

			int noteNum = 0;

			// #から始まる行はノートの数計算の対象外とする
			foreach (var item in sbSplit)
			{
				var str = item;
				if (str.Length > 0 && str[0] != '#')
				{
					noteNum += str.Where(v => v != ' ').Count();
				}
			}

			var noteList = new List<Note>();
			var measureList = new MeasureLine[1];

			bool isIgnore = false;

			var sharpLine = new StringBuilder();
			noteInfo.MeasureStartTimeMillisec = noteInfo.PivotTimeMillisec;

			for (int i = 0; i < measureSB.Length; i++)
			{
				switch (measureSB[i])
				{
					case '#':
						isIgnore = true;
						break;
					case '\n':
						isIgnore = false;
						{
							var sbSharpOnly = new StringBuilder();
							string[] args = new string[0];
							int sharpIdx;
							for(sharpIdx = 0; sharpIdx < sharpLine.Length; sharpIdx++)
							{
								// 大文字は命令文として取得
								if (char.IsUpper(sharpLine[sharpIdx]))
								{
									sbSharpOnly.Append(sharpLine[sharpIdx]);
								}
								else
								{
									break;
								}
							}
							if (sharpIdx < sharpLine.Length)
							{
								args = sharpLine.ToString(sharpIdx, sharpLine.Length - sharpIdx).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
							}
							if (sbSharpOnly.Length > 0)
							{
								if (sharpMethodMap.TryGetValue($"{sbSharpOnly}", out var func))
								{
									func(noteInfo, args);
								}
								else
								{
									// メソッドなし
								}
							}
							sharpLine.Clear();
						}
						break;
					default:
						if (isIgnore)
						{
							sharpLine.Append(measureSB[i]);

							continue;
						}
						switch (measureSB[i])
						{
							case '0':
								if (measureList[0] == null)
								{
									var measure = new MeasureLine(MeasureType.Normal, noteInfo);
									noteInfo.BranchScore.Measures.Add(measure);
									measureList[0] = measure;
								}
								break;
							case '1':
							case '2':
							case '3':
							case '4':
								{
									var note = new Note((NoteType)measureSB[i], noteInfo);
									noteInfo.BranchScore.AddNote(note);
									noteList.Add(note);
									if (measureList[0] == null)
									{
										var measure = new MeasureLine(MeasureType.Normal, noteInfo);
										noteInfo.BranchScore.Measures.Add(measure);
										measureList[0] = measure;
									}
									noteInfo.PrevNote = note;
								}
								break;
							case '5':
							case '6':
							case '7':
								{
									var note = new Note((NoteType)measureSB[i], noteInfo);
									noteInfo.BranchScore.AddNote(note);
									noteList.Add(note);
									if (measureList[0] == null)
									{
										var measure = new MeasureLine(MeasureType.Normal, noteInfo);
										noteInfo.BranchScore.Measures.Add(measure);
										measureList[0] = measure;
									}
									if(noteInfo.PrevNote?.NoteType != note.NoteType)
									{
										noteInfo.PrevNote = note;
									}

									if (measureSB[i] == '7')
									{
										int cnt = 5;
										if (noteInfo.NowBalloonIndex < noteInfo.ScoreInfo.balloonCountList.Length)
										{
											cnt = noteInfo.ScoreInfo.balloonCountList[noteInfo.NowBalloonIndex];
											noteInfo.NowBalloonIndex++;
										}
										BalloonControlInfo.Add((int)noteInfo.PivotTimeMillisec, cnt, noteInfo.BranchType);
									}
								}
								break;
							case '8':
								{
									var note = new Note((NoteType)measureSB[i], noteInfo);
									noteInfo.BranchScore.AddNote(note);

									noteList.Add(note);
									if (measureList[0] == null)
									{
										var measure = new MeasureLine(MeasureType.Normal, noteInfo);
										noteInfo.BranchScore.Measures.Add(measure);
										measureList[0] = measure;
									}
									noteInfo.PrevNote = note;
								}
								break;
						}
						noteInfo.PivotTimeMillisec += (OneMinuteInMillisec * noteInfo.Measure) / (noteInfo.BPM * (noteNum > 0 ? noteNum : 1));

						break;
				}
			}



			// ノート数が0のときは空小節とみなして1小節分の時間を追加する
			if (noteNum == 0)
			{
				noteInfo.PivotTimeMillisec += (OneMinuteInMillisec * noteInfo.Measure) / noteInfo.BPM;
			}
			if (measureList[0] == null)
			{
				var measure = new MeasureLine(MeasureType.Normal, noteInfo);
				measureList[0] = measure;
			}
			notes = noteList;
			noteInfo.PrevMeasure = measureList[0];
			measureLines = measureList;

			// 現状のBPMを一旦保持する
			noteInfo.PrevBPM = noteInfo.BPM;


			return true;
		}
	}
}
