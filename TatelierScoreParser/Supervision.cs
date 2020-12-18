using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;

namespace Tatelier
{
	class Supervision
	{
		#region スタティック変数部
		public static int ScreenWidth = 1920;
		public static int ScreenHeight = 1080;

		public static readonly int ScreenWidthHalf = 960;
		public static readonly int ScreenHeightHalf = 540;

		static Supervision Singleton;

		/// <summary>
		/// カウンタ(ms)
		/// </summary>
		public static int NowMilliSec { get; private set; }

		/// <summary>
		/// カウンタ(μs)
		/// </summary>
		public static long NowMicroSec { get; private set; }
		#endregion


		string Title = "";

		Score[] scores;

		int nowTime;

		int diffTime;

		Song song;

		NoteImageControl noteImageControl;

		float speed = 1.0F;

		float left = 498;

		float judgeX = 0;

		/// <summary>
		/// 絶対に完璧に演奏するオート処理
		/// </summary>
		void UpdateSpecialAuto()
		{
			//bool don = false;
			//bool kat = false;
			//JudgeType judgeType = JudgeType.None;

			//int diffTime = 0;
			//Player player = Players[playerIndex]; //structなので注意!

			//bool rrr = player.BranchScoreControl2.TryChangeDeadline(nowTime);

			//DebugWindow.LeftText[4] = $"{Players[playerIndex].BranchCondition.GetBranchType(nowTime)}";
			int diffTime = 0;

			// 分岐譜面毎
			foreach (var bscore in scores[0].BranchScoreControl2.GetBranchScoreList())
			{
				// レイヤー層
				foreach (var noteList1 in bscore.NoteList)
				{
					// セクション層
					foreach (var noteList2 in noteList1)
					{
						// 各音符
						foreach (var item in noteList2)
						{
							// すでにヒットしている場合は次に進む
							if (item.Hit) continue;

							diffTime = item.StartTimeMillisec - nowTime;

							if (item.NoteType != NoteType.End && diffTime > 100) break;

							if (diffTime <= 0)
							{
								switch (item.NoteType)
								{
									case NoteType.Don:
									case NoteType.DonBig:
										//player.TaikoSEControl.Play(TaikoSEType.Don);
										//don = true;
										//item.Hit = true;
										//item.Visible = false;
										//Players[playerIndex].JudgeType = JudgeType.Great;
										//player.NoteFlyEffect.Fly(item.NoteType, nowTime);
										//player.ResultData.AddCount(JudgeType.Great);
										//judgeType = JudgeType.Great;
										//player.SoulGauge.Add(JudgeType.Great);
										//player.HitImageControl.Set(nowTime, item.NoteType, JudgeType.Great);
										break;
									case NoteType.Kat:
									case NoteType.KatBig:
										//player.TaikoSEControl.Play(TaikoSEType.Kat);
										//kat = true;
										//item.Hit = true;
										//item.Visible = false;
										//Players[playerIndex].JudgeType = JudgeType.Great;
										//player.NoteFlyEffect.Fly(item.NoteType, nowTime);
										//player.ResultData.AddCount(JudgeType.Great);
										//judgeType = JudgeType.Great;
										//player.SoulGauge.Add(JudgeType.Great);
										//player.HitImageControl.Set(nowTime, item.NoteType, JudgeType.Great);
										break;
									case NoteType.Roll:
										//AutoRoll.MoveNext();
										//if (AutoRoll.Current)
										//{
										//	player.NoteFlyEffect.Fly(NoteType.Don, nowTime);
										//	player.ResultData.AddRollCount();
										//	player.BranchCondition.AddRollCount();
										//	player.TaikoSEControl.Play(TaikoSEType.Don);
										//}
										break;
									case NoteType.RollBig:
										//AutoRoll.MoveNext();
										//if (AutoRoll.Current)
										//{
										//	player.NoteFlyEffect.Fly(NoteType.DonBig, nowTime);
										//	player.ResultData.AddRollCount();
										//	player.BranchCondition.AddRollCount();
										//	player.TaikoSEControl.Play(TaikoSEType.Don);
										//}
										break;
									case NoteType.Balloon:
										//AutoRoll.MoveNext();
										//if (AutoRoll.Current)
										//{
										//	player.TaikoSEControl.Play(TaikoSEType.Don);
										//}
										break;
									case NoteType.End:
										//item.PrevNote.Hit = true;
										//item.Hit = true;
										//if (item.PrevNote.NoteType == NoteType.Balloon)
										//{
										//	item.PrevNote.Visible = false;
										//	PlaySoundMem(balloon, DX_PLAYTYPE_BACK);
										//	player.NoteFlyEffect.Fly(NoteType.DonBig, nowTime);
										//}
										break;
								}

								//player.JudgeImageControl.Update(judgeType, nowTime);
							}
						}
					}
				}
			}

			//player.ComboNumberImageControl.Combo = player.ResultData.Combo;
			//player.ScoreNumberImageControl.ScorePoint = player.ResultData.ScorePoint;
			//background.Change(player.SoulGauge.NowStatus);
			//player.NoteFieldControl.Update(don, kat, false, nowTime);
		}

