using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using EnglishStemmer;

namespace Centivus.Test.EnglishStemmer
{
	[TestFixture]
	public class Test
	{

		/// <summary>
		/// Do nothing with two character words
		/// </summary>
		[Test()]
		public void TestTwoCharWords()
		{
			foreach (string test in new string[5] { "at", "in", "of", "no", "on" })
			{
				EnglishWord word = new EnglishWord(test);
				Assert.AreEqual(test, word.Stem, test + " was modified to become " + word.Stem);
			}
		}

		/// <summary>
		/// Only trim apostrophes where they are surrounding quotes or part of an 's
		/// </summary>
		[Test()]
		public void TrimApos()
		{
			foreach (string testLose in new string[8] {
				"'surrounded'","trail's","Hamlet's", "father's", "father’s", "’bus", "John's", "horses’"})
			{
				EnglishWord word = new EnglishWord(testLose);
				//Check apostrophes chopped
				Assert.IsFalse(word.Stem.Contains("'"), testLose + " found '");
				Assert.IsFalse(word.Stem.Contains("’"), testLose + " found ’");
				Assert.IsFalse(word.Stem.Contains("`"), testLose + " found `");
			}
			foreach (string testKeep in new string[11] { 
				"he’ll", "she'll", "we’ll", "M’Coy", "o’clock", "O’Reilly", "M’Coy", "you’re", 
				"isn’t", "we’d", "Hallowe’en" })
			{
				EnglishWord word = new EnglishWord(testKeep);
				//Check apostrophes not chopped
				Assert.IsTrue(word.Stem.Contains("'"), testKeep + "missing '");
				//but are all converted to normal apostrophe
				Assert.IsFalse(word.Stem.Contains("’"), testKeep + " found ’");
				Assert.IsFalse(word.Stem.Contains("`"), testKeep + " found `");
			}
		}

		[Test()]
		public void CheckYs()
		{
			foreach (string testGood in new string[4] { "yellow", "bay", "buy", "yacht" })
			{
				EnglishWord word = EnglishWord.CreateForTest(testGood);
				word.MarkYs();
				Assert.IsTrue(word.Stem.Contains("Y"), testGood + " wasn't modified with a Y" + word.Stem);
			}

			foreach (string testBad in new string[3] { "burry", "busy", "cuddly" })
			{
				EnglishWord word = EnglishWord.CreateForTest(testBad);
				word.MarkYs();
				Assert.IsFalse(word.Stem.Contains("Y"), testBad + " was modified to become " + word.Stem);
			}
		}

		[Test]
		public void HandleApostrophesStep0()
		{
			foreach (string testGood in new string[4] { "'fred's'", "fred's", "'fred'", "fred'" })
			{
				EnglishWord word = EnglishWord.CreateForTest(testGood);
				word.StandardiseApostrophesAndStripLeading();
				word.StripTrailingApostrophe();
				Assert.AreEqual("fred", word.Stem, testGood + " produced " + word.Stem);
			}
		}


