using System.Text;

namespace MiniBot.Activity
{
    public static class StringBuilderExtension
    {
        public static string Space { get; set; } = "";

        public static void SpaceAppend(this StringBuilder sb, string value)
        {
            sb.Append(Space + value);
        }
    }
}
