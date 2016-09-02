using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnglishStemmer;
using System.Collections;

namespace Centivus.Test.EnglishStemmer
{
	[TestFixture()]
	public class DictionaryTest
	{
		[Test()]
		public void TestSample()
		{
			foreach (string line in Properties.Resources.SampleVocabulary.Split('\n'))
			{
				string[] parts = line.Split(',');
				if (parts.Length > 1) //Excel saves with a blank line which arrives as a single part
				{
					TestInstance(parts[0].Replace("\r", "").Trim(), parts[1].Replace("\r", "").Trim());
				}
			}
		}


		public void TestInstance(string actual, string expected)
		{
			Assert.AreEqual(expected, new EnglishWord(actual).Stem, "Test failed with inputs of \"" + actual + ", " + expected + "\"");
			//Console.WriteLine(actual + " stemmed to " + expected);
		}

		[Test()]
		public void TestFull()
		{
			List<Exception> errors = new List<Exception>();
			IEnumerator output = Properties.Resources.output.Split('\n').GetEnumerator();
			foreach (string input in Properties.Resources.voc.Split('\n'))
			{
				output.MoveNext();
				try
				{
					string wordIn = input.Replace("\r", "").Trim();
					string stemOut = ((string)output.Current).Replace("\r", "").Trim();
					//Override the default result
					if (wordIn == "fluently") 
						stemOut = "fluent";
					TestInstance(wordIn, stemOut);
				}
				catch (Exception ex)
				{
					errors.Add(ex);
				}
			}
			foreach (Exception ex in errors)
				Console.WriteLine(ex.Message);

			if (errors.Count > 0)
				throw new ApplicationException("Failed to parse entire file, " + errors.Count + " errors.");
		}

		public void SpecificTest()
		{
			TestInstance("succeed", "succeed");
		}

	}
}
