using CrosswordServer.Models;
using Newtonsoft.Json;
using static CrosswordServer.Crossword;

namespace CrosswordServer.Utils
{
    internal static class CrosswordsLoader
    {
        public static List<CrosswordClue> LoadAndDeserializeCrossword(string path)
        {
            string json = LoadCrosswordFromFile(path);
            List<CrosswordClue> crossworClue = JsonConvert.DeserializeObject<List<CrosswordClue>>(json);

            return crossworClue;
        }

        public static string LoadCrosswordFromFile(string path)
        {
            string crosswordJson = File.ReadAllText(path);
            return crosswordJson;
        }

        public static Field[][] GenerateCrosswordFields(List<CrosswordClue> clues, out int fields)
        {
            int maxX = clues.Max(clue => clue.StartPoint.X + (clue.Horizontal ? clue.Word.Length - 1 : 0));
            int maxY = clues.Max(clue => clue.StartPoint.Y + (clue.Horizontal ? 0 : clue.Word.Length - 1));

            var crossword = new Field[maxX + 1][];

            for (int i = 0; i <= maxX; i++)
            {
                crossword[i] = new Field[maxY + 1];

                for (int j = 0; j <= maxY; j++)
                {
                    crossword[i][j] = new Field();
                }
            }

            int totalFields = 0;

            foreach (var clue in clues)
            {
                int x = clue.StartPoint.X;
                int y = clue.StartPoint.Y;

                for (int i = 0; i < clue.Word.Length; i++)
                {
                    if (crossword[x + (clue.Horizontal ? i : 0)][y + (clue.Horizontal ? 0 : i)].correctValue == '\0')
                    {
                        crossword[x + (clue.Horizontal ? i : 0)][y + (clue.Horizontal ? 0 : i)].correctValue = clue.Word[i];
                        totalFields++;
                    }
                }
            }

            fields = totalFields;
            return crossword;
        }

    }
}
