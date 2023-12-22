using Avalonia;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsGUI
{
   /// <summary>
   /// <b>Author: Samuel Gardner</b></br></br>
   /// </summary>
   internal class Analysis
   {
      public record struct WorksRow(DateTime? creation_date, string? language, bool? restricted, bool? complete, long? word_count, List<int>? tags);
      public record struct TagsRow(int? id, string? type, string? name, bool? canonical, long? cached_count, int? merger_id); //note, "merger_id" is not always present.
      public record struct ConsecutiveYearPair(int formerYear, int latterYear)
      {
         public ConsecutiveYearPair(int formerYear) : this(formerYear, formerYear + 1) { }
      };
      public record struct Discrimination(decimal lowerbound, decimal upperbound, string qualifier);
      public static readonly string[] tagsNamesLookupTable = new string[]
      {
         "Horror",
         "Hurt/Comfort",
         "Fluff",
         "Angst",
         "Romance",
         "Smut",
         "Science Fiction & Fantasy",
         "Domestic",
         "Drama",
         "Crack",
         "Humor",
         "Slice of Life",
         "Character Study",
         "Drabble",
         "Whump",
         "Action/Adventure"
      };
      private static readonly Discrimination[] discriminations = new[]
      {
         new Discrimination(0m, 0.01m, "unnoticable"),
         new Discrimination(0.01m, 0.05m, "small"),
         new Discrimination(0.05m, 0.10m, "decent"),
         new Discrimination(0.10m, decimal.MaxValue, "significant"),
      };

      /// <summary>
      /// This generates the data for the "percentage delta" graphs.
      /// </summary>
      public static async Task<List<((int source, int target) l, Dictionary<ConsecutiveYearPair, decimal> percentages)>> StepThreeAnalysis(List<(int sourcetarg, int targsource)>? timelineTagPairs = null)
      {
         if (timelineTagPairs == null)
         {
            timelineTagPairs = new List<(int sourcetarg, int targsource)>();
            for (int i = 0; i < tagsNamesLookupTable.Length; i++)
               for (int j = 0; j < i; j++)
               {
                  timelineTagPairs.Add((i, j));
               }
         }
         int year = 2011;
         Dictionary<int, IReadOnlyList<(int source, int target, int value)>> yearsData = new();
         //betcha didn't know you can do this with for loops in c#
         for (string p = $"GraphsGUI.embeds.csvs.links-{year}.csv"; year < 2021; ++year, p = $"GraphsGUI.embeds.csvs.links-{year}.csv")
         {
            Console.WriteLine($"Loading: {p}");
            ConcurrentBag<(int source, int target, int value)> yearData = new();

            string[] lines;
            using (Stream? tr = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(p))
            {
               if (tr != null)
               {
                  using (StreamReader sr = new StreamReader(tr, Encoding.UTF8))
                  {
                     lines = (await sr.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
                  }
               }
               else
                  throw new FileLoadException("Error: Tried to load the works csv from the embedded resources, didn't work.");
            }

            //string[] lines = File.ReadAllLines(p);
            Parallel.For(1, lines.Length, (i) => //skip headers
            {
               string[]? fields = lines[i].Split(',', StringSplitOptions.TrimEntries);
               if (fields != null && fields.Length == 4)
               {
                  //0th column is just a linear index, i.e., == i - 1
                  int source, target, value;
                  bool success = int.TryParse(fields[1], out source); //can't do and, short circuit eval can cause target/value to be unassigned
                  success &= int.TryParse(fields[2], out target);
                  success &= int.TryParse(fields[3], out value);
                  if (!success)
                     throw new Exception("Ophelia's data is malformed!");
                  yearData.Add((source, target, value));
               }
               else
                  throw new Exception("Ophelia's data is malformed!");
            });
            yearsData.Add(year, yearData.OrderBy(x => x.source).ThenBy(x => x.target).ToArray());
         }
         Console.WriteLine("Done loading... Time to analyize");
         ConcurrentDictionary<ConsecutiveYearPair, IReadOnlyCollection<(int source, int target, decimal percentageChange, string label)>> increases = new();
         Parallel.For(0, yearsData.Count - 1, (int i) =>
         {
            //i and i + 1
            var former = yearsData[i + 2011];
            var latter = yearsData[i + 2011 + 1]; //these were already sorted.
            List<(int source, int target, decimal percentageChange, string label)> increasesThisPair = new();
            for (int j = 0; j < former.Count; j++)
            {
               if (former[j].source != latter[j].source || former[j].target != latter[j].target)
                  throw new Exception("Error: implicitly sorted arrays did not have matching & comparable source/target indeces!");
               decimal percentage = former[j].value == 0m && latter[j].value == 0m ? 0m : former[j].value == 0 ? decimal.MaxValue : ((((decimal)latter[j].value) - ((decimal)former[j].value)) / ((decimal)former[j].value));
               string category = discriminations.FirstOrDefault(x =>
               {
                  decimal a = Math.Abs(percentage);
                  return (x.lowerbound < a && a <= x.upperbound) || (a == x.lowerbound && x.lowerbound == 0m); //in the case that a == decimal.MaxValue
               }, new Discrimination(-1m, -1m, "[unknown qualifier]")).qualifier;
               increasesThisPair.Add((former[j].source, former[j].target, percentage, category));
            }
            increases.AddOrUpdate(new ConsecutiveYearPair(i + 2011), (_) => increasesThisPair, (_, _) => increasesThisPair);
         });
         StringBuilder sb = new StringBuilder();
         int longestLabel = tagsNamesLookupTable.MaxBy(x => x.Length)?.Length ?? 0;
         Func<(int source, int target, decimal percentageChange, string label), string> stringifyer = x =>
         {
            return $"{tagsNamesLookupTable[x.source].PadRight(longestLabel)} <-> {tagsNamesLookupTable[x.target].PadRight(longestLabel)}: " +
            $"{(x.percentageChange == decimal.MaxValue ? "infinite" :
            (x.percentageChange < 0 ? -x.percentageChange : x.percentageChange).ToString("P"))} " +
            $"{(x.percentageChange < 0 ? "decrease" : "increase")}, {x.label} change.";
         };
         foreach (var e in increases.OrderBy(x => x.Key.formerYear))
         {
            sb.AppendLine($"Analysis of years {e.Key.formerYear}-{e.Key.latterYear}:\n");
            int topX = 10;
            sb.AppendLine($"Top {topX} most changed:");
            sb.AppendLine(string.Join('\n', e.Value.OrderByDescending(x => x.percentageChange).Take(topX).Select(stringifyer)));
            sb.AppendLine("\nAll of them:");
            sb.AppendLine(string.Join('\n', e.Value.Select(stringifyer)));
            sb.AppendLine("\n\n\n");
         }
         var res = new List<((int source, int target) l, Dictionary<ConsecutiveYearPair, decimal> percentages)>();
         foreach (var e in timelineTagPairs)
         {
            int source, target;
            if (increases.Values.First().Any(x => x.source == e.sourcetarg && x.target == e.targsource))
            {
               source = e.sourcetarg;
               target = e.targsource;
            }
            else
            {
               source = e.targsource;
               target = e.sourcetarg;
            }
            Dictionary<ConsecutiveYearPair, decimal> dd = new();
            sb.AppendLine($"Timeline of {tagsNamesLookupTable[source]}-{tagsNamesLookupTable[target]}:\n");
            foreach (var i in increases.OrderBy(x => x.Key.formerYear))
            {
               sb.Append($"{i.Key.formerYear}-{i.Key.latterYear}: ");
               var r = i.Value.FirstOrDefault(x => x.target == target && x.source == source, new(-1, -1, decimal.MinValue, "[unknown]"));
               sb.AppendLine(stringifyer(r));
               dd.Add(i.Key, r.percentageChange);
            }
            res.Add(((source, target), dd));
         }
         string fn = "Step 3 analysis.txt";
         File.WriteAllText(fn, sb.ToString());
         Console.WriteLine("Output written to:\n" + Path.GetFullPath(fn));
         return res;
      }

      /// <summary>
      /// This generates the data for "total works" graph, and the log graphs for tag count.
      /// </summary>
      private static async Task WorksSimpleAnalysis()
      {
         Console.WriteLine("Beginning to parse the works file...");
         ConcurrentBag<WorksRow> works = new();
         {
            string[] worksLines;
            using (Stream? tr = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphsGUI.embeds.csvs.works-20210226.csv"))
            {
               if (tr != null)
               {
                  using (StreamReader sr = new StreamReader(tr, Encoding.UTF8))
                  {
                     worksLines = (await sr.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
                  }
               }
               else
                  throw new FileLoadException("Error: Tried to load the works csv from the embedded resources, didn't work.");
            }
            //string[] worksLines = File.ReadAllLines(worksCSVFilepath);
            Parallel.For(1, worksLines.Length, (i) => //skip headers
            {
               {
                  string[]? fields = worksLines[i].Split(',', StringSplitOptions.TrimEntries);
                  if (fields == null)
                  {
                     Console.WriteLine($"[Works] Line: #{i + 1} failed to be read.");
                  }
                  else if (fields.Length < 6)
                  {
                     Console.WriteLine($"[Works] Line: #{i + 1} failed to be read. (Improper column count?)");
                  }
                  else
                  {
                     bool scd = DateTime.TryParse(fields[0], out DateTime cd);
                     DateTime? creation_date = scd ? cd : null;
                     string? language = fields[1] ?? null;
                     bool sr = bool.TryParse(fields[2], out bool r);
                     bool? restricted = sr ? r : null;
                     bool sc = bool.TryParse(fields[3], out bool c);
                     bool? complete = sc ? c : null;
                     bool swc = long.TryParse(fields[4], out long wc);
                     long? word_count = swc ? wc : null;
                     List<int> tagsids;
                     if (string.IsNullOrWhiteSpace(fields[5]) || fields[5].Trim() == "\"\"") //tagless works are "", e.g., line 867447
                        tagsids = new List<int>();
                     else
                        tagsids = new List<int>(fields[5].Split('+',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries).Select(int.Parse) ?? new int[] { });
                     works.Add(new WorksRow(creation_date, language, restricted, complete, word_count, tagsids));
                  }
               }
            });
         }
         Console.WriteLine("Beginning to parse the tags file...");
         ConcurrentBag<TagsRow> tags = new();
         {
            string[] tagsLines;
            using (Stream? tr = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphsGUI.embeds.csvs.tags-20210226.csv"))
            {
               if (tr != null)
               {
                  using (StreamReader sr = new StreamReader(tr, Encoding.UTF8))
                  {
                     tagsLines = (await sr.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
                  }
               }
               else
                  throw new FileLoadException("Error: Tried to load the tags csv from the embedded resources, didn't work.");
            }
            //string[] tagsLines = File.ReadAllLines(tagsCSVFilepath);
            Parallel.For(1, tagsLines.Length, (i) =>
            {
               {
                  string[]? fields = tagsLines[i].Split(',', StringSplitOptions.TrimEntries);
                  if (fields == null)
                  {
                     Console.WriteLine($"[Tags] Line: #{i + 1} failed to be read.");
                  }
                  else if (fields.Length < 5)
                  {
                     Console.WriteLine($"[Tags] Line: #{i + 1} failed to be read. (Improper column count?)");
                  }
                  else
                  {
                     bool si = int.TryParse(fields[0], out int ii);
                     int? id = si ? ii : null;
                     string? type = fields[1];
                     string? name = fields[2];
                     bool sc = bool.TryParse(fields[3], out bool c);
                     bool? canonical = sc ? c : null;
                     bool scc = long.TryParse(fields[4], out long cc);
                     long? cached_count = scc ? cc : null;
                     int? merger_id = null;
                     if (fields.Length > 5)
                     {
                        bool smi = int.TryParse(fields[5], out int mi);
                        merger_id = smi ? mi : null;
                     }
                     tags.Add(new TagsRow(id, type, name, canonical, cached_count, merger_id));
                  }
               }
            });
         }
         Console.WriteLine("Beginning stat analysis...");
         //do whatever you want with works & tags...
         Dictionary<int, int> workYearToCount = new(); //for each year, how many works in that year?
         Dictionary<int, int> tagCountCount = new(); //for each number, how many works have exactly that number of tags?
         Dictionary<int, int> tagCountRestrictedCount = new(); //for each number, for each work that has exactly that number of tags, how many of them are restricted?
         foreach (var e in works)
         {
            if (e.creation_date.HasValue)
            {
               if (workYearToCount.ContainsKey(e.creation_date.Value.Year))
               {
                  workYearToCount[e.creation_date.Value.Year] += 1;
               }
               else
               {
                  workYearToCount.Add(e.creation_date.Value.Year, 1);
               }
            }
            if (e.tags != null)
            {
               if (tagCountCount.ContainsKey(e.tags.Count))
               {
                  tagCountCount[e.tags.Count] += 1;
               }
               else
               {
                  tagCountCount.Add(e.tags.Count, 1);
               }
               if (e.restricted.HasValue && e.restricted.Value)
               {
                  if (tagCountRestrictedCount.ContainsKey(e.tags.Count))
                  {
                     tagCountRestrictedCount[e.tags.Count] += 1;
                  }
                  else
                  {
                     tagCountRestrictedCount.Add(e.tags.Count, 1);
                  }
               }
            }
         }
         //print & save
         StringBuilder sb = new StringBuilder();
         sb.Append("How many works per year:\n");
         sb.Append(string.Join('\n', workYearToCount.OrderBy(x => x.Key).Select(x => $"Year: {x.Key}, How many works in that year: {x.Value}")) + "\n");
         sb.Append("\n");
         sb.Append("How many works exist for any given amount of tags:\n");
         sb.Append(string.Join('\n', tagCountCount.OrderBy(x => x.Key).Select(x => $"Tag Count: {x.Key}, How many works with that many tags: {x.Value}")) + "\n");
         sb.Append("\n");
         sb.Append("How many restricted works exist for any given amount of tags:\n");
         sb.Append(string.Join('\n', tagCountRestrictedCount.OrderBy(x => x.Key).Select(x => $"Tag Count: {x.Key}, How many restricted works with that many tags: {x.Value}")) + "\n");
         Console.WriteLine(sb.ToString());
         sb.Append("\n\n\nSanitized:\n\n\n");
         sb.Append("How many works per year:\n");
         sb.Append(string.Join('\n', workYearToCount.OrderBy(x => x.Key).Select(x => x.Key.ToString().PadRight(5) + x.Value.ToString())) + "\n");
         sb.Append("How many works exist for any given amount of tags:\n");
         sb.Append(string.Join('\n', tagCountCount.OrderBy(x => x.Key).Select(x => x.Key.ToString().PadRight(4) + x.Value.ToString())) + "\n");
         sb.Append("How many restricted works exist for any given amount of tags:\n");
         sb.Append(string.Join('\n', tagCountRestrictedCount.OrderBy(x => x.Key).Select(x => x.Key.ToString().PadRight(4) + x.Value.ToString())) + "\n");
         File.WriteAllText("stats.txt", sb.ToString());
      }

      private static Dictionary<A, B> ConvertConToNormal<A, B>(ConcurrentDictionary<A, B> con) where A : notnull
      {
         return con.ToDictionary(x => x.Key, x => x.Value);
      }
   }
}
