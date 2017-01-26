namespace Droid_video
{
    public static class GetText
    {
        public enum Language
        {
            FRENCH,
            ENGLISH
        }
        public static Language CurrentLanguage = Language.ENGLISH;
        public static string Text(string text)
        {
            switch (CurrentLanguage)
            {
                case Language.FRENCH:
                    return Properties.ResourceFrench.ResourceManager.GetString(text);
                case Language.ENGLISH:
                    return Properties.ResourceEnglish.ResourceManager.GetString(text);
                default:
                    return text;
            }
        }
    }
}
