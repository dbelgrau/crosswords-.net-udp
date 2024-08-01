namespace Crosswords.Utils
{
    public static class Message
    {
        public static string Connect()
        {
            return "CONNECT|";
        }

        public static string Ready()
        {
            return "READY|";
        }

        public static string FieldEdit(int x, int y, bool isEdited)
        {
            return "EDIT|" + x + "," + y + "," + isEdited;
        }

        public static string FieldGuess(int x, int y, char guess)
        {
            return "GUESS|" + x + "," + y + "," + guess;
        }

        public static string End()
        {
            return "DISCONNECT|";
        }
    }
}
