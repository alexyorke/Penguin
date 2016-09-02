using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using EnglishStemmer;

namespace Centivus.Test.EnglishStemmer
{
	[TestFixture]
	public class R1andR2
	{
		/// <summary>
		/// Test R1 and R2 definition matches for Beautiful
		/// </summary>
		[Test()]
		public void Beautiful()
		{
			string beautiful = "beautiful";
			WordRegion r1 = EnglishWord.CalculateR(beautiful,0);
			Assert.AreEqual(beautiful.Length, r1.End, "Error in R1 End");
			Assert.AreEqual(5, r1.Start, "Error in R1 Start");

			WordRegion r2 = EnglishWord.CalculateR(beautiful, r1.Start);
			Assert.AreEqual(beautiful.Length, r2.End, "Error in R2 End");
			Assert.AreEqual(7, r2.Start, "Error in R2 Start");
		}

		/// <summary>
		/// Test R1 and R2 definition matches for Beauty
		/// </summary>
		[Test()]
		public void Beauty()
		{
			string beauty = "beauty";
			WordRegion r1 = EnglishWord.CalculateR(beauty,0);
			Assert.AreEqual(beauty.Length, r1.End, "Error in R1 End");
			Assert.AreEqual(5, r1.Start, "Error in R1 Start");

			WordRegion r2 = EnglishWord.CalculateR(beauty, r1.Start);
			Assert.AreEqual(beauty.Length, r2.End, "Error in R2 End");
			Assert.AreEqual(beauty.Length, r2.Start, "Error in R2 Start");
		}

		/// <summary>
		/// Test R1 and R2 definition matches for Beau
		/// </summary>
		[Test()]
		public void Beau()
		{
			string beau = "beau";
			WordRegion r1 = EnglishWord.CalculateR(beau,0);
			Assert.AreEqual(beau.Length, r1.End, "Error in R1 End");
			Assert.AreEqual(4, r1.Start, "Error in R1 Start");

			WordRegion r2 = EnglishWord.CalculateR(beau, r1.Start);
			Assert.AreEqual(beau.Length,r2.End, "Error in R2 End");
			Assert.AreEqual(beau.Length, r2.Start, "Error in R2 Start");
		}

		/// <summary>
		/// Test R1 and R2 definition matches for animadversion
		/// </summary>
		[Test()]
		public void Animadversion()
		{
			string animadversion = "animadversion";
			WordRegion r1 = EnglishWord.CalculateR(animadversion,0);
			Assert.AreEqual(animadversion.Length,r1.End ,"Error in R1 End");
			Assert.AreEqual( 2, r1.Start, "Error in R1 Start");

			WordRegion r2 = EnglishWord.CalculateR(animadversion, r1.Start);
			Assert.AreEqual(animadversion.Length,r2.End, "Error in R2 End");
			Assert.AreEqual(4, r2.Start, "Error in R2 Start");
		}

		/// <summary>
		/// Test R1 and R2 definition matches for sprinkled
		/// </summary>
		[Test()]
		public void Sprinkled()
		{
			string sprinkled = "sprinkled";
			WordRegion r1 = EnglishWord.CalculateR(sprinkled,0);
			Assert.AreEqual(sprinkled.Length,r1.End, "Error in R1 End");
			Assert.AreEqual(5, r1.Start, "Error in R1 Start");

			WordRegion r2 = EnglishWord.CalculateR(sprinkled, r1.Start);
			Assert.AreEqual(sprinkled.Length, r2.End, "Error in R2 End");
			Assert.AreEqual(sprinkled.Length, r2.Start, "Error in R2 Start");
		}

		/// <summary>
		/// Test R1 and R2 definition matches for eucharist
		/// </summary>
		[Test()]
		public void eucharist()
		{
			string eucharist = "eucharist";
			WordRegion r1 = EnglishWord.CalculateR(eucharist,0);
			Assert.AreEqual(eucharist.Length, r1.End, "Error in R1 End");
			Assert.AreEqual(3, r1.Start, "Error in R1 Start");

			WordRegion r2 = EnglishWord.CalculateR(eucharist, r1.Start);
			Assert.AreEqual(eucharist.Length, r2.End , "Error in R2 End");
			Assert.AreEqual(6, r2.Start, "Error in R2 Start");
		}

	}
}
