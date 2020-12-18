using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tatelier
{
	static class Utility
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string GetCourse(string text)
		{
			switch (text.ToUpper())
			{
				case "0":
				case "EASY":
					return "Easy";
				case "1":
				case "NORMAL":
					return "Normal";
				case "2":
				case "HARD":
					return "Hard";
				case "3":
				case "ONI":
					return "Oni";
				case "4":
				case "EDIT":
					return "Edit";
				default:
					return text;
			}
		}
	}
}