		/// <summary>
		/// Search for the longest among the following suffixes, and perform the action indicated. 
		/// <para>
		/// sses - replace by ss 
		/// </para><para>
		/// ied+   ies* - replace by i if preceded by more than one letter, otherwise by ie (so ties -> tie, cries -> cri) 
		/// </para><para>
		/// s  - delete if the preceding word part contains a vowel not immediately before the s 
		/// (so gas and this retain the s, gaps and kiwis lose it) 
		///</para><para>
		/// us+   ss - do nothing 
		/// </para></summary>
		[Test]
		public void StripSuffixStep1a()
		{
			//sses - replace by ss
			EnglishWord wordSuccesses = EnglishWord.CreateForTest("successes");
			wordSuccesses.StripSuffixStep1a();
			Assert.AreEqual("success", wordSuccesses.Stem);

			// ied+   ies* - replace by i if preceded by more than one letter, otherwise by ie (so ties -> tie, cries -> cri) 
			EnglishWord wordTies = EnglishWord.CreateForTest("ties");
			wordTies.StripSuffixStep1a();
			Assert.AreEqual("tie", wordTies.Stem);
			//and
			EnglishWord wordCries = EnglishWord.CreateForTest("cries");
			wordCries.StripSuffixStep1a();
			Assert.AreEqual("cri", wordCries.Stem);

			//s  - delete if the preceding word part contains a vowel not immediately before the s 
			// (so gas and this retain the s, gaps and kiwis lose it) 
			EnglishWord wordGas = EnglishWord.CreateForTest("gas");
			wordGas.StripSuffixStep1a();
			Assert.AreEqual("gas", wordGas.Stem);

			EnglishWord wordThis = EnglishWord.CreateForTest("this");
			wordThis.StripSuffixStep1a();
			Assert.AreEqual("this", wordThis.Stem);

			EnglishWord wordGaps = EnglishWord.CreateForTest("gaps");
			wordGaps.StripSuffixStep1a();
			Assert.AreEqual("gap", wordGaps.Stem);

			EnglishWord wordKiwis = EnglishWord.CreateForTest("kiwis");
			wordKiwis.StripSuffixStep1a();
			Assert.AreEqual("kiwi", wordKiwis.Stem);

			//My tests
			EnglishWord word = EnglishWord.CreateForTest("righteous");
			word.StripSuffixStep1a();
			Assert.AreEqual("righteous", word.Stem);
			word = EnglishWord.CreateForTest("business");
			word.StripSuffixStep1a();
			Assert.AreEqual("business", word.Stem);			
		}

		/// <summary>
		/// Step 1b: Search for the longest among the following suffixes, and perform the action indicated. 
		/// <para>eed   eedly+  - replace by ee if in R1 
		/// </para><para>
		/// ed   edly+   ing   ingly+ - delete if the preceding word part contains a vowel, and then 
		/// if the word ends at, bl or iz add e (so luxuriat -> luxuriate), or 
		/// if the word ends with a double remove the last letter (so hopp -> hop), or 
		/// if the word is short, add e (so hop -> hope) 
		/// </para>
		/// </summary>
		[Test]
		public void StripSuffixStep1b()
		{
			//eed   eedly+  - replace by ee if in R1 
			EnglishWord word = EnglishWord.CreateWithR1R2("succeed");
			word.StripSuffixStep1b();
			Assert.AreEqual("succee", word.Stem);			

			//ed   edly+   ing   ingly+ - delete if the preceding word part contains a vowel, and then 
			word = EnglishWord.CreateWithR1R2("sucked");
			word.StripSuffixStep1b();
			Assert.AreEqual("suck", word.Stem);

			word = EnglishWord.CreateWithR1R2("swingingly");
			word.StripSuffixStep1b();
			Assert.AreEqual("swing", word.Stem);

			word = EnglishWord.CreateWithR1R2("exceedingly");
			word.StripSuffixStep1b();
			Assert.AreEqual("exceed", word.Stem);
			
			word = EnglishWord.CreateWithR1R2("deed");
			word.StripSuffixStep1b();
			Assert.AreEqual("deed", word.Stem);

			// if the word ends at, bl or iz add e (so luxuriat -> luxuriate), or 
			word = EnglishWord.CreateWithR1R2("disabling");
			word.StripSuffixStep1b();
			Assert.AreEqual("disable", word.Stem);

			word = EnglishWord.CreateWithR1R2("luxuriated");
			word.StripSuffixStep1b();
			Assert.AreEqual("luxuriate", word.Stem);

			word = EnglishWord.CreateWithR1R2("specializingly");
			word.StripSuffixStep1b();
			Assert.AreEqual("specialize", word.Stem);

			word = EnglishWord.CreateWithR1R2("supposedly");
			word.StripSuffixStep1b();
			Assert.AreEqual("suppos", word.Stem);
			
			word = EnglishWord.CreateWithR1R2("specialised");
			word.StripSuffixStep1b();
			Assert.AreEqual("specialis", word.Stem);

			// if the word ends with a double remove the last letter (so hopp -> hop), or 
			word = EnglishWord.CreateWithR1R2("shopped");
			word.StripSuffixStep1b();
			Assert.AreEqual("shop", word.Stem);

			word = EnglishWord.CreateWithR1R2("supping");
			word.StripSuffixStep1b();
			Assert.AreEqual("sup", word.Stem);

			word = EnglishWord.CreateWithR1R2("prodded");
			word.StripSuffixStep1b();
			Assert.AreEqual("prod", word.Stem);

			// if the word is short, add e (so hop -> hope) 
			word = EnglishWord.CreateWithR1R2("hoping");
			word.StripSuffixStep1b();
			Assert.AreEqual("hope", word.Stem);
		}

