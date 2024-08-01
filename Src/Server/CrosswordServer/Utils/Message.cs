namespace CrosswordServer.Utils
{
    public static class Message
    {
        #region enums
        public enum CompleteState
        {
            VICTORY,
            DEFEAT,
            DRAW
        }
        public enum ErrorState
        {
            SERVER_FULL
        }
        #endregion

        public static string Connected()
        {
            return "CONNECT|";
        }
        public static string Crossword(string crossword)
        {
            return "CROSSWORD|" + crossword;
        }
        public static string FieldEdited(int x, int y, bool val)
        {
            return "EDIT|" + x + "," + y + "," + val;
        }
        public static string FieldGuessed(int x, int y, char correctValue)
        {
            return "GUESS|" + x + "," + y + "," + correctValue;
        }
        public static string Points(int playerPoints, int enemyPoints)
        {
            return "POINTS|" + playerPoints + "," + enemyPoints;
        }
        public static string Error(ErrorState state)
        {
            switch (state)
            {
                case ErrorState.SERVER_FULL:
                    return "ERROR|SERVER_IS_FULL";
                default:
                    return "ERROR|UNIDENTIFIED_ERROR";
            }
        }
        public static string Complete(CompleteState state, int playerPoints, int enemyPoints)
        {
            switch (state)
            {
                case CompleteState.VICTORY:
                    return "COMPLETE|VICTORY" + "," + playerPoints + "," + enemyPoints;
                case CompleteState.DEFEAT:
                    return "COMPLETE|DEFEAT" + "," + playerPoints + "," + enemyPoints;
                case CompleteState.DRAW:
                    return "COMPLETE|DRAW" + "," + playerPoints + "," + enemyPoints;
                default:
                    return "COMPLETE|ERROR";
            }
        }
        public static string Disconnect()
        {
            return "DISCONNECT|";
        }
    }
}
