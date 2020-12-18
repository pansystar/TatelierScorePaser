using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;

namespace Tatelier
{
	/// <summary>
	/// 音符画像管理クラス
	/// </summary>
	internal class NoteImageControl
		: IDisposable
	{
		bool disposed = true;

		int don = -1;
		int kat = -1;
		int donBig = -1;
		int katBig = -1;
		int roll = -1;
		int rollContent = -1;
		int rollBig = -1;
		int rollContentBig = -1;
		int balloon = -1;
		int rollEnd = -1;
		int rollBigEnd = -1;

		/// <summary>
		/// 音符画像ハンドルを取得する
		/// </summary>
		/// <param name="noteType">音符種類</param>
		/// <returns>画像ハンドル</returns>
		public int GetImageHandle(NoteType noteType)
		{
			switch (noteType)
			{
				case NoteType.Don: return don;
				case NoteType.Kat: return kat;
				case NoteType.DonBig: return donBig;
				case NoteType.KatBig: return katBig;
				case NoteType.Roll: return roll;
				case NoteType.RollBig: return rollBig;
				case NoteType.Balloon: return balloon;
				default: return -1;
			}
		}

		/// <summary>
		/// 特殊音符の開始から終了までをつなぐ音符画像ハンドルを取得する	
		/// </summary>
		/// <param name="noteType">音符種類</param>
		/// <returns>画像ハンドル</returns>
		public int GetContentNoteImageHandle(NoteType noteType)
		{
			switch (noteType)
			{
				case NoteType.Roll: return rollContent;
				case NoteType.RollBig: return rollContentBig;
				default: return -1;
			}
		}

		/// <summary>
		/// 特殊音符の終了印譜画像ハンドルを取得する
		/// </summary>
		/// <param name="noteType">音符種類</param>
		/// <returns>画像ハンドル</returns>
		public int GetEndNoteImageHandle(NoteType noteType)
		{
			switch (noteType)
			{
				case NoteType.Roll: return rollEnd;
				case NoteType.RollBig: return rollBigEnd;
				case NoteType.Balloon: return balloon;
				default: return -1;
			}
		}

		void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// managed
				}
								
				// unmanaged
				don = DeleteGraph(don);
				kat = DeleteGraph(kat);
				donBig = DeleteGraph(donBig);
				katBig = DeleteGraph(katBig);
				roll = DeleteGraph(roll);
				rollContent = DeleteGraph(rollContent);
				rollBig = DeleteGraph(rollBig);
				rollContentBig = DeleteGraph(rollContentBig);
				rollEnd = DeleteGraph(rollEnd);
				rollBigEnd = DeleteGraph(rollBigEnd);

				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		~NoteImageControl()
		{
			Dispose();
		}


		public NoteImageControl()
		{

		}

		[Obsolete("")]
		public NoteImageControl(string folderPath)
		{			
			don = LoadGraph(Path.Combine(folderPath, "Don.png"));
			kat = LoadGraph(Path.Combine(folderPath, "Kat.png"));
			donBig = LoadGraph(Path.Combine(folderPath, "DonBig.png"));
			katBig = LoadGraph(Path.Combine(folderPath, "KatBig.png"));
			balloon = LoadGraph(Path.Combine(folderPath, "Balloon.png"));
			roll = LoadGraph(Path.Combine(folderPath, "Roll.png"));
			rollContent = LoadGraph(Path.Combine(folderPath, "RollContent.png"));
			rollEnd = LoadGraph(Path.Combine(folderPath, "RollEnd.png"));
			rollBig = LoadGraph(Path.Combine(folderPath, "RollBig.png"));
			rollContentBig = LoadGraph(Path.Combine(folderPath, "RollContentBig.png"));
			rollBigEnd = LoadGraph(Path.Combine(folderPath, "RollEndBig.png"));
		}
	}
}
