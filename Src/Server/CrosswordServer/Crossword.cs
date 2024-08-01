using CrosswordServer.Models;
using CrosswordServer.Utils;

namespace CrosswordServer
{
    public class Crossword
    {
        public struct Field
        {
            internal char correctValue;
            internal bool isEdited;
            internal bool isGuessed;
        }

        private Field[][] crossword;
        private int emptyFields;

        public Crossword(List<CrosswordClue> clues)
        {
            crossword = CrosswordsLoader.GenerateCrosswordFields(clues, out int totalFields);
            emptyFields = totalFields;
        }

        public void SetFieldIsEdited(int x, int y, bool isEdited)
        {
            crossword[x][y].isEdited = isEdited;
        }

        public bool IsFieldEdited(int x, int y)
        {
            return crossword[x][y].isEdited;
        }

        public void SetFieldGuessed(int x, int y)
        {
            crossword[x][y].isGuessed = true;
        }

        public bool IsFieldGuessed(int x, int y)
        {
            return crossword[x][y].isGuessed;
        }

        public bool TryGuessField(int x, int y, char guess, out bool isCompleted)
        {
            isCompleted = false;
            if (char.ToUpper(crossword[x][y].correctValue) == char.ToUpper(guess))
            {
                emptyFields--;
                if (emptyFields == 0)
                {
                    isCompleted = true;
                }
                return true;
            }
            return false;
        }

        public char GetCorrectValue(int x, int y)
        {
            return crossword[x][y].correctValue;
        }
    }
}
