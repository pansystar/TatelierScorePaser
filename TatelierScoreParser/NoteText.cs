using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;

namespace Tatelier
{
	public enum NoteTextType
	{
		/// <summary>
		/// ドン
		/// </summary>
		Don,
		/// <summary>
		/// ド
		/// </summary>
		Do,
		/// <summary>
		/// コ
		/// </summary>
		Ko,
		/// <summary>
		/// カッ
		/// </summary>
		Katt,
		/// <summary>
		/// カ
		/// </summary>
		Kat,
		/// <summary>
		/// 連打
		/// </summary>
		Renda,
		/// <summary>
		/// ゲキ連打
		/// </summary>
		GekiRenda,
	}

	class NoteText
		: IDisposable
	{
		bool disposed = false;

		int handle;

		public void Draw(float xf, float yf, NoteTextType noteTextType)
		{
			GetGraphSize(handle, out var w, out var h);
			int itemHeight = 33;
			DrawRectRotaGraphFastF(xf, yf, 0, ((int)noteTextType) * itemHeight, w, itemHeight, 1.0F, 0.0F, handle, TRUE);
		}

		void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					DeleteGraph(handle);
				}
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public NoteText(string filePath)
		{
			handle = LoadGraph(filePath);
		}
	}
}