		[Test()]
		public void IsShort()
		{
			//Define a short syllable in a word as either 
			//(a) a vowel followed by a non-vowel other than w, x or Y and preceded by a non-vowel, or 
			//* (b) a vowel at the beginning of the word followed by a non-vowel. 

			//So rap, trap, entrap end with a short syllable, and ow, on, at are classed as short syllables. But uproot, bestow, disturb do not end with a short syllable. 
			EnglishWord word = EnglishWord.CreateForTest("rap");
			Assert.AreEqual(true, word.IsShortSyllable(1), word.Original + " is not a short syllable");

			word = EnglishWord.CreateForTest("trap");
			Assert.AreEqual(true, word.IsShortSyllable(2), word.Original + " is not a short syllable");
			word = EnglishWord.CreateForTest("entrap");
			Assert.AreEqual(true, word.IsShortSyllable(4), word.Original + " is not a short syllable");

			word = EnglishWord.CreateForTest("ow");
			Assert.AreEqual(true, word.IsShortSyllable(0), word.Original + " is not a short syllable");
			word = EnglishWord.CreateForTest("on");
			Assert.AreEqual(true, word.IsShortSyllable(0), word.Original + " is not a short syllable");
			word = EnglishWord.CreateForTest("at");
			Assert.AreEqual(true, word.IsShortSyllable(0), word.Original + " is not a short syllable");

			word = EnglishWord.CreateWithR1R2("uproot");
			Assert.AreEqual(false, word.IsShortSyllable(word.Original.IndexOf(word.GetR1())), word.Original + " is a short syllable");
			word = EnglishWord.CreateWithR1R2("bestow");
			Assert.AreEqual(false, word.IsShortSyllable(word.Original.IndexOf(word.GetR1())), word.Original + " is a short syllable");
			word = EnglishWord.CreateWithR1R2("disturb");
			Assert.AreEqual(false, word.IsShortSyllable(word.Original.IndexOf(word.GetR1())), word.Original + " is a short syllable");

			//A word is called short if it ends in a short syllable, and if R1 is null. 
			//So bed, shed and shred are short words, bead, embed, beds are not short words. 
			word = EnglishWord.CreateWithR1R2("bed");
			Assert.AreEqual(true, word.IsShortWord(), word.Original + " was expected to be a short Word");
			word = EnglishWord.CreateWithR1R2("shed");
			Assert.AreEqual(true, word.IsShortWord(), word.Original + " was expected to be a short Word");
			word = EnglishWord.CreateWithR1R2("shred");
			Assert.AreEqual(true, word.IsShortWord(), word.Original + " was expected to be a short Word");

			word = EnglishWord.CreateWithR1R2("bead");
			Assert.AreEqual(false, word.IsShortWord(), word.Original + " was not expected to be a short Word");
			word = EnglishWord.CreateWithR1R2("embed");
			Assert.AreEqual(false, word.IsShortWord(), word.Original + " was not expected to be a short Word");
			word = EnglishWord.CreateWithR1R2("beds");
			Assert.AreEqual(false, word.IsShortWord(), word.Original + " was not expected to be a short Word");
		}

