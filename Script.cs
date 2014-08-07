using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplescriptPort
{
    public class Script
    {
        private string[][] x;

        private bool recursiveFind(string[][] x, string theNeedle)
        {
            for (int i = 0; i < x.Length; i++)
            {
                string[] this_item = x[i];
                for (int j = 0; j < this_item.Length; j++)
                {
                    if (this_item[j].Is(theNeedle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string blockSearcher(string[][] x, string[] search_term)
        {
            //search term is in the form of {"gate", "green"}
            List<string> final_final = new List<string>();
            List<string[]> final = new List<string[]>();
            for (int i = 0; i < x.Length; i++)
            {
                string[] this_item = x[i];
                if (recursiveContains(search_term, this_item))
                {
                    final.Add(this_item);
                }  
            }

            ambigiousResolver("");
            string[][] y = duplicateRemover(final.ToArray());
            return y[0][0];
        }

        private void ambigiousResolver(string term)
        {
            //Empty
            return;
        }

        private bool recursiveContains(string[] x, string[] y)
        {
            for (int i = 0; i < x.Length; i++)
            {
                string this_item = x[i].Replace(" ", string.Empty);
                for (int j = 0; j < y.Length; j++)
                {
                    if (string.Compare(this_item, y[j].Replace(" ", string.Empty), true) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool isInteger(string str)
        {
            int result = -1;
            return int.TryParse(str, out result);
        }

        private string[][] duplicateRemover(string[][] x)
        {
            List<string[]> final = new List<string[]>();
            List<string> final_strings = new List<string>();
            for (int i = 0; i < x.Length; i++)
            {
                string[] this_item = x[i];
                if (!this_item.ToString().IsIn(final_strings))
                {
                    final.Add(this_item);
                    final_strings.Add(this_item.ToString());
                }
            }

            return final.ToArray();
        }

        private int superFind(string[] x, string search_term)
        {
            int j = 0;
            for (int i = 0; i < x.Length; i++)
            {
                string this_item = x[i];
                if (this_item.Contains(search_term))
                {
                    j++;
                }
            }

            return j;
        }

        private string[] parse(string the_phrase)
        {
            string[] allowed_words = As.theWordsOf("delete remove undo move change find and them those replace");

            string[] english_numbers = As.theWordsOf("one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen");

            string current_block = null;

            bool in_replace_all = false;

            bool in_replace = false;

            bool in_remove = false;

            string[] separated_query = the_phrase.Split(' ');

            StringBuilder processed = new StringBuilder();

            for (int i = 0; i < separated_query.Length; i++)
            {
                string this_item = separated_query[i];

                if (i < separated_query.Length - 1)
                {
                    if (this_item.Is("replace") && separated_query[i + 1].Is("all"))
                    {
                        if (in_replace)
                        {
                            //set end of processed to "REPLACE_ALL:"
                            processed.AppendLine("REPLACE_ALL:");
                        }

                        in_replace_all = true;
                    }
                }

                if (this_item.Is("replace"))
                {                    
                    processed.AppendLine("{return, \"REPLACE:\"}");//?
                }

                if (this_item.IsIn("remove", "delete"))
                {
                    processed.AppendLine("REMOVE:");
                    in_remove = true;
                }

                if (this_item.Is("with"))
                {
                    processed.AppendLine("->");
                }

                if (this_item.Is("then"))
                {
                    processed.AppendLine(this_item);
                    if (!in_replace_all && !in_replace && !in_remove)
                    {
                        processed.AppendLine("return");
                    }

                    current_block = this_item;
                }

                if (this_item.IsIn("those", "them"))
                {
                    processed.AppendLine(current_block);
                }

                if (this_item.Is("move"))
                {
                    processed.AppendLine("return");
                    processed.AppendLine("MOVE:");
                }

                if (this_item.Is("left") && separated_query.HasNext(i))
                {
                    processed.AppendLine("->-");

                    //Optimization instead of checking if exists then finding index,
                    //just check if index is not -1
                    int possible_num_index = item_offset(separated_query[i + 1], english_numbers);
                    if (possible_num_index != -1)
                    {
                        int number = possible_num_index + 1; //one is at 0th index
                        processed.AppendLine(number.ToString());
                        processed.AppendLine("Y");
                    }
                }

                if (this_item.Is("up") && separated_query.HasNext(i))
                {
                    processed.AppendLine("->-");
                    int possible_num_index = item_offset(separated_query[i + 1], english_numbers);
                    if (possible_num_index != -1)
                    {
                        int number = possible_num_index + 1; //one is at 0th index
                        processed.AppendLine(number.ToString());
                        processed.AppendLine("X");
                    }
                }

                if (this_item.Is("down") && separated_query.HasNext(i))
                {
                    processed.AppendLine("->");
                    int possible_num_index = item_offset(separated_query[i + 1], english_numbers);
                    if (possible_num_index != -1)
                    {
                        int number = possible_num_index + 1; //one is at 0th index
                        processed.AppendLine(number.ToString());
                        processed.AppendLine("X");
                    }
                }

                if (this_item.Is("right") && separated_query.HasNext(i))
                {
                    processed.AppendLine("->");
                    int possible_num_index = item_offset(separated_query[i + 1], english_numbers);
                    if (possible_num_index != -1)
                    {
                        int number = possible_num_index + 1; //one is at 0th index
                        processed.AppendLine(number.ToString());
                        processed.AppendLine("Y");
                    }
                }

                if (this_item.IsIn("nevermind", "cancel", "abort", "stop", "undo"))
                {
                    processed.AppendLine("{return, \"CANCEL_LAST_COMMAND\"}");
                }
            }

            return findDuplicatesNextToEachOther(optimize(processed.ToString()));
        }

        //Better for future translations instead of System.String.IndexOf
        private int item_offset(string needle, string[] haystack)
        {
            for (int i = 0; i < haystack.Length; i++)
            {
                string this_item = haystack[i];
                if (needle.Is(this_item))
                {
                    return i;
                }
            }

            return -1;
        }

        private string[] filter_query(string[] the_query)
        {
            List<string> the_query_filtered = new List<string>();
            for (int i = 0; i < the_query.Length; i++)
            {
                string this_item = the_query[i];
                if (this_item.IsNot(string.Empty)) //Use !string.IsNullOrWhitespace?
                {
                    the_query_filtered.Add(this_item);
                }
            }

            return the_query_filtered.ToArray();
        }

        private string[] optimize(string query)
        {
            string[] the_query = query.Split('\r', '\n');

            //List<string> the_query_filtered = new List<string>();
            for (int i = 0; i < the_query.Length; i++)
            {
                string this_item = the_query[i];
                if (this_item.Is("CANCEL_LAST_COMMAND"))
                {
                    the_query[i - 1] = string.Empty;
                    the_query[i] = string.Empty;
                }
            }

            the_query = filter_query(the_query);
            for (int i = 0; i < the_query.Length - 1; i++)
            {
                string this_item = the_query[i];
                if (this_item.Is(the_query[i + 1]))
                {
                    the_query[i + 1] = string.Empty;
                }
            }

            the_query = filter_query(the_query);
            string last_item = null;
            for (int i = 0; i < the_query.Length; i++)
            {
                string this_item = the_query[i];
                if (i > 0)
                {
                    last_item = the_query[i - 1]; 
                }

                if (this_item.Is(last_item))
                {
                    the_query[i] = string.Empty;
                }

                if (i < the_query.Length - 1)
                {
                    if (this_item.Contains("REPLACE"))
                    {
                        if (the_query[i + 1].Contains("REMOVE"))
                        {
                            string item_one_subject = this_item.Split(':')[1];
                            string item_thing = item_one_subject.Split(new string[1] { "->" }, StringSplitOptions.None)[0];
                            item_one_subject = item_one_subject.Split(new string[1] { "->" }, StringSplitOptions.None)[1];
                            string item_two_subject = the_query[i + 1].Split(':')[1];
                            //log item_one_subject
                            //log item_two_subject
                            if (item_one_subject.Is(item_two_subject))
                            {
                                the_query[i] = "REMOVE:" + item_thing;
                                //set item (i + 1) of the_query to ""
                            }
                        }
                    }
                }
            }

            //Remove replace queries if their blocks have already been removed (nothing to replace)

            return the_query;
        }

        private string[] findDuplicatesNextToEachOther(string[] local_x)
        {
            List<string> final = new List<string>();
            for (int i = 0; i < local_x.Length; i++)
            {
                string this_item = local_x[i];
                if (i < local_x.Length - 1)
                {
                    if (this_item.IsNot(local_x[i + 1]))
                    {
                        final.Add(this_item);
                    }
                }
                else
                {
                    final.Add(this_item);
                }
            }

            return final.ToArray();
        }

        private void LoadX()
        {
            List<string[]> x = new List<string[]>();
            string content = File.ReadAllText("terms.txt").Substring(1);

            int brakcetIndex = -1;
            string currentContent = content;
            while ((brakcetIndex = currentContent.IndexOf("{")) != -1)
            {
                string bracketStr = currentContent.Substring(brakcetIndex + 1);
                int endBracket = bracketStr.IndexOf("}");

                string array = bracketStr.Substring(0, endBracket).Replace("\"", string.Empty);
                string[] arrayElements = array.Split(',');
                for (int i = 0; i < arrayElements.Length; i++)
                {
                    arrayElements[i] = arrayElements[i].Trim();
                }

                x.Add(arrayElements);
                currentContent = bracketStr.Substring(endBracket + 3 > currentContent.Length ? endBracket : endBracket + 3);
            }

            this.x = x.ToArray();
        }

        public Script()
        {
            LoadX();
        }

        public int[] Run()
        {
            //remove, replace, delete, move, cancel, undo*,
            string[] the_phrase = As.theWordsOf("remove all of the 883 blocks and replace those with 993 blocks then delete them thanks bye. replace all 883 with 993  Oh sorry could you move all of those blocks right seven blocks? Wait wait sorry cancel that last thing");

            List<string[]> final = new List<string[]>();
            List<string> tmp = new List<string>();

            bool FLAG_ONE = false;
            bool FLAG_TWO = false;
            bool FLAG_THREE = false;

            for (int i = 0; i < the_phrase.Length; i++)
            {
                if (FLAG_ONE)
                {
                    i++;
                    FLAG_ONE = false;
                }
                else if (FLAG_TWO)
                {
                    FLAG_TWO = false;
                    i++;
                }
                else if (FLAG_THREE)
                {
                    FLAG_THREE = false;
                    i++;
                }

                if (recursiveFind(this.x, the_phrase[i]))
                {
                    tmp.Add(the_phrase[i]);
                    //set FLAG_ONE to true

                    if (recursiveFind(this.x, the_phrase[i + 1]))
                    {
                        tmp.Add(the_phrase[i + 1]);
                        FLAG_TWO = true;

                        if (recursiveFind(this.x, the_phrase[i + 2]))
                        {
                            tmp.Add(the_phrase[i + 2]);
                            FLAG_THREE = true;
                        }
                    }
                }

                if (tmp.Count > 0)
                {
                    final.Add(tmp.ToArray());
                    tmp.Clear();
                }
            }

            List<int> final_ids = new List<int>();
            for (int i = 0; i < final.Count; i++)
            {
                string[] this_item = final[i];
                final_ids.Add(int.Parse(blockSearcher(this.x, this_item)));
            }

            //final_ids is just the ids. These ids need to replace the tokenized word. It is very important that this occurs.

            return final_ids.ToArray();
        }
    }
}
