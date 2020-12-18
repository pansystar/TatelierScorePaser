using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{
	/// <summary>
	/// 分岐譜面管理クラス
	/// ※分岐のない譜面も使う
	/// </summary>
	class BranchScoreControl2
	{
		public List<int> KeyList = new List<int>();

		public SortedDictionary<int, BranchType> BranchTypeList = new SortedDictionary<int, BranchType>();

		public SortedDictionary<int, BranchScore> NormalScoreList = new SortedDictionary<int, BranchScore>();
		public SortedDictionary<int, BranchScore> ExpertScoreList = new SortedDictionary<int, BranchScore>();
		public SortedDictionary<int, BranchScore> MasterScoreList = new SortedDictionary<int, BranchScore>();

		public BranchType NowBranchType;

		public BranchScore CommonScore = new BranchScore();

		int nowDeadlineIndex = 0;

		public List<int> OneBeforeMeasureTime = new List<int>();

		public bool TryChangeDeadline(int time)
		{
			if (nowDeadlineIndex == OneBeforeMeasureTime.Count) return false;
			if(OneBeforeMeasureTime[nowDeadlineIndex] < time)
			{
				nowDeadlineIndex++;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Change(int time, BranchType branchType)
		{
			NowBranchType = branchType;

			foreach (var key in KeyList)
			{
				if (key < time) continue;

				BranchTypeList[key] = branchType;
			}
		}

		public IEnumerable<BranchScore> GetBranchScoreList()
		{
			yield return CommonScore;

			foreach(var t in BranchTypeList)
			{
				switch (t.Value)
				{
					case BranchType.Normal:
						{
							if (NormalScoreList.TryGetValue(t.Key, out var score))
							{
								yield return score;
							}
						}
						break;
					case BranchType.Expert:
						{
							if (ExpertScoreList.TryGetValue(t.Key, out var score))
							{
								yield return score;
							}
						}
						break;
					case BranchType.Master:
						{
							if (MasterScoreList.TryGetValue(t.Key, out var score))
							{
								yield return score;
							}
						}
						break;
				}
			}
		}

		public IEnumerable<(BranchType BranchType, BranchScore BranchScore)> GetAllBranchScoreList()
		{
			yield return (BranchType.Common, CommonScore);

			foreach (var item in NormalScoreList) yield return (BranchType.Normal, item.Value);

			foreach (var item in ExpertScoreList) yield return (BranchType.Expert, item.Value);

			foreach (var item in MasterScoreList) yield return (BranchType.Master, item.Value);
		}


		public void Build()
		{
			CommonScore.Build();
			foreach (var item in NormalScoreList.Values) item.Build();
			foreach (var item in ExpertScoreList.Values) item.Build();
			foreach (var item in MasterScoreList.Values) item.Build();
		}

		public BranchScoreControl2()
		{

		}
	}
}
