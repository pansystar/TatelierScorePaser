using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static DxLibDLL.DX;

namespace Tatelier
{
    /// <summary>
    /// 音楽クラス
    /// </summary>
    public class Song : IDisposable
    {
        int handle = -1;

        bool isCurrentChange = false;
        bool nowPlaying;

        long currentTime = 0;
        bool disposed = false;

        long StartMicroTime;

        public long CurrentTime
        {
            get
			{
				return currentTime;
            }
            set
            {
                currentTime = value;
				StartMicroTime = Supervision.NowMilliSec - currentTime;
				isCurrentChange = true;
			}
        }

        /// <summary>
        /// 音楽が終了してるかどうか
        /// true: 終了済, false: 未終了
        /// </summary>
		public bool IsEnd => currentTime > GetSoundTotalTime(handle);

		int time;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="handle">音源ハンドル</param>
        /// <returns></returns>
		public int Init(int handle)
		{
			if (this.handle != -1)
			{
				StopSoundMem(this.handle);
				DeleteSoundMem(this.handle);
			}

			this.handle = handle;
			currentTime = 0;
			StartMicroTime = Supervision.NowMilliSec;

			return 0;
		}

		public int Init(string filePath)
        {
            if (handle != -1)
            {
                StopSoundMem(handle);
                DeleteSoundMem(handle);
            }

            handle = LoadSoundMem(filePath);
            currentTime = 0;
			StartMicroTime = Supervision.NowMilliSec;

			return 0;
        }

        public void Play()
        {
            if (!nowPlaying)
            {
                nowPlaying = true;
            }
        }
        
        public void Update()
        {
			if (isCurrentChange)
			{
				if(CheckSoundMem(handle) == 1)
				{
					StopSoundMem(handle);
				}
				SetSoundCurrentTime(currentTime, handle);
				StartMicroTime = Supervision.NowMilliSec - currentTime;
				isCurrentChange = false;
			}

			if (nowPlaying)
			{
				long nowTime = Supervision.NowMilliSec - StartMicroTime;
				if (CheckSoundMem(handle) == 0)
				{
					if (nowTime >= 0)
					{
						SetSoundCurrentTime(nowTime, handle);
						PlaySoundMem(handle, DX_PLAYTYPE_BACK, FALSE);
					}
				}
				//if (nowTime >= 0 &&
				//	time < Supervision.NowMilliSec)
				//{
				//	nowTime = GetSoundCurrentTime(handle);
				//	StartMicroTime = Supervision.NowMilliSec - nowTime;
				//	time = Supervision.NowMilliSec;
				//}
				currentTime = nowTime;
			}
			else
			{
				if (CheckSoundMem(handle) == 1)
				{
					StopSoundMem(handle);
					currentTime = GetSoundCurrentTime(handle);
				}
				StartMicroTime = Supervision.NowMilliSec - currentTime;
			}
        }

        public void Pause()
        {
            nowPlaying = false;
        }

        public void Stop()
        {
            StopSoundMem(handle);
            nowPlaying = false;
            currentTime = 0;
        }

		public void Dispose()
		{
			Dispose(true);
		}

		void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{

				}
				disposed = true;
			}
		}

		~Song()
		{
			Dispose();
		}

		public Song()
        {

        }
    }
}
