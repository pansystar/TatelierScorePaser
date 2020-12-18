using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tatelier
{
	class TJAInfo
	{
		public string FilePath = string.Empty;

		public string[] CourseNames = new string[0];

		public float AreaWidth = Supervision.ScreenWidth;

		public float Width = Supervision.ScreenWidth;
	}
	class TJA
	{
		public static TJA Create(TJAInfo info)
		{
			var tja = new TJA(info.FilePath, info.CourseNames);
			tja.SetDrawTime(info.AreaWidth, info.Width, 1);

			return tja;
		}

		public string Title;

        public string FilePath;

		public string WaveFileName;

		public double StartBPM;

		public double Offset;

		public Score[] Scores;

		public void SetDrawTime(float areaWidth, float width, float settingScrollSpeed)
		{
			foreach(var item in Scores)
			{
				item.SetDrawTime(areaWidth, width, settingScrollSpeed);
			}
		}

        public void SetDrawTime(float areaWidth, float width, float settingScrollSpeed, float itemScrollSpeed)
        {
            foreach (var item in Scores)
            {
                item.SetDrawTime(areaWidth, width, settingScrollSpeed, itemScrollSpeed);
            }
        }

        public TJA(string path, params string[] courseNameList)
		{
            FilePath = path;

            var regex = new Regex(@"(\S+):(.+)");

			var score = new List<Score>();

			var encoding = Encoding.UTF8;

            bool isAll = false;

            if(courseNameList?.Length == 0)
            {
                isAll = true;
                courseNameList = new string[]
                {
                    "Oni"
                };
            }

			using (var sr = new StreamReader(path, encoding))
			{
				foreach (var s in courseNameList)
				{
					string courseName = Utility.GetCourse(s);
					string nowCourse = Utility.GetCourse("Oni");
					var sb = new StringBuilder();

					bool nowReadScore = false;

					int[] ballonCountArray = new int[0];

					while (!sr.EndOfStream)
					{
						// 1行読む
						var line = sr.ReadLine();

						// #のやつら
						if (line.Length > 0
							&& line[0] == '#')
						{
							if (line.StartsWith("#START"))
							{
								nowReadScore = true;
								sb.AppendLine(line);
							}
							else if (line.StartsWith("#END"))
							{
								nowReadScore = false;
								sb.AppendLine(line);

								if (isAll || courseNameList.Any(v => v == nowCourse))
								{
									score.Add(new Score(sb, new ScoreInfo()
									{
										CourseName = nowCourse,
										StartBPM = StartBPM,
										Offset = Offset,
										balloonCountList = ballonCountArray
									}));
								}
								sb.Clear();
							}
							else
							{
								sb.AppendLine(line);
							}
						}
						else
						{
							if (nowReadScore)
							{
								sb.AppendLine(line);
							}
							else
							{
								// 例) line = "TITLE:夏祭り"
								if (regex.IsMatch(line))
								{
									var match = regex.Match(line);
									var groups = match.Groups;

									switch (groups[1].Value.ToUpper())
									{
										case "BALLOON":
											if(courseName == nowCourse)
											{
												ballonCountArray = groups[2].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(v => int.Parse(v.Replace(" ", string.Empty))).ToArray();
											}
											break;
										case "TITLE":
											Title = groups[2].Value;
											break;
										case "WAVE":
											WaveFileName = groups[2].Value;
											break;
										case "OFFSET":
											Offset = double.Parse(groups[2].Value);
											break;
										case "BPM":
											StartBPM = double.Parse(groups[2].Value);
											break;
										case "COURSE":
											nowCourse = Utility.GetCourse(groups[2].Value);
											break;
										case "LEVEL":
											break;
									}

								}
							}
						}
					}

					sr.BaseStream.Seek(0, SeekOrigin.Begin);
				}
			}

			Scores = score.ToArray();
		}
	}
}