		/// <summary>
		/// replace suffix y or Y by i if preceded by a non-vowel which is not the first letter of the word (so cry -> cri, by -> by, say -> say)
		/// </summary>
		[Test()]
		public void ReplaceSuffixStep1c()
		{
			EnglishWord word = EnglishWord.CreateForTest("cry");
			word.ReplaceSuffixStep1c();
			Assert.AreEqual("cri", word.Stem);
			word = EnglishWord.CreateForTest("by");
			word.ReplaceSuffixStep1c();
			Assert.AreEqual("by", word.Stem);
			word = EnglishWord.CreateForTest("say");
			word.ReplaceSuffixStep1c();
			Assert.AreEqual("say", word.Stem);
		}

		/// <summary>
		/// Step 2: 
		///Search for the longest among the following suffixes, and, if found and in R1, perform the action indicated.
		///
		///tional:   replace by tion 
		///enci:   replace by ence 
		///anci:   replace by ance 
		///abli:   replace by able 
		///entli:   replace by ent 
		///izer   ization:   replace by ize 
		///ational   ation   ator:   replace by ate 
		///alism   aliti   alli:   replace by al 
		///fulness:   replace by ful 
		///ousli   ousness:   replace by ous 
		///iveness   iviti:   replace by ive 
		///biliti   bli+:   replace by ble 
		///ogi+:   replace by og if preceded by l 
		///fulli+:   replace by ful fully
		///lessli+:   replace by less 
		///li+:   delete if preceded by a valid li-ending
		/// </summary>
		[Test()]
		public void Step2()
		{			
			EnglishWord word = EnglishWord.CreateWithR1R2("emergenci");
			word.ReplaceEndingStep2();
			Assert.AreEqual("emergence", word.Stem);
			word = EnglishWord.CreateWithR1R2("discrepanci");
			word.ReplaceEndingStep2();
			Assert.AreEqual("discrepance", word.Stem);
			word = EnglishWord.CreateWithR1R2("suitabli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("suitable", word.Stem);
			word = EnglishWord.CreateWithR1R2("eminentli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("eminent", word.Stem);
			word = EnglishWord.CreateWithR1R2("presentli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("present", word.Stem);
			word = EnglishWord.CreateWithR1R2("womanizer");
			word.ReplaceEndingStep2();
			Assert.AreEqual("womanize", word.Stem);
			word = EnglishWord.CreateWithR1R2("civilization");
			word.ReplaceEndingStep2();
			Assert.AreEqual("civilize", word.Stem);			
			word = EnglishWord.CreateWithR1R2("commercialization");
			word.ReplaceEndingStep2();
			Assert.AreEqual("commercialize", word.Stem);
			word = EnglishWord.CreateWithR1R2("instigational");
			word.ReplaceEndingStep2();
			Assert.AreEqual("instigate", word.Stem);
			word = EnglishWord.CreateWithR1R2("instigation");
			word.ReplaceEndingStep2();
			Assert.AreEqual("instigate", word.Stem);
			word = EnglishWord.CreateWithR1R2("instigator");
			word.ReplaceEndingStep2();
			Assert.AreEqual("instigate", word.Stem);
			word = EnglishWord.CreateWithR1R2("nationalism");
			word.ReplaceEndingStep2();
			Assert.AreEqual("national", word.Stem);			
			word = EnglishWord.CreateWithR1R2("nationaliti");
			word.ReplaceEndingStep2();
			Assert.AreEqual("national", word.Stem);
			word = EnglishWord.CreateWithR1R2("rationalli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("rational", word.Stem);
			word = EnglishWord.CreateWithR1R2("institutional");
			word.ReplaceEndingStep2();
			Assert.AreEqual("institution", word.Stem);
			word = EnglishWord.CreateWithR1R2("usefulness");
			word.ReplaceEndingStep2();
			Assert.AreEqual("useful", word.Stem);
			word = EnglishWord.CreateWithR1R2("obviousli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("obvious", word.Stem);
			word = EnglishWord.CreateWithR1R2("obviousness");
			word.ReplaceEndingStep2();
			Assert.AreEqual("obvious", word.Stem);
			word = EnglishWord.CreateWithR1R2("forgiveness");
			word.ReplaceEndingStep2();
			Assert.AreEqual("forgive", word.Stem);
			word = EnglishWord.CreateWithR1R2("nativiti");
			word.ReplaceEndingStep2();
			Assert.AreEqual("native", word.Stem);
			word = EnglishWord.CreateWithR1R2("instabiliti");
			word.ReplaceEndingStep2();
			Assert.AreEqual("instable", word.Stem);
			word = EnglishWord.CreateWithR1R2("fashionabli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("fashionable", word.Stem);
			word = EnglishWord.CreateWithR1R2("suitabli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("suitable", word.Stem);
			word = EnglishWord.CreateWithR1R2("visibli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("visible", word.Stem);
			word = EnglishWord.CreateWithR1R2("stressfulli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("stressful", word.Stem);
			word = EnglishWord.CreateWithR1R2("lifelessli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("lifeless", word.Stem);
			word = EnglishWord.CreateWithR1R2("fulli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("fulli", word.Stem);
			word = EnglishWord.CreateWithR1R2("contriteli");
			word.ReplaceEndingStep2();
			Assert.AreEqual("contrite", word.Stem);
			word = EnglishWord.CreateWithR1R2("biologi");
			word.ReplaceEndingStep2();
			Assert.AreEqual("biolog", word.Stem);
		}

		//[Test()]
		public void Step2Fail()
		{
			//These work but shouldn't - Look wrong
			EnglishWord word = EnglishWord.CreateWithR1R2("international");
			word.ReplaceEndingStep2();
			Assert.AreEqual("internate", word.Stem);

			///Ooh these don't work
			word = EnglishWord.CreateWithR1R2("commercialisation");
			word.ReplaceEndingStep2();
			Assert.AreEqual("commercialise", word.Stem);

			word = EnglishWord.CreateWithR1R2("psychology");
			word.ReplaceEndingStep2();
			Assert.AreEqual("psycholog", word.Stem); //?????
		}

		/// <summary>
		/// Step 3: 
		///Search for the longest among the following suffixes, and, if found and in R1, perform the action indicated. 
		///
		///tional+:   replace by tion 
		///ational+:   replace by ate 
		///alize:   replace by al 
		///icate   iciti   ical:   replace by ic 
		///ful   ness:   delete 
		///ative*:   delete if in R2 
		/// </summary>
		[Test()]
		public void Step3()
		{
			EnglishWord word = EnglishWord.CreateWithR1R2("intentional");
			word.ReplaceEndingStep3();
			Assert.AreEqual("intention", word.Stem);
			word = EnglishWord.CreateWithR1R2("locational");
			word.ReplaceEndingStep3();
			Assert.AreEqual("locate", word.Stem);
			word = EnglishWord.CreateWithR1R2("intentionalize");
			word.ReplaceEndingStep3();
			Assert.AreEqual("intentional", word.Stem);
			word = EnglishWord.CreateWithR1R2("silicate");
			word.ReplaceEndingStep3();
			Assert.AreEqual("silic", word.Stem);
			word = EnglishWord.CreateWithR1R2("electriciti");
			word.ReplaceEndingStep3();
			Assert.AreEqual("electric", word.Stem);
			word = EnglishWord.CreateWithR1R2("electrical");
			word.ReplaceEndingStep3();
			Assert.AreEqual("electric", word.Stem);
			word = EnglishWord.CreateWithR1R2("useful");
			word.ReplaceEndingStep3();
			Assert.AreEqual("use", word.Stem);
			word = EnglishWord.CreateWithR1R2("usefulness");
			word.ReplaceEndingStep3();
			Assert.AreEqual("useful", word.Stem);
			word = EnglishWord.CreateWithR1R2("regenerative");
			word.ReplaceEndingStep3();
			Assert.AreEqual("regener", word.Stem);
		}


		/// <summary>
		/// Step 4: Search for the longest among the following suffixes, and, if found and in R2, perform the action indicated. 
		/// al   ance   ence   er   ic   able   ible   ant   ement   ment   ent   ism   ate   iti   ous   ive   ize delete 
		/// ion delete if preceded by s or t 
		/// </summary>
		[Test()]
		public void Step4()
		{
			EnglishWord word = EnglishWord.CreateWithR1R2("preferential");
			word.StripSuffixStep4();
			Assert.AreEqual("preferenti", word.Stem);
			word = EnglishWord.CreateWithR1R2("ambulance");
			word.StripSuffixStep4();
			Assert.AreEqual("ambul", word.Stem);
			word = EnglishWord.CreateWithR1R2("emergence");
			word.StripSuffixStep4();
			Assert.AreEqual("emerg", word.Stem);
			word = EnglishWord.CreateWithR1R2("deserter");
			word.StripSuffixStep4();
			Assert.AreEqual("desert", word.Stem);
			word = EnglishWord.CreateWithR1R2("rhetoric");
			word.StripSuffixStep4();
			Assert.AreEqual("rhetor", word.Stem);
			word = EnglishWord.CreateWithR1R2("communicable");
			word.StripSuffixStep4();
			Assert.AreEqual("communic", word.Stem);
			word = EnglishWord.CreateWithR1R2("militant");
			word.StripSuffixStep4();
			Assert.AreEqual("milit", word.Stem);
			word = EnglishWord.CreateWithR1R2("establishement");//Cant think of a real word
			word.StripSuffixStep4();
			Assert.AreEqual("establish", word.Stem);
			word = EnglishWord.CreateWithR1R2("establishment");
			word.StripSuffixStep4();
			Assert.AreEqual("establish", word.Stem);
			word = EnglishWord.CreateWithR1R2("concurrent");
			word.StripSuffixStep4();
			Assert.AreEqual("concurr", word.Stem);
			word = EnglishWord.CreateWithR1R2("minimalism");
			word.StripSuffixStep4();
			Assert.AreEqual("minimal", word.Stem);
			word = EnglishWord.CreateWithR1R2("regenerate");
			word.StripSuffixStep4();
			Assert.AreEqual("regener", word.Stem);
			word = EnglishWord.CreateWithR1R2("connectiviti");
			word.StripSuffixStep4();
			Assert.AreEqual("connectiv", word.Stem);
			word = EnglishWord.CreateWithR1R2("ridiculous");
			word.StripSuffixStep4();
			Assert.AreEqual("ridicul", word.Stem);
			word = EnglishWord.CreateWithR1R2("superlative");
			word.StripSuffixStep4();
			Assert.AreEqual("superlat", word.Stem);
			word = EnglishWord.CreateWithR1R2("disincentivize");
			word.StripSuffixStep4();
			Assert.AreEqual("disincentiv", word.Stem);
			word = EnglishWord.CreateWithR1R2("apprehension");
			word.StripSuffixStep4();
			Assert.AreEqual("apprehens", word.Stem);
			word = EnglishWord.CreateWithR1R2("computation");
			word.StripSuffixStep4();
			Assert.AreEqual("computat", word.Stem);
		}

		/// <summary>
		/// Step 5: * Search for the the following suffixes, and, if found, perform the action indicated. 
		///e delete if in R2, or in R1 and not preceded by a short syllable 
		/// l delete if in R2 and preceded by l 
		/// </summary>
		[Test()]
		public void Step5()
		{
			EnglishWord word = EnglishWord.CreateWithR1R2("ambulance");
			word.StripSuffixStep5();
			Assert.AreEqual("ambulanc", word.Stem);
			word = EnglishWord.CreateWithR1R2("sublime");
			word.StripSuffixStep5();
			Assert.AreEqual("sublim", word.Stem);
			word = EnglishWord.CreateWithR1R2("firewall");
			word.StripSuffixStep5();
			Assert.AreEqual("firewal", word.Stem);
		}

		/// <summary>
		///Finally, turn any remaining Y letters in the word back into lower case. 
		/// </summary>
		[Test()]
		public void Finally()
		{
			EnglishWord word = EnglishWord.CreateWithR1R2("ferrY");
			word.Finally();
			Assert.AreEqual("ferry", word.Stem);

			word = EnglishWord.CreateWithR1R2("whYwhY");
			word.Finally();
			Assert.AreEqual("whywhy", word.Stem);

		}
	}
}