		/// <summary>
		/// 描画：音符
		/// </summary>
		void DrawNote()
		{
			float y = 120;

			foreach (var bscore in scores[0].BranchScoreControl2.GetBranchScoreList())
			{
				// レイヤー層
				foreach (var noteList1 in bscore.NoteList)
				{
					// セクション層(後ろから)
					foreach (var noteList2 in noteList1.Reverse<Note[]>())
					{
						// 音符層
						foreach (var item in noteList2.Reverse<Note>())
						{
							int t = (item.StartTimeMillisec - nowTime);
							if (item.Visible)
							{
								switch (item.NoteType)
								{
									case NoteType.Don:
									case NoteType.Kat:
									case NoteType.DonBig:
									case NoteType.KatBig:
									case NoteType.Roll:
									case NoteType.RollBig:
										if (item.NoteType != NoteType.End
											&& item.StartDrawTimeMillisec < nowTime)
										{
											int handle = noteImageControl.GetImageHandle(item.NoteType);
											GetGraphSize(handle, out int w, out int h);
											float hHalf = h / 2;
											float x = left + judgeX + (t * item.MagForDraw) * speed;
											DrawRotaGraphFastF(x, y, 1.0F, 0.0F, handle, TRUE);
										}
										break;
								}

								switch (item.NoteType)
								{
									case NoteType.Balloon:
										{
											float x;
											int handle = noteImageControl.GetImageHandle(item.NoteType);
											GetGraphSize(handle, out int w, out int h);
											float hHalf = h / 2;
											if (t > 0)
											{
												x = left + judgeX + (t * item.MagForDraw) * speed;
											}
											else
											{
												var finishTime = (item.FinishTimeMillisec - nowTime);
												x = finishTime < 0 ? left + judgeX + (finishTime * item.MagForDraw) : left + judgeX;
											}

											DrawRotaGraphFastF(x, y, 1.0F, 0.0F, handle, TRUE);
										}
										break;
									case NoteType.End:
										{
											// 前回音符によって処理を変える
											switch (item.PrevNote.NoteType)
											{
												case NoteType.Roll:
												case NoteType.RollBig:
													{
														int handle = noteImageControl.GetContentNoteImageHandle(item.PrevNote.NoteType);
														GetGraphSize(handle, out int w, out int h);
														float hHalf = h / 2;
														float x = left + judgeX + (t * item.MagForDraw) * speed;
														int prevT = (item.PrevNote.StartTimeMillisec - nowTime);
														float prevX = left + judgeX + (prevT * item.PrevNote.MagForDraw);

														DrawModiGraphF(prevX - 1, y - hHalf, x + 1, y - hHalf, x + 1, y + hHalf, prevX - 1, y + hHalf, handle, TRUE);
														DrawRotaGraphFastF(x, y, 1.0F, 0.0F, noteImageControl.GetEndNoteImageHandle(item.PrevNote.NoteType), TRUE);
													}
													break;
											}
										}
										break;
								}
							}
						}
					}
				}
			}
		}


		void MainRun(string[] args)
		{
			Title = $"TatelierScoreParser";

			SetMainWindowText(Title);

			ChangeWindowMode(1);
			SetAlwaysRunFlag(1);

			SetWindowStyleMode(7);
			SetWindowSizeChangeEnableFlag(1);
			ChangeFontType(DX_FONTTYPE_ANTIALIASING_4X4);
			SetFontSize(48);
			SetWindowVisibleFlag(FALSE);
			SetDoubleStartValidFlag(TRUE);
			SetMultiThreadFlag(TRUE);
			SetWindowUserCloseEnableFlag(FALSE);

			SetGraphMode(ScreenWidth, ScreenHeight, 32);
			SetWindowSize(960, 540);

			DxLib_Init();


			var tja = new TJA("正露丸2000.tja", "Oni");
			scores = tja.Scores;
			int songHandle = LoadSoundMem(Path.Combine(tja.WaveFileName));

			song = new Song();
			song.Init(songHandle);

			noteImageControl = new NoteImageControl("Note");

			scores[0].SetDrawTime(1422, ScreenWidth, speed);

			song.CurrentTime = (tja.Scores[0].OffsetMillisec);

			if (song.CurrentTime > -2000)
			{
				song.CurrentTime = -2000;
			}

			song.Play();

			while (ProcessMessage() == 0)
			{
				ClearDrawScreen();

				NowMicroSec = GetNowHiPerformanceCount();
				NowMilliSec = (int)(NowMicroSec / 1000);

				SetDrawScreen(DX_SCREEN_BACK);
				SetWindowVisibleFlag(TRUE);


				song.Update();

				nowTime = (int)song.CurrentTime + scores[0].OffsetMillisec;

				UpdateSpecialAuto();
				DrawNote();




				ScreenFlip();

				// ×ボタンが押されたかどうかを取得する
				if (GetWindowUserCloseFlag(TRUE) == 1)
				{
					break;
				}
			}
			DxLib_End();
		}
		static void Main(string[] args)
		{
			Singleton = new Supervision();
			Singleton.MainRun(args);
		}
	}
}
