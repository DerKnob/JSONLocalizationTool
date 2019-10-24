using JSONLocalizationTool.Helper;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace JSONLocalizationTool.WPF
{
    public static class ColorManager
    {
        public static Color AccentColor
        {
            get { return GetAccentColor(); }
        }

        public static Color BackgroundColor
        {
            get { return GetBackgroundColor(); }
        }

        public static Color PrimaryTextColor
        {
            get { return GetPrimaryTextColor(); }
        }

        private static Color GetAccentColor()
        {
            if (EnvironmentUtils.IsWindowsVersion8OrHigher())
            {
                try
                {
                    IntPtr pElementName = Marshal.StringToHGlobalUni(ImmersiveColors.ImmersiveStartSelectionBackground.ToString());
                    Color color = GetColor(pElementName);
                    return color;
                }
                catch (Exception)
                {
                }
            }

            return (Color)Application.Current.FindResource("ColorControlBackgroundSelectedBlue");
        }

        private static Color GetBackgroundColor()
        {
            if (EnvironmentUtils.IsWindowsVersion8OrHigher())
            {
                try
                {
                    IntPtr pElementName = Marshal.StringToHGlobalUni(ImmersiveColors.ImmersiveStartBackground.ToString());
                    Color color = GetColor(pElementName);
                    return color;
                }
                catch (Exception)
                {
                }
            }

            return (Color)Application.Current.FindResource("ColorControlBorderSelectedBlue");
        }

        private static Color GetPrimaryTextColor()
        {
            if (EnvironmentUtils.IsWindowsVersion8OrHigher())
            {
                try
                {
                    IntPtr pElementName = Marshal.StringToHGlobalUni(ImmersiveColors.ImmersiveStartPrimaryText.ToString());
                    Color color = GetColor(pElementName);
                    return color;
                }
                catch (Exception)
                {
                }
            }

            return (Color)Application.Current.FindResource("ColorControlFontAccentInvert");
        }

        private static Color GetColor(IntPtr pElementName)
        {
            var colourset = StarScreenColorsHelper.GetImmersiveUserColorSetPreference(false, false);
            uint type = StarScreenColorsHelper.GetImmersiveColorTypeFromName(pElementName);
            Marshal.FreeCoTaskMem(pElementName);
            uint colourdword = StarScreenColorsHelper.GetImmersiveColorFromColorSetEx((uint)colourset, type, false, 0);
            byte[] colourbytes = new byte[4];
            colourbytes[0] = (byte)((0xFF000000 & colourdword) >> 24); // A
            colourbytes[1] = (byte)((0x00FF0000 & colourdword) >> 16); // B
            colourbytes[2] = (byte)((0x0000FF00 & colourdword) >> 8); // G
            colourbytes[3] = (byte)(0x000000FF & colourdword); // R
            Color color = Color.FromArgb(colourbytes[0], colourbytes[3], colourbytes[2], colourbytes[1]);
            return color;
        }

    }
}
