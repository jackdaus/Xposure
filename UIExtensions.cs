using StereoKit;

namespace StereoKitApp
{
    internal class UIExtensions
    {
        public static bool Button(string text, bool disabled = false, Color? color = null)
        {
            if (disabled)
            {
                UI.PushTint(Color.Black);
            }

            if (color != null)
            {
                UI.PushTint(color.Value);
            }


            bool buttonPressed = UI.Button(text);


            if (color != null)
            {
                UI.PopTint();
            }

            if (disabled)
            {
                UI.PopTint();
            }

            return disabled ?  false : buttonPressed;
        }
    }
}
